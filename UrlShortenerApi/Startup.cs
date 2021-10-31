using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

using UrlShortener.Application.DbContext;
using UrlShortener.Application.Services;

namespace UrlShortener.Application
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

            services.AddSingleton<IUrlShortener, UrlShortenerService>();
            services.AddSingleton<IKeyGenerator, HashKeyGenerator>();
            services.AddSingleton<IBaseEncoder, Base62Encoder>();
            

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UrlShortenerApi", Version = "v1", Description="Rest API for URL Shortener" });
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddHttpContextAccessor();
            services.AddEntityFrameworkSqlite().AddDbContext<UrlShortenerContext>();


            services.AddProblemDetails(options =>
            {
                options.ShouldLogUnhandledException = (_, _, _) => true;
                options.IncludeExceptionDetails = (_, _) => true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UrlShortenerApi v1"));
            
            app.UseProblemDetails();

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            app.UseSerilogRequestLogging(options =>
            {
                // Customize the message template
                options.MessageTemplate = "Handled {RequestPath}";

                // Emit debug-level events instead of the defaults
                options.GetLevel = (_, _, _) => LogEventLevel.Debug;

                //// Attach additional properties to the request completion event
                //options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                //{
                //    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                //    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                //};
            });
        }
    }
}
