namespace ApiSostenibilitat
{
    using ApiSostenibilitat.Data;
    using ApiSostenibilitat.Models;
    using ApiSostenibilitatDef.Tools;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
    using System.Text;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Afegim DbContext
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine(connectionString);
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                connectionString,
                sqlServerOptionsAction:
                options => options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: System.TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)
                )
            );

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                // Configuració de contrasenyes
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;

                // Configuració del correu electrònic
                options.User.RequireUniqueEmail = true;

                // Configuració de lockout (bloqueig d’usuari)
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Configuració del login
                options.SignIn.RequireConfirmedEmail = false; // true si vols que es confirmi el correu
            })
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Configuracio del Token i les seves validacions
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings["Issuer"],

                        ValidateAudience = true,
                        ValidAudience = jwtSettings["Audience"],

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                options.JsonSerializerOptions.MaxDepth = 64;
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            new string[]{}
                        }
                });
            });

            //-------------------------------------------------SIGNALR----------------------------
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("https://localhost:7246"); //ClienteRazor
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowCredentials();
                });
            });
            builder.Services.AddSignalR();

            //---------------------------------------------- Middlewares ---------------------------//
            var app = builder.Build();

            // Crear rols inicials: Admin i User
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Crear roles iniciales
                await RoleTools.CrearRolsInicials(services);

                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Verifica els usuaris ya existeixen
                var existingUser = await userManager.FindByEmailAsync("user@gmail.com");
                var existingAdmin = await userManager.FindByEmailAsync("admin@gmail.com");
                var existingDoctor = await userManager.FindByEmailAsync("doctor@gmail.com");
                if (existingAdmin == null)
                {
                    Console.WriteLine("Creant usuari admin...");

                    var adminUser = new User
                    {
                        UserName = "Admin",
                        Email = "admin@gmail.com",
                        Name = "Admin",
                        Surname = "Admin",
                        Weight = 68,
                        Exercise = ExerciciEnum.Molt,
                        HoursSleep = 8,
                        Age = 23
                    };

                    var createResult = await userManager.CreateAsync(adminUser, "Itb2025@");

                    if (!createResult.Succeeded)
                    {
                        foreach (var error in createResult.Errors)
                        {
                            Console.WriteLine($"Error creant usuari admin: {error.Code} - {error.Description}");
                        }
                    }
                    else
                    {
                        using var innerScope = app.Services.CreateScope();
                        var scopedUserManager = innerScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                        var scopedRoleManager = innerScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                        var newUser = await scopedUserManager.FindByEmailAsync("admin@gmail.com");

                        if (newUser != null && await scopedRoleManager.RoleExistsAsync("Admin"))
                        {
                            await scopedUserManager.AddToRoleAsync(newUser, "Admin");
                        }
                    }
                }
                if (existingUser == null)
                {
                    Console.WriteLine("Creant usuari...");

                    var user = new User
                    {
                        UserName = "User",
                        Email = "user@gmail.com",
                        Name = "User",
                        Surname = "User",
                        Weight = 50,
                        Exercise = ExerciciEnum.Poc,
                        HoursSleep = 8,
                        Age = 19
                    };

                    var createResult = await userManager.CreateAsync(user, "Itb2025@");

                    if (!createResult.Succeeded)
                    {
                        foreach (var error in createResult.Errors)
                        {
                            Console.WriteLine($"Error creant usuari: {error.Code} - {error.Description}");
                        }
                    }
                    else
                    {
                        using var innerScope = app.Services.CreateScope();
                        var scopedUserManager = innerScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                        var scopedRoleManager = innerScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                        var newUser = await scopedUserManager.FindByEmailAsync("user@gmail.com");

                        if (newUser != null && await scopedRoleManager.RoleExistsAsync("User"))
                        {
                            await scopedUserManager.AddToRoleAsync(newUser, "User");
                        }
                    }
                }

                if (existingDoctor == null)
                {
                    Console.WriteLine("Creant usuari doctor...");

                    var docUser = new User
                    {
                        UserName = "Doctor",
                        Email = "doctor@gmail.com",
                        Name = "Doctor",
                        Surname = "doctor",
                        Weight = 90,
                        Exercise = ExerciciEnum.Poc,
                        HoursSleep = 9,
                        Age = 42
                    };

                    var createResult = await userManager.CreateAsync(docUser, "Itb2025@");

                    if (!createResult.Succeeded)
                    {
                        foreach (var error in createResult.Errors)
                        {
                            Console.WriteLine($"=Error creant usuari doctor: {error.Code} - {error.Description}");
                        }
                    }
                    else
                    {
                        using var innerScope = app.Services.CreateScope();
                        var scopedUserManager = innerScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                        var scopedRoleManager = innerScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                        var newUser = await scopedUserManager.FindByEmailAsync("doctor@gmail.com");

                        if (newUser != null && await scopedRoleManager.RoleExistsAsync("Doctor"))
                        {
                            await scopedUserManager.AddToRoleAsync(newUser, "Doctor");
                        }
                    }
                }

                    // Configure the HTTP request pipeline.
                    if (app.Environment.IsDevelopment())
                    {
                        app.MapOpenApi();
                        app.UseSwagger();
                        app.UseSwaggerUI();
                    }

                    app.UseHttpsRedirection();
                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.UseCors();

                    app.MapControllers();

                    app.Run();
                }
            }
        }
    }


