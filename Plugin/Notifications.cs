using GLib;

namespace LudicoGTK.Plugin.Modules;

public class Notifications
{
    private static readonly Application App = AppGlobals.appInstance;
    
    public static void Push(string title, string text)
    {
        var notification = new Notification(title);
        notification.Body = text;
        
        App.SendNotification("ludico-notification", notification);
    }

    public static void PushSuccess(string title, string text)
    {
        var notification = new Notification(title);
        notification.Icon = new ThemedIcon("emblem-success");
        notification.Body = text;
        
        App.SendNotification("ludico-notification-success", notification);
    }
    
    public static void PushError(string title, string text)
    {
        var notification = new Notification(title);
        notification.Icon = new ThemedIcon("emblem-error");
        notification.Body = text;
        
        App.SendNotification("ludico-notification-success", notification);
    }
    
    public static void PushWarning(string title, string text)
    {
        var notification = new Notification(title);
        notification.Icon = new ThemedIcon("emblem-warning");
        notification.Body = text;
        
        App.SendNotification("ludico-notification-success", notification);
    }
}