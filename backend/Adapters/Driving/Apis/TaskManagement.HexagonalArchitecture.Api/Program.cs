using Microsoft.EntityFrameworkCore;
using TaskManagement.HexagonalArchitecture.Application;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.HexagonalArchitecture.Api.Common.ExtensionMethods.v1;
using TaskManagement.HexagonalArchitecture.Api.Common.Handlers.v1;
using TaskManagement.HexagonalArchitecture.Database;
using TaskManagement.HexagonalArchitecture.Database.Repositories.v1;
using TaskManagement.HexagonalArchitecture.Database.UnitOfWork.v1;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.UnitOfWork.v1;

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

            builder.Services.AddValidation();
            builder.Services.AddApplicationModule();
            builder.Services.AddSwaggerDocumentation();

            builder.Services.AddAuthorization(builder.Configuration);
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
                app.UseExceptionHandler("/Error");
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

        // private static void AddDatabaseModule(this IServiceCollection services, IConfiguration configuration)
        // {
        //     var connectionString = configuration.GetConnectionString("SqlServerConnection") ??
        //                            throw new InvalidOperationException(
        //                                "Connection string 'SqlServerConnection' not found.");
        //     services.AddDbContext<ApplicationDbContext>(options =>
        //         options.UseSqlServer(connectionString));
        //
        //     services.AddDatabaseDeveloperPageExceptionFilter();
        //
        //     services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
        //         .AddEntityFrameworkStores<ApplicationDbContext>();
        //
        //     services.AddScoped<IUnitOfWork, UnitOfWork>();
        //     services.AddScoped<ICommentRepository, CommentRepository>();
        //     services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        // }
    }
}