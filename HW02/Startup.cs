using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HW02.CustomSettings;
using HW02.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HW02
{
    public class Startup
    {
        /// <summary>
        /// initilize instance of <see cref="Startup"/> class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="hostingEnvironment"></param>
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

        // <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        public IHostingEnvironment HostingEnvironment { get; }
        /// <summary>
        /// Gets the configuration settings
        /// </summary>
        /// <value>
        /// The configuration settings
        /// </value>
        public IConfiguration Configuration { get; }

        
        ///<summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            //service for db stuff with entity framework
            services.AddDbContext<TaskDatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //swagger
            services.AddSwaggerGen(ConfigureSwaggerUI);

            services.AddMvc();

            //support for injection of IOptions
            services.AddOptions();

            //support for TaskLimits section
            services.Configure<TasksLimits>(Configuration.GetSection("TasksLimits"));

            //support for generic IConfiguration access for generic string access
            services.AddSingleton<IConfiguration>(Configuration);

        }

        /// <summary>
        /// Swagger: configure the swagger UI
        /// </summary>
        /// <param name="swaggerGenOptions">The swaggerGenOptions</param>
        private void ConfigureSwaggerUI(SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.SwaggerDoc("v1", new Info { Title = "Tasks", Version = "v1" });

            // Naming the file CustomerDemo.config so that it won't be returned externally but be returned to the Swashbuckle code.
            var filePath = Path.Combine(HostingEnvironment.ContentRootPath, "codedoc.config");
            swaggerGenOptions.IncludeXmlComments(filePath);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// configure the http request pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            // SWAGGER: Insert middleware to expose the generated Swagger as JSON endpoints
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    // Necessary for API management so it has the proper values for the Backend service URL otherwise 
                    // you will see an error in trace similar to "Backend service URL is not defined"
                    swaggerDoc.Host = httpReq.Host.Value;
                });
            });

            // SWAGGER: swagger-ui-middleware to expose interactive documentation
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tasks");
            });

        }
    }
}
