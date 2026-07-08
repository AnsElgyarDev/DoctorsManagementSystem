using DoctorsManagementSystem.Data;
using DoctorsManagementSystem.Endpoints;
using DoctorsManagementSystem.Middlewares;
using DoctorsManagementSystem.Service;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddScoped<IPatientServices, PatientServices>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin() 
              .AllowAnyMethod() 
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // to transfer the Endpoints
    app.MapScalarApiReference(); // to create the UI 
}

// Adding Middlewares

app.UseExceptionHandler();
app.UseMiddleware<RequestLogMiddleware>();  

app.UsePatientEndpoints();
app.Run();
