using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// �t�m��Ʈw�W�U��
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDB")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login"; // �]�w�n�J�������|
        options.LogoutPath = "/"; // �]�w�n�X�������|
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // �]�w Session �L���ɶ�
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// �K�[ MVC ���
builder.Services.AddControllersWithViews();

builder.Services.AddControllers();


// �t�m EmailService
builder.Services.AddSingleton<EmailService>();

// �t�m RabbitMQ 
builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddHostedService<NotificationScheduler>();
builder.Services.AddHostedService<RabbitMqConsumer>();

// �t�m���
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});



var app = builder.Build();

// ������t�m
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession(); // �ҥ� Session
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // �ҥΨ�������
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


