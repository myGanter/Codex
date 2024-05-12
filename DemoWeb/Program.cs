using Codex.AspNet;
using Codex.AspNet.Decorators;
using Codex.AspNet.Dtos;
using Codex.AspNet.EntityFrameworkCore;
using Codex.Cache;
using Codex.CQRS;
using DemoWeb.Controllers;
using DemoWeb.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetSection("SQLiteConnectionString").Value;
builder.Services.AddDbContext<DbContext, DemoWebSQLiteContext>(x => x.UseSqlite(connectionString));

//Codex infrastructure
builder.Services.AddCodex();
builder.Services.AddCodexEntityFrameworkCore();

//Handlers & decorators
builder.Services.AddScoped<IAsyncHandler<ParseNumberDto, string, ErrorDto>, ParseNumberAsyncHandler>();
builder.Services.AddDecorator(typeof(ParseNumberAfterDecorator));

//Configure pipeline
DecoratorsPipeLine.FromAsyncHandler<ParseNumberDto, string, ErrorDto>()
    .Before<IAsyncHandler<ParseNumberDto, string, ErrorDto>, AsyncValidationDecorator<ParseNumberDto, string>>()
    .After<IAsyncHandler<ParseNumberDto, string, ErrorDto>, ParseNumberAfterDecorator>();

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