// Create a new WebApplication builder by calling WebApplication.CreateBuilder(args)
// 'args' is the command-line arguments passed to the application
using SK_API.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
      policy  =>
      {
          policy.WithOrigins("http://localhost:3000",
                              "https://staging.polyglot-edu.com",
                              "https://polyglot-edu.com");
      });
});

// Add services to the container.
// The 'Services' property of the builder is an instance of IServiceCollection

builder.Services.AddTransient<Auth>();
// 'AddControllers()' method registers the controllers in the application's dependency injection container
builder.Services.AddControllers();

// Add API Explorer services to the container.
// This enables the generation of OpenAPI (Swagger) documentation for the API endpoints
// 'AddEndpointsApiExplorer()' adds the necessary services for API documentation
builder.Services.AddEndpointsApiExplorer();

// Add Swagger services to the container.
// 'AddSwaggerGen()' configures the generation of the Swagger JSON document
// This document describes the API and its endpoints
builder.Services.AddSwaggerGen();

// Load secret variables from the secrest.json file
builder.Configuration.AddEnvironmentVariables(); //AddJsonFile("secrets.json", optional: true, reloadOnChange: true);
// Build the WebApplication instance based on the services and configuration defined in the builder
var app = builder.Build();

// Configure the HTTP request pipeline.
// The HTTP request pipeline is a series of middleware components that process incoming HTTP requests and generate HTTP responses

// TODO: fix this, enable swagger for testing even in production
// Check if the application is running in the Development environment
// if (app.Environment.IsDevelopment())
// {
// If the application is in Development environment, enable Swagger and SwaggerUI

// Add the Swagger JSON endpoint and Swagger UI to the request pipeline
app.UseSwagger();

// Configures the Swagger UI to display the Swagger JSON document in a web-based UI
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SK_API V1");
});
// }

// Redirect HTTP requests to HTTPS if the UseHttpsRedirection middleware is enabled
app.UseHttpsRedirection();

// add cors
app.UseCors();

// Add authorization middleware to the pipeline.
// This middleware checks if the incoming request is authorized to access the requested resources
app.UseAuthorization();

// Map the controllers to the request pipeline.
// 'MapControllers()' maps the ASP.NET Core controller routes to the request pipeline
app.MapControllers();

// The final middleware, 'app.Run()', is responsible for handling the request and generating the response
// It will be reached if none of the previous middleware components generate a response
// This can be used to add custom request handling logic if needed
app.Run();
