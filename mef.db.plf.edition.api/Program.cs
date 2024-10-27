using E.Loi.Edition.Generation.Generation.amendment;
using E.Loi.Edition.Generation.Generation.Nodes;
using E.Loi.Edition.Generation.Generation.Plf;
using E.Loi.Edition.Generation.Generation.TextLaw;
using E.Loi.Edition.Generation.Generation.VotingFile;
using E.Loi.Edition.Generation.Services;
using E.Loi.Edition.Generation.VotingFile;
using E.Loi.Services.DB;
using E.Loi.Services.Repositories;
using E.Loi.Services.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<LawDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Db")));
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddScoped<GenerationVotingFile>();
builder.Services.AddScoped<GenerateVoteResult>();
builder.Services.AddScoped<GenerateVotingFileCommission>();
builder.Services.AddScoped<GenerateVotingFileForPresident>();
builder.Services.AddScoped<GenerationAmendments>();
builder.Services.AddScoped<GenerateNode>();
builder.Services.AddScoped<GeneratePLF>();
builder.Services.AddScoped<GenerateTextLaw>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
//builder.Services.AddCors(options => options.AddDefaultPolicy(builder => builder.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://192.168.1.29:8082/api/").AllowCredentials()));

var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(cors => cors.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
app.MapControllers();

app.Run();
