using Microsoft.Extensions.DependencyInjection;
using Polly.Registry;
using StoneBreakeven.Api.Extensions;
using StoneBreakeven.Api.Middlewares;
using StoneBreakeven.ExampleService;
using StoneBreakeven.ExampleService.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var exampleServiceSettings = builder.Configuration.GetSection(ExampleServiceSettings.SectionName).Get<ExampleServiceSettings>();
var registry = ResiliencePolicies.CreatePolicies(exampleServiceSettings!);
builder.Services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);

builder.Services.AddHttpClient<IExampleService, ExampleService>(client =>
    client.BaseAddress = new Uri(exampleServiceSettings!.BaseAddress))
        .AddPolicyHandlerFromRegistry(ExampleServiceSettings.SectionName);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.Run();