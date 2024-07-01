using College.BLL.Exceptions;
using College.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureApplication();

// Add services to the container.
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.ConfigureMySqlContext(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.AddCustomServices(builder.Environment);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSerilog(builder);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName=="Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.ApplyMigrations();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.SeedRoles(builder).Wait();
    scope.ServiceProvider.SeedAdmin(builder).Wait();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
