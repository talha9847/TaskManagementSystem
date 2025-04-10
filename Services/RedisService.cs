using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApp.Namespace;

public class RedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisService()
    {
        try
        {
            _redis = ConnectionMultiplexer.Connect("localhost:6379"); // Update if needed
            _db = _redis.GetDatabase();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis Connection Error: {ex.Message}");
            throw;
        }
    }

    public void SetNotification(Notification notification)
    {
        _db.ListLeftPush("notification", notification.ToString());
    }

    public List<Notification> GetNotifications()
    {
        var notifications = _db.ListRange("notification");
        return notifications.Select(n => Notification.FromString(n)).ToList();
    }

    public void RemoveNotification(string notificationId)
    {
        var notifications = _db.ListRange("notification");

        Console.WriteLine($"🔍 Looking for ID: {notificationId}");
        Console.WriteLine("📌 Stored notifications in Redis:");

        foreach (var notification in notifications)
        {
            Console.WriteLine($"➡ {notification}"); // Print each stored value
        }

        foreach (var notification in notifications)
        {
            string notifString = notification.ToString().Trim();
            string storedId = notifString.Split('|')[0]; // Extract ID

            if (storedId == notificationId) 
            {
                _db.ListRemove("notification", notification); 
                Console.WriteLine($"removeddd {notifString}");
                return;
            }
        }

        Console.WriteLine($" {notificationId}");
    }
}
