using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using agsXMPP;
using agsXMPP.protocol.client;
using NJabber.controls;
using NJabber.model.roster;
using System.Windows.Media;


namespace NJabber.view
{
    
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        private readonly XmppClientConnection xmppCon;
        private readonly ObservableCollection<Message> messages = new ObservableCollection<Message>();
        private static readonly IDictionary<string, string> emotions = EmotionsProvider.Instance.GetAll();
        private readonly Jid to;
        private readonly Jid from;
        readonly FlowDocument rtbDocument = new FlowDocument();

        public MessageWindow(XmppClientConnection connection,Jid to,Jid from)
        {
            xmppCon = connection;
            this.to = to;
            this.from = from;
            InitializeComponent();
           
            foreach (string key in emotions.Keys )
            {
                var image = GetEmotionImage(key);
                image.MouseUp += ImageMouseUp;
                emotionPanel.Children.Add(image);
            }
            richTextbox.Document = rtbDocument;

            var message = new Message(from, to, MessageType.normal, "Of course :) in many publications :(, documents are not just ;) laid out in simple columns. =(( Often, other elements exist :)) that are taken out :-* of the regular flow. You have already seen such examples with the images placed inside documents. =))");
            messages.Add(message);
            AddMessageToDoc(message);

            var message2 = new Message(from, to, MessageType.normal, "Of course in many publications, documents are not just laid out in simple columns. Often, other elements exist that are taken out of the regular flow. You have already seen such examples with the images placed inside documents.=))");
            messages.Add(message2);
            AddMessageToDoc(message2);

            var message3 = new Message(from, to, MessageType.normal, ":)");
            messages.Add(message3);
            AddMessageToDoc(message3);
           
        }
        private static Image GetEmotionImage(string code)
        {
            var name = emotions[code];
            var image = new Image { ToolTip = name, Tag = code, Width = 30, Height = 18 };
            var source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new System.Uri(Util.GetAppPath() + "\\emotions\\" + name + ".gif");
            source.EndInit();
            image.Source = source;
            return image;
        }
        private static GifImage GetEmotionGifImage(string code)
        {
            var name = emotions[code];
            var image = new GifImage(new System.Uri(Util.GetAppPath() + "\\emotions\\" + name + ".gif")) { ToolTip = name, Tag = code };
            return image;
        }
        void ImageMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var button = (Image) sender;
            textBox1.Text += button.Tag;
        }

        
        private void AddMessageToDoc(Message message)
        {
          
            rtbDocument.Blocks.Add(ConvertMsgToBlock(message));
            rtbDocument.Blocks.Add(ComposeBody(message.Body));
            
        }

        private static Block ConvertMsgToBlock(Message message)
        {
            var boldText = message.From.User;
            var bold = new Bold();
            bold.Inlines.Add(boldText);
            var para = new Paragraph();
            para.Inlines.Add(bold);
            return para;
        }
        private static  Block ComposeBody(string body)
        {
            
            var para = new Paragraph();
            

            int cursor = 0;
            int last = 0;
            while(true)
            {
                int increment = 1;
                for(int j=3;j>1;j--)
                {
                    if (j + cursor <= body.Length)
                    {
                        var testdata = body.Substring(cursor, j);
                        if (emotions.ContainsKey(testdata))
                        {
                            var t = body.Substring(last, cursor-last);
                            para.Inlines.Add(t);
                            para.Inlines.Add(GetEmotionImage(testdata));
                            increment = j;
                            last = cursor + j;
                            break;
                        }
                    }
                }
                
                cursor+=increment;
                if (cursor == body.Length)
                {
                    if (cursor - last > 0)
                    {
                        para.Inlines.Add(body.Substring(last, cursor - last));
                    }
                    return para;
                }
            }
            
        }
       
        public void AddMessage(Message message)
        {
            messages.Add(message);
            if(message.Body!=null) AddMessageToDoc(message);
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
            AddMessageToDoc(message);
        }

        private void richTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            emotionPopup.IsOpen = true;
        }
    }
}
