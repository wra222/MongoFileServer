using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonGo.Services;
using log4net;
using log4net.Config;
using System;
using MonGo.Entity;
using System.IO;
using log4net.Repository;

namespace MonGo
{
    public class Startup
    {
        public static ILoggerRepository repository { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            repository = LogManager.CreateRepository("CoreLogRepository");
            XmlConfigurator.Configure(repository, new FileInfo("config/log4net.config"));
            Log4NetRepository.loggerRepository = repository;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                options.ExcludedHosts.Add("192.168.8.6");
                //options.ExcludedHosts.Add("www.example.com");
            });
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 2001;
            });

            //services.AddScoped<BookService>();
            services.AddScoped<FileService>();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
             
          
            //app.UseHttpsRedirection();
            app.UseMvc();
            //ServiceEntity serviceEntity = new ServiceEntity
            //{
            //    IP = NetworkHelper.LocalIPAddress,
            //    Port = Convert.ToInt32(Configuration["Service:Port"]),
            //    ServiceName = Configuration["Service:Name"],
            //    ConsulIP = Configuration["Consul:IP"],
            //    ConsulPort = Convert.ToInt32(Configuration["Consul:Port"])
            //};
            //app.RegisterConsul(lifetime, serviceEntity);
        }
    }
}
