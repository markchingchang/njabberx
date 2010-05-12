using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace NJabber.model.roster
{
    public class RosterItem:INotifyPropertyChanged
    {
        public RosterItem()
        {
            
        }

        public string JId { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string GroupName { get; set; }
        private string presence;
        public string Presence
        {
            get { return presence; }
            set { presence = value;
                InvokePropertyChanged("Presence");
                InvokePropertyChanged("IsOnline");
                InvokePropertyChanged("Online");
                InvokePropertyChanged("StatusBrush");
            }
        }

        public bool IsOnline
        {
            get
            {
                return presence != null;
            }
        }
      
        public override int GetHashCode()
        {
            return JId.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var objItem = obj as RosterItem;
            if(objItem== null) return false;
            return JId.Equals(objItem.JId);
        }
        public Brush StatusBrush
        {
            get {
                return IsOnline ? Brushes.Green : Brushes.Gray;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(String name)
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed != null) changed(this,new PropertyChangedEventArgs(name));
        }
    }
}
