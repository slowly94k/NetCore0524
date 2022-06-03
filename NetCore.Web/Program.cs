using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetCore.Services.Data;
using Microsoft.Extensions.DependencyInjection;

namespace NetCore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run(); 20.

            var webHost = CreateHostBuilder(args).Build();

            using (var scope = webHost.Services.CreateScope())
            {
                DBFirstDbInitializer initializer = scope.ServiceProvider
                                                        .GetService<DBFirstDbInitializer>();

                int rowAffected = initializer.PlantSeedData();
            }

            webHost.Run();

        }
        //21.
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
       
                .ConfigureLogging(builder => builder.AddFile(options =>
                {
                    options.LogDirectory = "Logs";      //�α���������
                    options.FileName = "log-";          //�α��������ξ�. log-20180000.txt
                    options.FileSizeLimit = null;       //�α����� ������ ���� (10MB)
                    options.RetainedFileCountLimit = null;  //�α����� �������� (2)
                }))
       
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
