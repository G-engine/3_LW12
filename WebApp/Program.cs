using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using DBModel;
using WebApp.CRUD;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<EmployeeRepo>();
builder.Services.AddScoped<ProductRepo>();
builder.Services.AddScoped<SupplierRepo>();
builder.Services.AddSqlServer<PharmacyContext>("Server=(localdb)\\serv;Database=Pharmacy;Trusted_Connection=True;", providerOptions => { providerOptions.EnableRetryOnFailure(); });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


//app.UseAuthentication();
//app.UseAuthorization();

app.MapRazorPages();

app.Run();