using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Configuration;
using WebApi.Data;
using WebApi.Domain;
using WebApi.Middleware;
using WebApi.Services;
using WebApi.Services.Interfaces;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(GetConnectionStringFromConfig()));

            services.AddScoped<IAuthorizationService, DefaultAuthorizationService>();

            var jwtSettings = GetJwtSettings();

            services.AddSingleton(jwtSettings);

            // Jwt setup
            TokenValidationParameters tokenValidationParameters;

            if (Environment.IsDevelopment())
            {
                // We do not validate the audience in a development environment 
                tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = "https://localhost:5001",
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true
                };
            } else
            {
                // TODO: Figure out valid issuer and audience for this api
                tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true
                };
            }

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // MVC middleware

            app.UseRouting();

            // API clients may not understand or obey redirects from HTTP to HTTPS. Such clients may send information over HTTP. Web APIs should either:
            // Not listen on HTTP.
            // Close the connection with status code 400(Bad Request) and not serve the request.
            // app.UseHttpsRedirection();

            // The default API projects don't include HSTS because HSTS is generally a browser only instruction. Other callers, such as phone or desktop apps, do not obey the instruction. Even within browsers, a single authenticated call to an API over HTTP has risks on insecure networks. The secure approach is to configure API projects to only listen to and respond over HTTPS.
            // app.UseHsts();

            app.UseAuthentication();
            app.UseAuthorization();

            // Custom middleware

            app.UseUnlistenHttpMiddleware();

            // Mapping controllers individually from their own routings in their corresponding classes.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private string GetConnectionStringFromConfig()
        {
            using StreamReader reader = new StreamReader(@"config.txt");
                return reader.ReadLine().Split('"')[1];
        }

        private JwtSettings GetJwtSettings()
        {
            using (StreamReader reader = new StreamReader(@"config.txt"))
            {
                // Skip the first line
                reader.ReadLine();

                string[] settings = reader.ReadLine().Split('"')[1].Split(';');
                return new JwtSettings { Secret = settings[0].Split('=')[1], TokenLifetime = TimeSpan.Parse(settings[1].Split('=')[1]) };
            }
        }
    }
}