using Polly.Registry;
using StoneBreakeven.Api.Extensions;
using StoneBreakeven.ExampleService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


const string registryKeyName = "example-service";

var registry = ResiliencePolicies.CreatePolicies(registryKeyName);
builder.Services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);

builder.Services.AddHttpClient<IExampleService, ExampleService>(client =>
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ExampleService:Uri")!))
        .AddPolicyHandlerFromRegistry(registryKeyName);

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