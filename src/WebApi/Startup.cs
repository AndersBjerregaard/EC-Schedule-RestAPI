using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Domain;
using WebApi.Middleware;
using WebApi.Services;
using WebApi.Services.Interfaces;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(GetConnectionStringFromConfig()));

            services.AddScoped<IAuthorizationService, DefaultAuthorizationService>();
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
                return reader.ReadLine().Split(':')[1];
        }
    }
}
