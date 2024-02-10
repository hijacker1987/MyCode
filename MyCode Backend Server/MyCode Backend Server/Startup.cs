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
using MyCode_Backend_Server.Controllers;

namespace MyCode_Backend_Server
{
    public class Startup(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

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
            services.AddScoped<IDbInitializer, DbInitializer>();

            var connection = _configuration["ConnectionString"];
            services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));

            var issuer = _configuration["IssueAudience"];
            var issueSign = _configuration["IssueSign"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddCookie(c => { c.Cookie.Name = "Authorization"; })
                    .AddJwtBearer(options =>
                    {
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
                    .AddEntityFrameworkStores<DataContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            var connection = _configuration["FEAddress"];

            app.UseCors(builder =>
            {
                builder.WithOrigins(connection!)
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials();
            });

            app.UseAuthentication();

            app.UseAuthorization();

            if (env.IsEnvironment("Test"))
            {
                dataContext.Database.EnsureCreated();
            }

            if (dbInitializer != null)
            {
                using var scope = app.ApplicationServices.CreateScope();
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<DataContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                    dbInitializer.Initialize(context, userManager, roleManager).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Startup>>();
                    logger.LogError(ex, "Error when initializing the Database!");
                }
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
