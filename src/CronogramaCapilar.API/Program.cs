using CronogramaCapilar.API;
using CronogramaCapilar.API.Extensions;
using CronogramaCapilar.Application;
using CronogramaCapilar.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Cada camada registra os próprios serviços.
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationsAsync();

    app.MapOpenApi().AllowAnonymous();

    // Swagger UI em /swagger, lendo o documento gerado pelo .NET em /openapi/v1.json
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Cronograma Capilar v1");
        options.EnablePersistAuthorization();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

await app.RunAsync();
