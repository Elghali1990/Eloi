using E.Loi.Services.DB;
using E.Loi.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace E.Loi.Api
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<LawDbContext>(options => options
            .UseSqlServer(builder.Configuration.GetConnectionString("Db")));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddControllersWithViews()
                    .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );
            builder.Services.AddHttpClient();
            builder.Services.AddServerSideBlazor().AddCircuitOptions(options => options.DetailedErrors = true);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
           // object value = builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
            builder.Services.AddCors();
            var app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors(cors => cors.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().WithOrigins("*"));
            app.MapControllers();

            app.Run();
        }
    }
}
