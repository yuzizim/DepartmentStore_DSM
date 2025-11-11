using DepartmentStore.Client.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ====================
// 1. Add Services
// ====================
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation(); // Cho phép reload UI nhanh khi dev

builder.Services.AddHttpClient(); // Cho phép gọi API từ client

// Thêm session để lưu user info, roles
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Đã có 2 dòng này rồi:
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession();

// SỬA THÀNH DÒNG NÀY (có 2 tham số):
builder.Services.AddHttpClient<ApiClientService>((sp, client) =>
{
    client.BaseAddress = new Uri("https://localhost:7153/api/");
});

var app = builder.Build();

// ====================
// 2. Configure Middleware
// ====================

// Xử lý lỗi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kích hoạt Session (bắt buộc trước MapRazorPages)
app.UseSession();

// Cho phép dùng Authentication/Authorization nếu cần sau này
// app.UseAuthentication();
// app.UseAuthorization();

// ====================
// 3. Map Routes
// ====================

// Redirect root URL → /Account/Login
app.MapGet("/", async context =>
{
    context.Response.Redirect("/Account/Login");
});

// Razor Pages endpoints
app.MapRazorPages();

app.Run();
