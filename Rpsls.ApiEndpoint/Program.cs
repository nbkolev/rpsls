using System.Diagnostics.CodeAnalysis;
using GameEngine;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rpsls.ApiEndpoint.Controllers;

internal class Program
{
    [ExcludeFromCodeCoverage]
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddHttpClient();
        builder.Services.Configure<GameControlerOptions>(
            builder.Configuration.GetSection(GameControlerOptions.ConfigSectionName));
        
        
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.WriteIndented = true;
            

        });
        
        builder.Services.AddControllers();
        
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }
       
        
        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        } 
        else
        {
            app.UseExceptionHandler("/Error");
        }

        //In production it should run behind reverse proxy, so no
        //app.UseHttpsRedirection(); is necessary.
        
        //At the moment no app.UseAuthorization(); is necessary.

        app.MapControllers();

        app.Run();
    }
}