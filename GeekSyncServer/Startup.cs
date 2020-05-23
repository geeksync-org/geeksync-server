using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net.WebSockets;
using GeekSyncServer.Internal;
using GeekSyncServer.Exceptions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GeekSyncServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public class RemoveVersionFromParameter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var versionParameter = operation.Parameters.Single(p => p.Name == "version");
                operation.Parameters.Remove(versionParameter);
            }
        }

        public class ReplaceVersionWithExactValueInPath : IDocumentFilter
        {
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                OpenApiPaths np = new OpenApiPaths();
                foreach (string s in swaggerDoc.Paths.Keys)
                {
                    np.Add(s.Replace("{version}", swaggerDoc.Info.Version), swaggerDoc.Paths[s]);
                }
                swaggerDoc.Paths = np;
                /*= swaggerDoc.Paths
                    .ToDictionary(
                        path => path.Key.Replace("v{version}", swaggerDoc.Info.Version),
                        path => path.Value
                    );*/
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version as 1.0
                config.DefaultApiVersion = new ApiVersion(0, 3);
                // If the client hasn't specified the API version in the request, use the default API version number 
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            services.AddSwaggerGen(c =>
            {

            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0.2", new OpenApiInfo { Title = "geeksync-server API", Version = "0.2" });
                c.SwaggerDoc("v0.3", new OpenApiInfo { Title = "geeksync-server API", Version = "0.3" });

                c.OperationFilter<RemoveVersionFromParameter>();
                c.DocumentFilter<ReplaceVersionWithExactValueInPath>();

                c.DocInclusionPredicate((version, desc) =>
        {
            var versions = desc.CustomAttributes()
                .OfType<ApiVersionAttribute>()
                .SelectMany(attr => attr.Versions);

            var maps = desc.CustomAttributes()
                .OfType<MapToApiVersionAttribute>()
                .SelectMany(attr => attr.Versions)
                .ToArray();

            return versions.Any(v => $"v{v.ToString()}" == version)
                          && (!maps.Any() || maps.Any(v => $"v{v.ToString()}" == version)); ;
        });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //SSL will be handled on k8s ingress
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
                {
                    // TOTO: logger: Console.WriteLine("Request for: " +context.Request.Path.ToString() );
                    if (context.Request.Path.ToString().StartsWith("/ws/"))
                    {
                        if (context.WebSockets.IsWebSocketRequest)
                        {
                            // TOTO: logger: Console.WriteLine("Got WS request on "+context.Request.Path.ToString());
                            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                            string[] path = context.Request.Path.ToString().Split('/');
                            string channelID = path[2];

                            try
                            {
                                Channel channel = ChannelManager.Instance[Guid.Parse(channelID)];
                                if (channel != null)
                                {
                                    await channel.ConnectWebSocket(webSocket);
                                }
                                else
                                {
                                    context.Response.StatusCode = 404;
                                }
                            }
                            catch (FormatException)
                            {
                                context.Response.StatusCode = 400;
                            }

                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                        }
                    }
                    else
                    {
                        await next();
                    }

                });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v0.2/swagger.json", "geeksync-server API v.0.2");
                    c.SwaggerEndpoint("/swagger/v0.3/swagger.json", "geeksync-server API v.0.3");
                    c.DisplayOperationId();
                    c.DisplayRequestDuration();
                });



        }
    }

}
