using AlarmaApi.Controllers;
using AlarmaApi.Procesos;

namespace AlarmaApi
{
    public class Program
    {
        public static AlarmaApi.Comunicacion.Arduino arduino;
        private static Thread heartBeatThread = null;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseKestrel();
            builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory())
            .UseUrls("http://*:5000", "https://*:5001","http://*:8080")
            .UseIISIntegration()
            
            ;
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("*"));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            arduino = Comunicacion.Arduino.GetInstance();
            heartBeatThread = new Thread(() => HeartBeat.Inicio(arduino));
            heartBeatThread.Start();
            app.Run();
        }
    }
}