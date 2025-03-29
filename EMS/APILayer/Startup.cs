using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using EMS2.Services;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EMS2
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add controllers
            services.AddControllers();

            // Initialize CosmosClient with account and key from appsettings.json
            var cosmosClient = new CosmosClient(
                _configuration["CosmosDb:Account"],
                _configuration["CosmosDb:Key"]
            );

            // Fetch the DatabaseName and ContainerNames from appsettings.json
            var databaseName = _configuration["CosmosDb:DatabaseName"];
            var containerNameEvents = _configuration["CosmosDb:ContainerNameEvents"];
            var containerNameRegistrationContainer = _configuration["CosmosDb:containerNameRegistrationContainer"]; 
            var containerNameUsers = _configuration["CosmosDb:containerNameUsers"];

            // Register the CosmosClient as a Singleton Service
            services.AddSingleton(cosmosClient);

            // Register the Events Container
            services.AddSingleton(s =>
            {
                return cosmosClient.GetContainer(databaseName, containerNameEvents);
            });

            // Register the Participants Container
            services.AddSingleton(s =>
            {
                return cosmosClient.GetContainer(databaseName, containerNameRegistrationContainer);
            });

            // Register the EventService
            services.AddSingleton<IEventService>(s =>
            {
                return new EventService(
                    s.GetRequiredService<CosmosClient>(),
                    databaseName,
                    containerNameEvents
                );
            });

            // Register the ParticipantService
            services.AddSingleton<IParticipantService>(s =>
            {
                return new ParticipantService(
                    s.GetRequiredService<CosmosClient>(),
                    databaseName,
                    containerNameRegistrationContainer);
            });
            // Register the Users
            services.AddSingleton<IUserService>(s =>
            {
                return new UserService(
                    s.GetRequiredService<CosmosClient>(),
                    databaseName,
                    containerNameUsers);
            });


            // JWT Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
                    };
                });

            // Configure Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Event API",
                    Description = "Event Management API",
                    TermsOfService = new Uri("https://kaarinfotech.com/Privacy.html"),
                    Contact = new OpenApiContact
                    {
                        Name = "Dhanush",
                        Email = "dhanush.as@kaarinfotech.com",
                        Url = new Uri("https://LinkedIn.com/kaarinfotech.com/contactus"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "KAAR Infotech",
                        Url = new Uri("https://kaarinfotech.com"),
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
