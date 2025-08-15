using TaskManagement.HexagonalArchitecture.Application;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.HexagonalArchitecture.Api.Common.ExtensionMethods.v1;
using TaskManagement.HexagonalArchitecture.Api.Common.Handlers.v1;
using TaskManagement.HexagonalArchitecture.Database;

namespace TaskManagement.HexagonalArchitecture.Api
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddCors();
            builder.Services.AddControllers();

            builder.Services.AddValidation();
            builder.Services.AddApplicationModule();
            builder.Services.AddSwaggerDocumentation();

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddDatabaseModule(builder.Configuration);

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(options =>
                options
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseExceptionHandler();

            app.Run();
        }
    }
}