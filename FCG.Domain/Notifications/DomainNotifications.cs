namespace FCG.Domain.Notifications
{
    public class DomainNotifications : DomainNotificationsBase
    {
        public DomainNotifications() { }

        public DomainNotifications(string notification)
        {
            Add(notification);
        }
    }
}
