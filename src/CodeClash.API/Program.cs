using CodeClash.API;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApiServices()
    .AddObservability();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
