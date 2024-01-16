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

            var connection = _configuration["ConnectionString"];

            services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));

            var issuer = _configuration["IssueAudience"];
            var issueSign = _configuration["IssueSign"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        if (issueSign != null)
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

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, DataContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                var connection = _configuration["FEAddress"];

                context.Response.Headers.Append("Access-Control-Allow-Origin", connection);
                context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
                context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");

                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                }
                else
                {
                    await next();
                }
            });

            app.UseAuthentication();

            app.UseAuthorization();

            if (env.IsEnvironment("Test"))
            {
                await dataContext.Database.EnsureCreatedAsync();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            lifetime.ApplicationStarted.Register(async () =>
            {
                await AddRolesAndAdmin(app);
            });
        }

        public async Task AddRolesAndAdmin(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var roleList = new List<string> { "Admin", "User" };

            foreach (var role in roleList)
            {
                await CreateRole(roleManager, role);
            }

            await CreateAdminIfNotExists(userManager);
        }

        public async Task CreateRole(RoleManager<IdentityRole<Guid>> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(Guid.NewGuid().ToString()) { Name = role });
            }
        }

        public async Task CreateAdminIfNotExists(UserManager<User> userManager)
        {
            var pass = _configuration["APass"];
            var mail = _configuration["AEmail"];
            var phone = _configuration["ACall"];
            var uName = _configuration["UName"];
            var dName = _configuration["AName"];

            var adminInDb = await userManager.FindByEmailAsync(mail!);

            if (adminInDb == null)
            {
                var admin = new User { UserName = uName, Email = mail, DisplayName = dName, PhoneNumber = phone };
                var adminCreated = await userManager.CreateAsync(admin, pass!);

                if (adminCreated.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
