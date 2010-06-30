using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJabber.view
{
    public interface IDisplay
    {
         object DataContext { get; set; }
    }
}
