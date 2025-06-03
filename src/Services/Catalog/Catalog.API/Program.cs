using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

//Add services to container.
builder.Services.AddCarter();              //For defining minimal api endpoints
builder.Services.AddMediatR(config =>      //for implementing mediator and CQRS design pattern
{
    config.RegisterServicesFromAssemblies(typeof(Program).Assembly);  
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));             //pipeline behavior for validation
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));                //pipeline behavior for logging
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);    //For database connectivity
}).UseLightweightSessions();     

if(builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();   //if development environment then seed the data. no seeding in prod env

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);  //To perform healthchecks of the API and backing services like cache and DB

var app = builder.Build();

//Configure http request pipeline
app.MapCarter();

app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse   //present the heaklthcheck results in a better format
    });

app.Run();
