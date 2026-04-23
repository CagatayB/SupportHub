using SupportHub.Application.Interfaces;
using SupportHub.Application.Services;
using SupportHub.Infrastructure.Persisteance;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5070; // launchSettings.json'daki HTTPS portun neyse onu yaz
});

// builder.Configuration ile appsettings.json içindeki verilere erişiyoruz
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SupportHubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Bu satırı ekleyerek TicketService'in arayüzü bulmasını sağlıyoruz
builder.Services.AddScoped<ISupportHubDbContext>(provider =>
    provider.GetRequiredService<SupportHubDbContext>());


builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();