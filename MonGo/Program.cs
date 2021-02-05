using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace MonGo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            //BuildWebHost(args ).Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                              .AddJsonFile("config/host.json")
                              
                              .Build();

            var url = configuration["http"];
            return WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration((context, config) => {
                config.AddJsonFile("Config/appsettings.json", false, true); //3.最后一个参数就是是否热更新的布尔值
            })
                .UseStartup<Startup>()
                .UseUrls(url);
        }
         
        //public static IWebHost BuildWebHost(string[] args) =>
        //   WebHost.CreateDefaultBuilder(args)
        //       .UseStartup<Startup>()
        //       .UseUrls("http://*:5000")
        //       //.UseUrls(config.GetConnectionString<string>("uri"))
        //       .Build();
    }
}
