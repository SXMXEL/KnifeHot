using System;
using Assets.SimpleAndroidNotifications;
using UnityEngine;

namespace Managers
{
    public class NotificationsManager
    {
        public void ScheduleNotification(DateTime fireDateTime)
        {
            if (fireDateTime < DateTime.Now)
            {
                return;
            }
            
            var notificationParams = new NotificationParams
            {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = TimeSpan.FromSeconds((fireDateTime - DateTime.Now).TotalSeconds),
                Title = "Knife Hot",
                Message = "It's time to throw some knives? 😉",
                Ticker = "It's time to throw some knives? 😉",
                Sound = true,
                Vibrate = false,
                Light = false,
                SmallIcon = NotificationIcon.Clock,
                SmallIconColor = Color.white,
                LargeIcon = "app_icon"
            };
            
            if (!Application.isEditor)
            {
                NotificationManager.SendCustom(notificationParams);
            }
            
        }
    }
}
