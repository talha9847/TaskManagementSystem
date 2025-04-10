using System;
using RabbitMQ.Client;
namespace MyApp.Namespace
{
   
    public class NotificationService
    {
        private readonly RedisService _redisService;
        private readonly RabbitMQService _rabbitMQService;

        public NotificationService()
        {
            _redisService = new RedisService();
            _rabbitMQService = new RabbitMQService();
        }

        public void HandleTaskEvent(string action, string taskId, string taskTitle)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                Action = action,
                Message = taskTitle,
                Timestamp = DateTime.Now
            };

            _redisService.SetNotification(notification);

            _rabbitMQService.SendNotificationEvent(notification);
        }
    }
}


