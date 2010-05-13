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
using agsXMPP.protocol.client;
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
        private readonly Dictionary<string, Presence> presenceMap;
        private readonly Dictionary<string, MessageWindow> chatFormMap;
        private const string DefaultGroupName = "Other friends";

        public MainWindow()
        {

            InitializeComponent();
            rosterItems = new ObservableCollection<RosterItem>();
            view = new ListCollectionView(rosterItems);
            presenceMap = new Dictionary<string, Presence>();
            chatFormMap = new Dictionary<string, MessageWindow>();

            listbox.ItemsSource = view;


            xmppCon = new XmppClientConnection
                          {
                              SocketConnectionType = SocketConnectionType.Direct,
                              Server = "127.0.0.1",
                              Username = "bedanand",
                              Password = "sharma",
                              Priority = 10,
                              
                          };

            xmppCon.OnReadXml += new XmlHandler(xmppCon_OnReadXml);
            xmppCon.OnSocketError += xmppCon_OnSocketError;
            xmppCon.OnLogin += xmppCon_OnLogin;
            xmppCon.OnError += xmppCon_OnError;
            xmppCon.OnRosterStart += XmppConOnRosterStart;
            xmppCon.OnRosterItem += XmppConOnRosterItem;
            xmppCon.OnRosterEnd += XmppConOnRosterEnd;
            xmppCon.OnStreamError += xmppCon_OnStreamError;
            xmppCon.OnAuthError += xmppCon_OnAuthError;
            xmppCon.OnXmppConnectionStateChanged += xmppCon_OnXmppConnectionStateChanged;
            xmppCon.OnPresence += XmppConOnPresence;
            xmppCon.OnMessage += XmppConOnMessage;
            xmppCon.Open();

        }

        void xmppCon_OnReadXml(object sender, string xml)
        {

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new MethodInvoker(() => xmppCon_OnReadXml(sender, xml)));
                return;
            }
            debug.Text += xml + "\n\r";

        }

       

        void xmppCon_OnXmppConnectionStateChanged(object sender, XmppConnectionState state)
        {
            
        }

        void xmppCon_OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            MessageBox.Show("login failed");
        }

        void xmppCon_OnStreamError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            MessageBox.Show("Stream error");
        }

        void xmppCon_OnSocketError(object sender, Exception ex)
        {
            MessageBox.Show(ex.Message);
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
            if (chatFormMap.ContainsKey(msg.From.User))
            {
                messageWindow = chatFormMap[msg.From.User];
            }
            else
            {
                messageWindow = new MessageWindow(xmppCon, msg.From, msg.To);
                chatFormMap.Add(msg.From.User, messageWindow);
            }
            messageWindow.Show();
            messageWindow.AddMessage(msg);
        }

        void XmppConOnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            if (!presenceMap.ContainsKey(pres.From.User))
            {
                presenceMap.Add(pres.From.User, pres);
            }
            else
            {
                presenceMap[pres.From.User] = pres;
            }
            int i = rosterItems.IndexOf(new RosterItem() { UserName = pres.From.User });
            if (i >= 0)
            {

                
                rosterItems[i].PreShow = pres.Show.ToString();
                rosterItems[i].PreType = pres.Type.ToString();
                rosterItems[i].PreStatus = pres.Status;

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
            var rosterItem = new RosterItem() { UserName = item.Jid.User, Name = nodeText, GroupName = groupname };

            
            if (presenceMap.ContainsKey(item.Jid.User))
            {
                var pre = presenceMap[item.Jid.User];
                rosterItem.PreShow = pre.Show.ToString();
                rosterItem.PreType = pre.Type.ToString();
                rosterItem.PreStatus = pre.Status;
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

       
    }
}
