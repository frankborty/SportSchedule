var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICsvProcessorService, NBAScheduleCSVProcessorService>();
builder.Services.AddDbContext<SportScheduleDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SportScheduleDb")));

builder.Services.AddCors(options =>
{
    CorsConfigurator.ConfigureCors(builder.Configuration, options);
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors("FrontendOnly");
app.UseAuthorization();
app.MapControllers();

app.Run();
