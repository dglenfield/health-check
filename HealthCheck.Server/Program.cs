global using HealthCheck.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHealthChecks()
    .AddCheck("ICMP_01", new ICMPHealthCheck("www.amazon.com", 100))
    .AddCheck("ICMP_02", new ICMPHealthCheck("www.google.com", 100))
    .AddCheck("ICMP_03", new ICMPHealthCheck($"www.{Guid.NewGuid():N}.com", 100));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AngularPolicy", cfg =>
    {
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
        cfg.WithOrigins(builder.Configuration["AllowedCORS"]);
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AngularPolicy");

app.UseHealthChecks(new PathString("/api/health"), new CustomHealthCheckOptions());

app.MapControllers();

app.MapMethods("/api/heartbeat", ["HEAD"], () => Results.Ok());

app.MapHub<HealthCheckHub>("api/health-hub");

app.MapFallbackToFile("/index.html");

app.Run();
