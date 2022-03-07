using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Services
{
    public class Startup
    {
        private const string DEFAULT_ORIGIN = nameof(DEFAULT_ORIGIN);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddInfrastructure(Configuration);
            services.AddHttpContextAccessor();
            services.AddHealthChecks();
            services.AddCors(options =>
            {
                options.AddPolicy(DEFAULT_ORIGIN,
                    builder =>
                    {
                        builder
                            .WithOrigins("http://localhost:8083")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders("Content-Disposition");
                    });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API",
                });

                c.SchemaGeneratorOptions.GeneratePolymorphicSchemas = true;

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bolt Service V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHealthChecks("/self", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseHealthChecks("/ready", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("ready")
            });

            app.UseCors(DEFAULT_ORIGIN);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

                endpoints.MapControllers();
            });
        }
    }
}
