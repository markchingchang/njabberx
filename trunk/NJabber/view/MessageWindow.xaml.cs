using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using agsXMPP;
using agsXMPP.protocol.client;

namespace NJabber.view
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        private readonly XmppClientConnection xmppCon;
        private readonly ObservableCollection<Message> messages = new ObservableCollection<Message>();
        private readonly Jid to;
        private readonly Jid from;

        public MessageWindow(XmppClientConnection connection,Jid to,Jid from)
        {
            xmppCon = connection;
            this.to = to;
            this.from = from;
            InitializeComponent();
            listBox1.ItemsSource = messages;

        }
       
        public void AddMessage(Message message)
        {
            messages.Add(message);
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message(to, from, MessageType.normal, textBox1.Text);
            
            messages.Add(message);
            xmppCon.Send(message);
        }
    }
}
