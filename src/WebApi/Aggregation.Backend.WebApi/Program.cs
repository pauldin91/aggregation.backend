using Aggregation.Backend.Application.Extensions;
using Aggregation.Backend.Infrastructure.Data.Contexts;
using Aggregation.Backend.Infrastructure.Extensions;
using Aggregation.Backend.Infrastructure.Options;
using Aggregation.Backend.WebApi.Extensions;
using Aggregation.Backend.WebApi.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger());

builder.Services.AddApplicationExtensions();
builder.Services.AddInfrastructureExtensions(builder.Configuration);
builder.Services.AddWebApiExtensions(builder.Configuration);

var app = builder.Build();

var extIdOptions = new ExternalIdProviderOptions();
app.Configuration.Bind(nameof(ExternalIdProviderOptions), extIdOptions);

app.UseExceptionHandler("/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aggregation API v1");

    c.OAuthClientId(extIdOptions.ClientId);
    c.OAuthClientSecret(extIdOptions.ClientSecret);
    c.OAuth2RedirectUrl("https://localhost:7064/swagger/oauth2-redirect.html");
    //c.OAuthScopes("read:user", "user:email");
    c.OAuthUsePkce();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.UseOutputCache();

app.UseMiddleware<RequestAnalyticsMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

using var scope = app.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<AggregationBackendIdentityDbContext>();

db.Database.EnsureCreated();
await db.Database.MigrateAsync();

await app.RunAsync();