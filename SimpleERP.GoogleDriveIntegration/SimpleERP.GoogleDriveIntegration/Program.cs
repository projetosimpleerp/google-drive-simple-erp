using Hangfire;
using Hangfire.JobsLogger;
using Serilog;
using SimpleERP.GoogleDriveIntegration.Filters;
using SimpleERP.GoogleDriveIntegration.Jobs;
using SimpleERP.GoogleDriveIntegration.Services.Drive;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .CreateLogger();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDriveUploaderService, DriveUploaderService>();

builder.Services.AddHangfire(conf => conf.UseRecommendedSerializerSettings().UseInMemoryStorage().UseJobsLogger());

// Define a quantidade de retentativas
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 3, DelaysInSeconds = new int[] { 300 } });
builder.Services.AddHangfireServer();

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

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AuthorizationFilter() }
});

Jobs.StartJobs(builder.Configuration);

app.Run();
