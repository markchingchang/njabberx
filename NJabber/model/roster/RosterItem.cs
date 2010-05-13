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

        public string UserName { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string GroupName { get; set; }
        private string preShow;
        public string PreShow
        {
            get { return preShow; }
            set
            {
                preShow = value;
                InvokePropertyChanged("PreShow");
                InvokePropertyChanged("StatusBrush");
                InvokePropertyChanged("DisplayStatus");
            }
        }

        private string preStatus;

        public string PreStatus
        {
            get { return preStatus; }
            set { preStatus = value;
            InvokePropertyChanged("PreStatus");
            InvokePropertyChanged("DisplayStatus");
            }
        }
        public string DisplayStatus
        {
            get
            {
                return preShow + " " + preStatus;
            }
        }

        private string preType;

        public string PreType
        {
            get { return preType; }
            set { preType = value;
                InvokePropertyChanged("IsOnline");
                InvokePropertyChanged("StatusBrush");
                InvokePropertyChanged("DisplayStatus");
            }
        }

        public bool IsOnline
        {
            get
            {
                return PreType == "available";
            }
        }
      
        public override int GetHashCode()
        {
            return UserName.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var objItem = obj as RosterItem;
            if(objItem== null) return false;
            return UserName.Equals(objItem.UserName);
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
