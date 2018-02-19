using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HW02.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HW02
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //setup the webhost
            IWebHost host = BuildWebHost(args);

            //get dependency injection for creating services
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    //get the database context service
                    var context = services.GetRequiredService<TaskDatabaseContext>();

                    //initilize data
                    DBInitilizer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "*** FATAL ERROR *** Could not seed the database. ****FATAL ERROR ***");
                }
            }
                // run the app
                host.Run();
            }
        

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
