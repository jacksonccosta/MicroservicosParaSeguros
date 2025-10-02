using ContratacaoService.Core.Ports;
using ContratacaoService.Core.Services;
using ContratacaoService.Infrastructure.Adapters;
using ContratacaoService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContratacaoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IContratacaoRepository, ContratacaoRepository>();
builder.Services.AddScoped<IPropostaGateway, PropostaHttpGateway>();
builder.Services.AddScoped<ContratacaoUseCaseService>();

builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Proposta Service API - Jackson Costa",
        Version = "v1",
        Description = "Microsserviço responsável pelo gerenciamento do ciclo de vida de contratações de seguro."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }