using CodexCQRS.AspNet;
using CodexCQRS.AspNet.Decorators;
using CodexCQRS.AspNet.Dtos;
using CodexCQRS.AspNet.EntityFrameworkCore;
using CodexCQRS.Cache;
using CodexCQRS.CQRS;
using DemoWeb.Controllers;
using DemoWeb.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetSection("SQLiteConnectionString").Value;
builder.Services.AddDbContext<DbContext, DemoWebSQLiteContext>(x => x.UseSqlite(connectionString));

//Codex infrastructure
builder.Services.AddCodex();
builder.Services.AddCodexEntityFrameworkCore();

//Handlers & decorators
builder.Services.AddAsyncHandler<ParseNumberAsyncHandler, ParseNumberDto, string, ErrorDto>();
builder.Services.AddDecorator(typeof(ParseNumberAfterDecorator));

builder.Services.AddAsyncHandler<GenerateStringAsyncHandler, IGenerateStringAsyncHandler, GenerateStringDto, GenerateStringResultDto, ErrorDto>();
builder.Services.AddDecorator(typeof(GenerateStringToUpperAsyncDecorator<>));

//Configure pipeline
DecoratorsPipeLine.FromAsyncHandler<ParseNumberDto, string, ErrorDto>()
    .Before<IAsyncHandler<ParseNumberDto, string, ErrorDto>, AsyncValidationDecorator<ParseNumberDto, string>>()
    .After<IAsyncHandler<ParseNumberDto, string, ErrorDto>, ParseNumberAfterDecorator>();

DecoratorsPipeLine.FromHandlerType(typeof(GenerateStringAsyncHandler))
    .Before(typeof(AsyncValidationDecorator<,>))
    .After(typeof(GenerateStringToUpperAsyncDecorator<>));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();