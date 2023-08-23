using BackflowService;
using Microsoft.DotNet.DarcLib.VirtualMonoRepo;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

var runId = Guid.NewGuid().ToString();
var vmrPath = $"/mnt/data/vmr/{runId}";
var tmpPath = "/mnt/data/tmp";

builder.Services.AddLogging(b =>
    b.AddConsole(options =>
        options.FormatterName = SimpleConsoleLoggerFormatter.FormatterName)
     .AddConsoleFormatter<SimpleConsoleLoggerFormatter, SimpleConsoleFormatterOptions>(
        options => options.TimestampFormat = "[HH:mm:ss] "));

// Add start up processes
builder.Services.AddTransient<IStartupFilter>(sp => ActivatorUtilities.CreateInstance<VmrInitStartupFilter>(sp, vmrPath));

// Add services to the container.
builder.Services.AddControllers();

// Register background services
builder.Services.AddHostedService(sp => ActivatorUtilities.CreateInstance<RequestProcessor>(sp, vmrPath));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<GitFileManagerFactory>();
builder.Services.AddVmrManagers(
    sp => sp.GetRequiredService<GitFileManagerFactory>(),
    "git",
    vmrPath,
    tmpPath,
    gitHubToken: null,
    azureDevOpsToken: null);
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
