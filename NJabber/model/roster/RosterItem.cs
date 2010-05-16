using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using agsXMPP.protocol.client;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace NJabber.model.roster
{
    public class RosterItem : INotifyPropertyChanged
    {
        public RosterItem()
        {
            image = "avatar.jpg";
        }

        private String image;
        public String ImageSource
        {
            get { return Util.GetAppPath() + "\\" +  image; }
            set
            {
                image = value;
                InvokePropertyChanged("ImageSource");
            }
        }

        public string UserName { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string GroupName { get; set; }
        private ShowType preShow;
        public ShowType PreShow
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
            set
            {
                preStatus = value;
                InvokePropertyChanged("PreStatus");
                InvokePropertyChanged("DisplayStatus");
            }
        }
        public string DisplayStatus
        {
            get
            {
                if (IsOnline) return ShowTypeProvider.Instance.Get(preShow.ToString()) + (!string.IsNullOrEmpty(preStatus) ? " - " + preStatus : null);
                return null;
            }
        }

        private string preType;

        public string PreType
        {
            get { return preType; }
            set
            {
                preType = value;
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
            if (objItem == null) return false;
            return UserName.Equals(objItem.UserName);
        }
        public Brush StatusBrush
        {
            get
            {
                if (IsOnline)
                {
                    switch (PreShow)
                    {
                        case ShowType.NONE:
                            return Brushes.GreenYellow;
                        case ShowType.dnd:
                            return Brushes.Red;
                        case ShowType.away:
                            return Brushes.Yellow;
                        case ShowType.xa:
                            return Brushes.Yellow;
                        default:
                            return Brushes.GreenYellow;

                    }
                }
                return Brushes.Gray;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(String name)
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed != null) changed(this, new PropertyChangedEventArgs(name));
        }
    }
}
