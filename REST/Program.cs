using DBModel;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using WebApp.CRUD;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddScoped<EmployeeRepo>();
builder.Services.AddScoped<SupplierRepo>();
builder.Services.AddScoped<ProductRepo>();
builder.Services.AddDbContext<PharmacyContext>(opt =>
    opt.UseSqlServer("Server=(localdb)\\serv;Database=Pharmacy;Trusted_Connection=True;", providerOptions => { providerOptions.EnableRetryOnFailure(); }));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();

app.Run();