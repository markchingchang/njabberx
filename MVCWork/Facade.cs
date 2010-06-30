using System;
using System.Collections.Generic;

namespace MVCWork
{
    public class Facade
    {
        public IDictionary<String,IMediator> Mediators= new Dictionary<string, IMediator>();

        private static Facade intance;
        public static Facade Intance
        {
            get
            {
                if(intance==null)intance= new Facade();
                return intance;
            }
           
        }

        public void RegisterMediator(IMediator mediator)
        {
           if(!Mediators.ContainsKey(mediator.Name)) Mediators.Add(mediator.Name, mediator);
        }
        public void RemoveMediator(string name)
        {
           if(Mediators.ContainsKey(name)) Mediators.Remove(name);
        }
        public void SendNotification(string notification, object body)
        {
            foreach(IMediator mediator in Mediators.Values)
            {
                 if(mediator.ListNotificationInterests().Contains(notification))
                 {
                     mediator.HandleNotification( new Notification(){Name = notification,Body = body});
                 }
            }
        }
       
    }
}
