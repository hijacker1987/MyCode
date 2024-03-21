using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Service.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Email_Sender;
using IEmailSender = MyCode_Backend_Server.Service.Email_Sender.IEmailSender;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MyCode_Backend_Server
{
    public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IWebHostEnvironment _environment = environment;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddTransient<IEmailSender, EmailSender>();

            if (_environment.IsEnvironment("Test"))
            {
                services.AddScoped<IDbInitializer, TestDbInitializer>();
            }
            else if (_environment.IsEnvironment("Development"))
            {
                services.AddScoped<IDbInitializer, DbInitializer>();
            }

            var connection = _configuration["ConnectionString"];
            var testConnection = _configuration["TestConnectionString"];

            services.AddDbContext<DataContext>(options =>
            {
                if (_environment.IsEnvironment("Test"))
                {
                    options.UseSqlServer(testConnection);
                }
                else if (_environment.IsEnvironment("Development"))
                {
                    options.UseSqlServer(connection);
                }
            });

            services.AddAuthentication(o => {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = "Google";
            })
                    .AddCookie(options => {
                        options.Cookie.Name = "Authorization";
                        options.LoginPath = "/account/google-login";
                        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                        options.Cookie.SameSite = SameSiteMode.None;
                        options.Cookie.HttpOnly = true;
                    })
                    .AddGoogle("Google", options =>
                    {
                        options.ClientId = _configuration["GoogleClientId"]!;
                        options.ClientSecret = _configuration["GoogleClientSecret"]!;
                    })
                    .AddJwtBearer(options =>
                    {
                        var issuer = "";
                        var issueSign = "";

                        if (_environment.IsEnvironment("Test"))
                        {
                            issuer = "api With Test Authentication comes and goes here";
                            issueSign = "V3ryStr0ngP@ssw0rdW1thM0reTh@n256B1tsF0rT3st1ng";
                        }
                        else if (_environment.IsEnvironment("Development"))
                        {
                            issuer = _configuration["IssueAudience"];
                            issueSign = _configuration["IssueSign"];
                        }

                        if (issueSign != null)
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ClockSkew = TimeSpan.FromMinutes(5),
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = issuer,
                                ValidAudience = issuer,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issueSign)),
                            };
                            options.Events = new JwtBearerEvents
                            {
                                OnMessageReceived = context =>
                                {
                                    context.Token = context.Request.Cookies["Authorization"];
                                    return Task.CompletedTask;
                                }
                            };
                        }
                    });

            services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
                    .AddRoles<IdentityRole<Guid>>()
                    .AddEntityFrameworkStores<DataContext>()
                    .AddDefaultTokenProviders();

            var frontConnection = _configuration["FEAddress"];

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(frontConnection!, "https://accounts.google.com/")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext, IDbInitializer dbInitializer, IDbInitializer testDbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            dataContext.Database.EnsureCreated();

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<DataContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                if (_environment.IsEnvironment("Test"))
                {
                    testDbInitializer.Initialize(dataContext, userManager, roleManager).Wait();
                }
                else if (_environment.IsEnvironment("Development"))
                {
                    dbInitializer.Initialize(dataContext, userManager, roleManager).Wait();
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Startup>>();
                logger.LogError(ex, "Error when initializing the Database!");
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
