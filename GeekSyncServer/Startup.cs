using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

namespace GeekSyncServer
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

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "geeksync-server API", Version = "0.2" });
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
                    if (context.Request.Path.ToString().StartsWith("/ws/"))
                    {
                        if (context.WebSockets.IsWebSocketRequest)
                        {
                            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                            string[] path=context.Request.Path.ToString().Split('/');
                            string pairing=path[2];
                            string desktop=path[3];

                            try
                            {
                                Channel channel=ChannelManager.Instance[Guid.Parse(pairing)];
                                if (channel!=null) 
                                {
                                    await channel.ConnectWebSocket(Guid.Parse(desktop),webSocket);
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
                            catch(DesktopNotConnectedException)
                            {
                                context.Response.StatusCode = 404;
                            }
                            catch(DesktopWebSocketAlreadyConnectedException)
                            {
                                context.Response.StatusCode = 400;
                            }
                            catch(DesktopWebSocketException)
                            {
                                context.Response.StatusCode = 500;
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
            });

        }
    }
}
