using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using agsXMPP;
using agsXMPP.net;
using agsXMPP.protocol.Base;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using NJabber.uitl;
using NJabber.view;
using RosterItem = NJabber.model.roster.RosterItem;
using System.ComponentModel;
using MessageBox = System.Windows.MessageBox;
using Stream=agsXMPP.protocol.Base.Stream;
using NJabber.model.roster;

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
        private const string DefaultGroupName = "Buddies";

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

            xmppCon.OnReadXml += XmppConOnReadXml;
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
            xmppCon.OnIq += XmppConOnIq;
            xmppCon.Open();
        }

        void XmppConOnIq(object sender, IQ iq)
        {
            if (iq.Type == IqType.get)
            {

            }
            else if (iq.Type == IqType.result)
            {
                ImageWHash image=null;
                if (iq.Query !=null && iq.Query.Namespace == "jabber:iq:avatar")
                {
                   image = Util.Base64ToImage(iq.Query.FirstChild.InnerXml); 
                }
                else
                {
                    Element vcard;
                    if ((vcard = iq.SelectSingleElement("vCard", "vcard-temp"))!=null)
                    {
                        Element photo = vcard.SelectSingleElement("PHOTO");
                        if(photo!=null)
                        {
                            image = Util.Base64ToImage(photo.SelectSingleElement("BINVAL").InnerXml);        
                        }
                        
                    }
                    
                }
                if(image== null) return;
                string filepath = "avt" + image.Hash + ".jpg";
                image.Image.Save(filepath);
                ImageCacheProvider.Instance.Add(image.Hash, filepath);
                var roster = GetRoster(iq.From.User);
                if (roster != null) roster.ImageSource = filepath;
            }
        }

        RosterItem GetRoster(string user)
        {
            int i = rosterItems.IndexOf(new RosterItem() { UserName = user });
            if (i >= 0)
            {
                return rosterItems[i];
            }
            return null;
        }


        void XmppConOnReadXml(object sender, string xml)
        {

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new MethodInvoker(() => XmppConOnReadXml(sender, xml)));
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
        
        void XmppConOnPresence(object sender, Presence pres)
        {
            if (!presenceMap.ContainsKey(pres.From.User))
            {
                presenceMap.Add(pres.From.User, pres);
            }
            else
            {
                presenceMap[pres.From.User] = pres;
            }
            SetPresence(pres);
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new MethodInvoker(Sort));
                return;
            }
            Sort();

        }
        private  void SetPresence(Presence pres)
        {
            int i = rosterItems.IndexOf(new RosterItem() { UserName = pres.From.User });
            if (i >= 0)
            {
                rosterItems[i].PreShow = pres.Show;
                rosterItems[i].PreType = pres.Type.ToString();
                rosterItems[i].PreStatus = pres.Status;

            }
            
            if(PresenceUtil.IsAvatar(pres))
            {
                string hash;
                if ((hash = PresenceUtil.GetAvatarHash(pres)) != null)
                {
                    string imagepath;
                    if ((imagepath = ImageCacheProvider.Instance.GetImagePath(hash)) != null)
                    {
                        if (i >= 0) rosterItems[i].ImageSource = imagepath;
                    }
                    else
                    {
                        var requestAvatar = new IQ(IqType.get, pres.To, pres.From);
                        requestAvatar.Query = new Element("query", "", "jabber:iq:avatar");
                        xmppCon.Send(requestAvatar);
                    }
                }
               
            }
            else if(PresenceUtil.IsVCard(pres))
            {
                string hash;
                if ((hash = PresenceUtil.GetVCardHash(pres)) != null)
                {
                    string imagepath;
                    if ((imagepath = ImageCacheProvider.Instance.GetImagePath(hash)) != null)
                    {
                        if (i >= 0) rosterItems[i].ImageSource = imagepath;
                    }
                    else
                    {
                        var requestAvatar = new IQ(IqType.get, pres.To, pres.From);
                        requestAvatar.Query = new Element("vCard", "", "vcard-temp");
                        xmppCon.Send(requestAvatar);
                    }
                }
            }

        }

        private void Sort()
        {
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription { PropertyName = "GroupName", Direction = ListSortDirection.Ascending });
            view.SortDescriptions.Add(new SortDescription { PropertyName = "IsOnline", Direction = ListSortDirection.Descending });
            view.SortDescriptions.Add(new SortDescription { PropertyName = "PreShow", Direction = ListSortDirection.Ascending });
            view.SortDescriptions.Add(new SortDescription { PropertyName = "Name", Direction = ListSortDirection.Ascending });
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

            rosterItems.Add(rosterItem);
            
            if (presenceMap.ContainsKey(item.Jid.User))
            {
                var pre = presenceMap[item.Jid.User];
                SetPresence(pre);
            }

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
