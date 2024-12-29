using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class NotificationScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqPublisher _publisher;
    private readonly ILogger<NotificationScheduler> _logger;

    public NotificationScheduler(IServiceScopeFactory scopeFactory, RabbitMqPublisher publisher, ILogger<NotificationScheduler> logger)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                    // 确保包含导航属性 User 和 Book
                    var dueTransactions = context.Transactions
       .Include(t => t.User) // 加载 User
       .Include(t => t.Book) // 加载 Book
       .Where(t => t.DueDate > DateTime.Now && // 到期日期在未来
                   t.DueDate <= DateTime.Now.AddDays(3) && // 离到期日期不超过 3 天
                   t.ReturnedDate == null && // 尚未归还
                   t.User != null &&
                   !string.IsNullOrEmpty(t.User.Email))
       .ToList();

                    foreach (var transaction in dueTransactions)
                    {
                        var message = new NotificationMessage
                        {
                            Email = transaction.User.Email,
                            Subject = "借閱到期通知",
                            Body = $"親愛的用户，您借閱的書籍《{transaction.Book.Title}》即將於 {transaction.DueDate:yyyy-MM-dd} 到期，請及時歸還。"
                        };

                        _publisher.Publish(message);
                        _logger.LogInformation($"通知消息已推送: {message.Subject}");
                    }

                    var overdueTransactions = context.Transactions
       .Include(t => t.User) // 加载 User
       .Include(t => t.Book) // 加载 Book
       .Where(t => t.DueDate <= DateTime.Now && // 到期日期已过
                   t.ReturnedDate == null && // 尚未归还
                   t.User != null &&
                   !string.IsNullOrEmpty(t.User.Email))
       .ToList();

                    foreach (var transaction in overdueTransactions)
                    {
                        var message = new NotificationMessage
                        {
                            Email = transaction.User.Email,
                            Subject = "借阅逾期提醒",
                            Body = $"亲爱的用户，您借阅的书籍《{transaction.Book.Title}》已于 {transaction.DueDate:yyyy-MM-dd} 到期，请尽快归还以免产生罚金。"
                        };

                        _publisher.Publish(message);
                        _logger.LogInformation($"即期通知已推送: {message.Subject}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"调度系统运行错误: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
