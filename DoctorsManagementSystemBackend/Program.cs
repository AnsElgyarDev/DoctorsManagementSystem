using DoctorsManagementSystem.Data;
using DoctorsManagementSystem.Dto;
using DoctorsManagementSystem.Endpoints;
using DoctorsManagementSystem.Middlewares;
using DoctorsManagementSystem.model;
using DoctorsManagementSystem.Service;
using Mapster;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IDashboardServices, DashboardServices>();   
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddScoped<IPatientServices, PatientServices>();
builder.Services.AddScoped<IAppointmentServices, AppointmentServices>();

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
app.UseDashboardEndpoints();
app.UseAppointmentEndpoints();
app.Run();
