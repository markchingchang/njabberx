using System;
using System.Collections.Generic;
using System.Text;

namespace MVCWork
{
    public class Notification:INotification
    {
        public string Name
        {
            get; set;
        }

        public object Body
        {
            get; set;
        }
    }
}
