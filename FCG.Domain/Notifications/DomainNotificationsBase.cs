namespace FCG.Domain.Notifications
{
    public abstract class DomainNotificationsBase
    {
        private readonly List<string> _notifications = new List<string>();

        public List<string> Notifications => _notifications;

        public bool HasNotifications => _notifications.Any();

        public virtual bool HasResult => false;

        public void Add(string notification)
        {
            _notifications.Add(notification);
        }

        public void AddRange(IEnumerable<string> notifications)
        {
            _notifications.AddRange(notifications);
        }
    }
}
