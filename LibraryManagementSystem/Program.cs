using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 配置資料庫上下文
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDB")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login"; // 設定登入頁面路徑
        options.LogoutPath = "/"; // 設定登出頁面路徑
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 設定 Session 過期時間
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 添加 MVC 支持
builder.Services.AddControllersWithViews();

builder.Services.AddControllers();


// 配置 EmailService
builder.Services.AddSingleton<EmailService>();

// 配置 RabbitMQ 
builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddHostedService<NotificationScheduler>();
builder.Services.AddHostedService<RabbitMqConsumer>();

// 配置日志
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});



var app = builder.Build();

// 中間件配置
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession(); // 啟用 Session
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 啟用身份驗證
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


