namespace LibraryManagementSystem.Models
{
    public class NotificationMessage
    {
        public string Email { get; set; }         // 收件人邮箱
        public string Subject { get; set; }   // 邮件主题
        public string Body { get; set; }      // 邮件内容
    }

}
