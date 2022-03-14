using DwFramework.Core;
using DwFramework.Web;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using ProtoBuf;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.Configuration;
using System.Net;
using System.Text;

namespace WebExample;

class Program
{
    static async Task Main(string[] args)
    {
        var host = new ServiceHost();
        host.AddJsonConfiguration("config.json", false, true);
        host.ConfigureLogging(builder => builder.UserNLog());
        host.ConfigureWebHostDefaults(webHostBuilder =>
        {
            webHostBuilder.UseKestrel((context, options) =>
            {
                var config = context.Configuration.ParseConfiguration<Config.Http>();
                foreach (var item in config.Listens)
                {
                    options.Listen(string.IsNullOrEmpty(item.Ip) ? IPAddress.Any : IPAddress.Parse(item.Ip), item.Port, listenOptions =>
                    {
                        listenOptions.Protocols = item.Protocols == HttpProtocols.None ? HttpProtocols.Http1 : item.Protocols;
                        if (item.UseSSL) listenOptions.UseHttps(item.Cert, item.Password);
                    });
                }
            });
            webHostBuilder.ConfigureServices(services =>
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
                });
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("name", new OpenApiInfo()
                    {
                        Title = "title",
                        Version = "version",
                        Description = "description"
                    });
                });
                services.AddControllers().AddJsonOptions(options =>
                {
                    //不使用驼峰样式的key
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    //不使用驼峰样式的key
                    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                });
                services.AddMvc(options => options.UseRoutePrefix("api"));
                services.AddWebSocket();
                services.AddRpc();
                services.AddRazorPages();
                services.AddServerSideBlazor();
                services.AddAntDesign();
            });
            webHostBuilder.Configure(app =>
            {
                app.UseCors("any");
                app.UseRouting();
                app.UseStaticFiles();
                app.UseSwagger(c => c.RouteTemplate = "{documentName}/swagger.json");
                app.UseSwaggerUI(c => c.SwaggerEndpoint($"/{"name"}/swagger.json", "desc"));
                app.UseWebSocket();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapRpcFromAssemblies();
                    endpoints.MapBlazorHub();
                    endpoints.MapFallbackToPage("/_Host");
                });
            });
        });
        // host.ConfigureSocket(configuration, "tcp");
        // host.ConfigureSocket(configuration, "udp");
        host.OnHostStarted += async p =>
        {
            var web = p.GetWebSocket();
            web.OnConnect += async (c, a) => Console.WriteLine($"{c.ID} 建立连接");
            web.OnReceive += async (c, a) => web.BroadCast(a.Data);
            web.OnClose += async (c, a) => Console.WriteLine($"{c.ID} 断开连接");

            // Task.Run(async () =>
            // {
            //     try
            //     {
            //         await Task.Delay(3000);
            //         using var channel = GrpcChannel.ForAddress("http://localhost:9001");
            //         var service = channel.CreateGrpcService<IGreeterService>();
            //         var r = await service.SayHelloAsync(new HelloRequest()
            //         {
            //             Name = "XXX"
            //         });
            //         Console.WriteLine(r.Message);
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine(ex.Message);
            //     }
            // });

            // var tcp = p.GetTcp();
            // tcp.OnConnect += (c, a) => Console.WriteLine($"{c.ID} connected");
            // tcp.OnReceive += (c, a) =>
            // {
            //     Console.WriteLine($"{c.ID} received {Encoding.UTF8.GetString(a.Data)}");
            //     var body = @"<h1>Hello World</h1><span>XXXXXXX</span>";
            //     _ = c.SendAsync(Encoding.UTF8.GetBytes(
            //         "HTTP/1.1 200 OK\r\n"
            //         + "Date: Sat, 31 Dec 2005 23:59:59 GMT\r\n"
            //         + "Content-Type: text/html;charset=UTF8\r\n"
            //         + $"Content-Length: {Encoding.UTF8.GetByteCount(body)}\r\n\n"
            //         + $"{body}"
            //     ));
            // };
            // tcp.OnSend += (c, a) => Console.WriteLine($"{c.ID} sent {Encoding.UTF8.GetString(a.Data)}");
            // tcp.OnClose += (c, a) => Console.WriteLine($"{c.ID} closed");


            // var udp = p.GetUdp();
            // udp.OnReceive += (c, a) =>
            // {
            //     Console.WriteLine($"{c} received {Encoding.UTF8.GetString(a.Data)}");
            //     udp.SendTo(Encoding.UTF8.GetBytes("World"), c);
            // };
            // udp.OnSend += (c, a) => Console.WriteLine($"{c} sent {Encoding.UTF8.GetString(a.Data)}");
        };
        await host.RunAsync();
    }
}

[Service]
public interface IGreeterService
{
    [Operation]
    Task<HelloReply> SayHelloAsync(HelloRequest request,
        CallContext context = default);
}

[ProtoContract]
public class HelloRequest
{
    [ProtoMember(1)]
    public string Name { get; set; }
}

[ProtoContract]
public class HelloReply
{
    [ProtoMember(1)]
    public string Message { get; set; }
}

[RPC]
public class GreeterService : IGreeterService
{
    public Task<HelloReply> SayHelloAsync(HelloRequest request, CallContext context = default)
    {
        return Task.FromResult(
            new HelloReply
            {
                Message = $"Hello {request.Name}"
            });
    }
}