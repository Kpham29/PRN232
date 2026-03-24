using LMS.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(opt => opt.IdleTimeout = TimeSpan.FromMinutes(60));
builder.Services.AddHttpClient("LmsApi", c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!));
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
