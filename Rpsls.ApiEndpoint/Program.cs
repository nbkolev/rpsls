using GameEngine;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rpsls.ApiEndpoint.Controllers;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        builder.Services.AddHttpClient();
        builder.Services.Configure<GameControlerOptions>(
            builder.Configuration.GetSection(GameControlerOptions.ConfigSectionName));

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        //At the moment no app.UseAuthorization(); is necessary.

        app.MapControllers();

        app.Run();
    }
}