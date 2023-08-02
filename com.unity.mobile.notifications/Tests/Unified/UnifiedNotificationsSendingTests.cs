using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Notifications;

class UnifiedNotificationsSendingTests
{
    [OneTimeSetUp]
    public void BeforeTests()
    {
        var args = NotificationCenterArgs.Default;
        args.AndroidChannelId = "Unified";
        args.AndroidChannelName = "Unified";
        args.AndroidChannelDescription = "Unified notifications";
        NotificationCenter.Initialize(args);
        NotificationCenter.OnNotificationReceived += OnNotificationReceived;
    }

    [SetUp]
    public void BeforeEachTest()
    {
        receivedNotificationCount = 0;
        lastNotification = null;
    }

    uint receivedNotificationCount = 0;
    Notification? lastNotification;
    void OnNotificationReceived(Notification notification)
    {
        ++receivedNotificationCount;
        lastNotification = notification;
    }

    IEnumerator WaitForNotification(float timeout)
    {
        var startCount = receivedNotificationCount;
        float timePassed = 0;
        while (receivedNotificationCount == startCount && timePassed < timeout)
        {
            yield return null;
            timePassed += Time.deltaTime;
        }
    }

    [UnityTest]
    [UnityPlatform(new[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer })]
    public IEnumerator SendNotificationAllOptionsRoundtrip()
    {
        var notification = new Notification()
        {
            Title = "Test",
            Text = "Testing",
            Data = "TestData",
            Badge = 2,
        };
        NotificationCenter.ScheduleNotification(notification, new NotificationIntervalSchedule(TimeSpan.FromSeconds(2)));

        yield return new WaitForSeconds(0.5f);  // test notification is trully delayed
        yield return WaitForNotification(60.0f);

        Assert.AreEqual(1, receivedNotificationCount);
        Assert.IsTrue(lastNotification.HasValue);
        var n = lastNotification.Value;
        Assert.AreEqual("Test", n.Title);
        Assert.AreEqual("Testing", n.Text);
        Assert.AreEqual("TestData", n.Data);
        Assert.AreEqual(2, n.Badge);
    }

    [UnityTest]
    [UnityPlatform(new[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer })]
    public IEnumerator ScheduleNotificationAtSpecificTime()
    {
        var notification = new Notification()
        {
            Title = "AtTime",
            Text = "AtSpecificTime",
        };
        NotificationCenter.ScheduleNotification(notification, new NotificationDateTimeSchedule(DateTime.Now.AddSeconds(2)));

        yield return new WaitForSeconds(0.5f);  // test notification is trully delayed
        yield return WaitForNotification(60.0f);

        Assert.AreEqual(1, receivedNotificationCount);
        Assert.IsTrue(lastNotification.HasValue);
        var n = lastNotification.Value;
        Assert.AreEqual("AtTime", n.Title);
        Assert.AreEqual("AtSpecificTime", n.Text);
    }
}
