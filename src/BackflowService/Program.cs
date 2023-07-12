using BackflowService;
using Microsoft.DotNet.DarcLib.VirtualMonoRepo;

var builder = WebApplication.CreateBuilder(args);

var runId = Guid.NewGuid().ToString();
var vmrPath = $"/var/data/vmr/{runId}";

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
    "/var/data/tmp",
    gitHubToken: null,
    azureDevOpsToken: null);

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
