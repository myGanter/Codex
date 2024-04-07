using Codex.AspNetCore;
using Codex.Cache;
using Codex.CQRS;
using DemoWeb.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Codex infrastructure
builder.Services.AddCodex();

//Handlers & decorators
builder.Services.AddScoped<IAsyncHandler<ParseNumberDto, string, ErrorResult>, ParseNumberAsyncHandler>();
builder.Services.AddDecorator(typeof(ValidationAsyncDecorator<,>));
builder.Services.AddDecorator(typeof(ParseNumberAfterDecorator));

//Configure pipeline
DecoratorsPipeLine.FromAsyncHandler<ParseNumberDto, string, ErrorResult>()
    .Before<IAsyncHandler<ParseNumberDto, string, ErrorResult>, ValidationAsyncDecorator<ParseNumberDto, string>>()
    .After<IAsyncHandler<ParseNumberDto, string, ErrorResult>, ParseNumberAfterDecorator>();

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