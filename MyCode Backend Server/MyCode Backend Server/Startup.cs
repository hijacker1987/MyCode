using Microsoft.OpenApi.Models;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Authentication.Token;
using MyCode_Backend_Server.Service.Email_Sender;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using MyCode_Backend_Server.Data.Service;
using MyCode_Backend_Server.Hubs;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Bot;
using MyCode_Backend_Server.Service.Chat;
using System.Security.Claims;

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

            services.AddSignalR();
            services.AddSingleton<IDictionary<string, ChatRooms>>(options => new Dictionary<string, ChatRooms>());
            services.AddScoped<IChatService, ChatService>();

            services.AddScoped<IFAQBot, FAQBot>();
            services.AddScoped<FAQBotData>();

            services.AddScoped<DummyData>();

            if (_environment.IsEnvironment("Test"))
            {
                services.AddScoped<IDbInitializer, TestDbInitializer>();
            }
            else if (_environment.IsEnvironment("Development"))
            {
                services.AddScoped<IDbInitializer, DbInitializer>();
            }

            var connection = Environment.GetEnvironmentVariable("ConnectionString");
            var testConnection = _configuration["ConnectionStrings:TestConnectionString"];

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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "Authorization";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.HttpOnly = true;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            })
            .AddGoogle("Google", options =>
            {
                if (_environment.IsEnvironment("Test"))
                {
                    options.ClientId = "GoogleTestClientId";
                    options.ClientSecret = "GoogleTestClientSecret";
                }
                else if (_environment.IsEnvironment("Development"))
                {
                    options.ClientId = Environment.GetEnvironmentVariable("GoogleClientId")!;
                    options.ClientSecret = Environment.GetEnvironmentVariable("GoogleClientSecret")!;
                }
            })
            .AddFacebook("Facebook", options =>
            {
                if (_environment.IsEnvironment("Test"))
                {
                    options.ClientId = "FacebookTestClientId";
                    options.ClientSecret = "FacebookTestClientSecret";
                }
                else if (_environment.IsEnvironment("Development"))
                {
                    options.ClientId = Environment.GetEnvironmentVariable("FacebookClientId")!;
                    options.ClientSecret = Environment.GetEnvironmentVariable("FacebookClientSecret")!;
                }
            })
            .AddGitHub("GitHub", options =>
            {
                if (_environment.IsEnvironment("Test"))
                {
                    options.ClientId = "GitHubTestClientId";
                    options.ClientSecret = "GitHubTestClientSecret";
                }
                else if (_environment.IsEnvironment("Development"))
                {
                    options.ClientId = Environment.GetEnvironmentVariable("GitHubClientId")!;
                    options.ClientSecret = Environment.GetEnvironmentVariable("GitHubClientSecret")!;
                    options.CallbackPath = new PathString("/signin-github");
                    options.Scope.Add("user:email");

                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
                    options.ClaimActions.MapJsonKey(ClaimValueTypes.Email, "email");
                }
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
                    builder.WithOrigins(frontConnection!, "https://accounts.google.com/", "https://www.github.com/")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app,
                              DataContext dataContext,
                              IDbInitializer dbInitializer,
                              IDbInitializer testDbInitializer)
        {
            if (_environment.IsEnvironment("Development"))
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
                var faqData = services.GetRequiredService<FAQBotData>();
                var dummyData = services.GetRequiredService<DummyData>();

                if (_environment.IsEnvironment("Test"))
                {
                    testDbInitializer.Initialize(dataContext, userManager, roleManager, faqData, dummyData).Wait();
                }
                else if (_environment.IsEnvironment("Development"))
                {
                    dbInitializer.Initialize(dataContext, userManager, roleManager, faqData, dummyData).Wait();
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Startup>>();
                logger.LogError(ex, "Error when initializing the Database!");
            }

            app.UseWebSockets(new() { KeepAliveInterval = TimeSpan.FromSeconds(30) });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<MessageHub>("/message");
            });
        }
    }
}
