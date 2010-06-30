using System.Collections.Generic;

namespace MVCWork
{
    public interface IMediator
    {
        string Name { get; }
        void HandleNotification(INotification notification);
        IList<string> ListNotificationInterests();
    }
}
