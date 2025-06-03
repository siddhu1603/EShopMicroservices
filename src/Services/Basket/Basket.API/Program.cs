using BuildingBlocks.Exceptions.Handler;
using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using BuildingBlocks.Messaging.MassTransit;

var builder = WebApplication.CreateBuilder(args);

//Add services to container


//Application Services - 
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>      //for implementing mediator and CQRS design pattern
{
    config.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});


//Data Services - 
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);    //For database connectivity
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);                  //in ShoppingCart model, the username field will be used as Identity/PK
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();              //registering the basketrepository

//register the Cachedbasket Proxy service using the scrutor library which uses the decorator pattern.
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    //options.InstanceName = "Basket";
});


//Grpc Services - 
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});


//Async Communication Services
builder.Services.AddMessageBroker(builder.Configuration); //not passing any assembly parameter here bcoz, we r in the publisher side nd we dont need to give any consumer details.

//Cross-Cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();                 //registers the custom ExceptionHandler imlementation we have written

builder.Services.AddHealthChecks()                            //Add healthchecks for the backing services like cache and DB
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!) 
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

//configure http request pipeline

app.MapCarter();
app.UseExceptionHandler(options => { });
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse    //better format to view the healthcheck results.
    });

app.Run();
