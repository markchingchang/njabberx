using System;
using System.Collections.Generic;
using System.Text;

namespace MVCWork
{
    public  interface INotification
    {
        string Name { get; set; }
        object Body { get; set; }
    }
}
