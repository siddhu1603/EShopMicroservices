using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10); //means within 10 seconds we can send a max of 5 requests. further requests will throw 503 service unavailable error.
        options.PermitLimit = 5;
    });
});

var app = builder.Build();

//configure http request pipeline.
app.UseRateLimiter(); //apply rate limiting to all requests

app.MapReverseProxy();

app.Run();
