using E.Loi.Excel;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSignalR(e =>
{
    e.MaximumReceiveMessageSize = 102400000;
});
//Log.Logger = new LoggerConfiguration()
//            .MinimumLevel.Error()
//            .Enrich.With(new ThreadIdEnricher())
//            .Enrich.FromLogContext()
//            .WriteTo.Console()
//            .WriteTo.File("C:\\logsFront\\log-.txt", rollingInterval: RollingInterval.Day)
//            .CreateLogger();
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddServerSideBlazor().AddCircuitOptions(options => options.DetailedErrors = true);
builder.Services.AddBlazoredToast();
builder.Services.AddHttpClient("api", httpClient => httpClient.BaseAddress = new Uri(builder.Configuration["Api"]!));
builder.Services.AddHttpClient("edition", httpClient => httpClient.BaseAddress = new Uri(builder.Configuration["Edition"]!));
builder.Services.AddHttpClient("lawApi", httpClient => httpClient.BaseAddress = new Uri(builder.Configuration["LoiApi"]!));
builder.Services.AddHttpClient("Echange", httpClient => httpClient.BaseAddress = new Uri(builder.Configuration["Echange"]!));
builder.Services.AddHttpClient("UrlFinance", httpClient => httpClient.BaseAddress = new Uri(builder.Configuration["UrlFinance"]!));
builder.Services.AddControllersWithViews()
              .AddNewtonsoftJson(options =>
              options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
              );
builder.Services.AddAntDesign();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<StateContainerService>();
builder.Services.AddScoped<SessionStorageService>();
builder.Services.AddScoped<ReadJsonFileService>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<PrintService>();
builder.Services.Configure<RoleOptions>(builder.Configuration.GetSection("RoleOptions"));
builder.Services.Configure<PhaseOptions>(builder.Configuration.GetSection("PhaseOptions"));
builder.Services.AddScoped<ILawRepository, LawRepository>();
builder.Services.AddScoped<INodeRepository, NodeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAmendmentRepository, AmendmentRepository>();
builder.Services.AddScoped<IEditionRepository, EditionRepository>();
builder.Services.AddScoped<IVoteNodeRepository, VoteNodeRepository>();
builder.Services.AddScoped<INodeTypeRepository, NodeTypeRepository>();
builder.Services.AddScoped<IVoteAmendmentRepository, VoteAmendmentRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ILegislativeRepository, LegislativeRepository>();
builder.Services.AddScoped<ILegislativeYearsRepository, LegislativeYearsRepository>();
builder.Services.AddScoped<ILegislativeSessionsRepository, LegislativeSessionsRepository>();
builder.Services.AddScoped<IPhaseRepository, PhaseRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
builder.Services.AddScoped<IStatuRepository, StatuRepository>();
builder.Services.AddScoped<IEchangeService, EchangeService>();
builder.Services.AddScoped<ITraceService, TraceService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddCors();


//builder.Host.UseSerilog();
//builder.Services.AddScoped<ErrorHandlingMiddleware>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
//app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.UseStatusCodePagesWithRedirects("/404");
app.Run();
