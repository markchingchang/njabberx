using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using agsXMPP;
using agsXMPP.net;
using agsXMPP.protocol.Base;
using NJabber.view;
using RosterItem = NJabber.model.roster.RosterItem;
using System.ComponentModel;
using MessageBox = System.Windows.MessageBox;

namespace NJabber
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly XmppClientConnection xmppCon;
        private readonly ListCollectionView view;
        private readonly ObservableCollection<RosterItem> rosterItems;
        private readonly Dictionary<string, string> presenceMap;
        private readonly Dictionary<string, MessageWindow> chatFormMap;
        private const string DefaultGroupName = "Other friends";

        public MainWindow()
        {

            InitializeComponent();
            rosterItems = new ObservableCollection<RosterItem>();
            view = new ListCollectionView(rosterItems);
            presenceMap = new Dictionary<string, string>();
            chatFormMap = new Dictionary<string, MessageWindow>();

            listbox.ItemsSource = view;


            xmppCon = new XmppClientConnection("chat.facebook.com")
                         {
                             SocketConnectionType = SocketConnectionType.Direct
                         };
            xmppCon.OnLogin += xmppCon_OnLogin;
            xmppCon.OnError += xmppCon_OnError;
            xmppCon.OnRosterStart += XmppConOnRosterStart;
            xmppCon.OnRosterItem += XmppConOnRosterItem;
            xmppCon.OnRosterEnd += XmppConOnRosterEnd;

            xmppCon.OnPresence += XmppConOnPresence;
            xmppCon.OnMessage += XmppConOnMessage;
            xmppCon.Open("bedanand", Hidden.GetPassword());

        }

        void xmppCon_OnError(object sender, Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        void XmppConOnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new MethodInvoker(() => XmppConOnMessage(sender, msg)));
                return;
            }
            MessageWindow messageWindow;
            if (chatFormMap.ContainsKey(msg.From.ToString()))
            {
                messageWindow = chatFormMap[msg.From.ToString()];
            }
            else
            {
                messageWindow = new MessageWindow(xmppCon, msg.From, msg.To);
                chatFormMap.Add(msg.From.ToString(), messageWindow);
            }
            messageWindow.Show();
            messageWindow.AddMessage(msg);
        }

        void XmppConOnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            if (!presenceMap.ContainsKey(pres.From.ToString()))
            {
                presenceMap.Add(pres.From.ToString(), pres.Show.ToString());
            }
            else
            {
                presenceMap[pres.From.ToString()] = pres.Show.ToString();
            }
            int i = rosterItems.IndexOf(new RosterItem() { JId = pres.From.ToString() });
            if (i >= 0)
            {


                rosterItems[i].Presence = pres.Show.ToString();

            }
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new MethodInvoker(Sort));
                return;
            }
            Sort();

        }

        private void Sort()
        {
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription { PropertyName = "GroupName", Direction = ListSortDirection.Ascending });
            view.SortDescriptions.Add(new SortDescription { PropertyName = "IsOnline", Direction = ListSortDirection.Descending });
        }

        void XmppConOnRosterEnd(object sender)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new MethodInvoker(() => XmppConOnRosterEnd(sender)));
                return;
            }

            view.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
            Sort();
        }

        void XmppConOnRosterItem(object sender, agsXMPP.protocol.iq.roster.RosterItem item)
        {
            if (!listbox.Dispatcher.CheckAccess())
            {
                listbox.Dispatcher.BeginInvoke(new MethodInvoker(() => XmppConOnRosterItem(sender, item)));
                return;
            }

            string nodeText = item.Name ?? item.Jid.ToString();
            string groupname;
            if (item.GetGroups().Count > 0)
            {
                var g = (Group)item.GetGroups().Item(0);
                groupname = g.Name;
            }
            else
            {
                groupname = DefaultGroupName;
            }
            var rosterItem = new RosterItem() { JId = item.Jid.ToString(), Name = nodeText, GroupName = groupname };

            if (presenceMap.ContainsKey(item.Jid.ToString()))
            {
                rosterItem.Presence = presenceMap[item.Jid.ToString()];
            }
            rosterItems.Add(rosterItem);

        }



        void XmppConOnRosterStart(object sender)
        {
            if (!listbox.Dispatcher.CheckAccess())
            {
                listbox.Dispatcher.BeginInvoke(new MethodInvoker(() => XmppConOnRosterStart(sender)));
                return;
            }
            rosterItems.Clear();
        }

        void xmppCon_OnLogin(object sender)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
