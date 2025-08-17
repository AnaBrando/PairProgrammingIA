using IAPairProgrammer.Data;
using IAPairProgrammer.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // ou porta do seu front
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Adicione isso antes do app.MapControllers()

// app.MapControllers() ou app.UseEndpoints(...)

builder.Services.AddHttpClient<IOpenAiService, OpenAiService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=App_Data/database.db"));

builder.Services.AddScoped<IMemoryService, MemoryService>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
