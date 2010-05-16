using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;

namespace NJabber.uitl
{
    public class PresenceUtil
    {
        public static string GetAvatarHash(Presence presence)
        {
            Element node;
            if ((node=presence.SelectSingleElement("x", "jabber:x:avatar"))!=null)
            {
                Element hashNode;
                if ((hashNode = node.SelectSingleElement("hash", "jabber:x:avatar")) != null) return hashNode.InnerXml;
            }
            return null;
        }
        public static bool IsAvatar(Presence presence)
        {
            if ((presence.SelectSingleElement("x", "jabber:x:avatar")) != null) return true;
            return false;
        }
        public static bool IsVCard(Presence presence)
        {
            if ((presence.SelectSingleElement("x", "vcard-temp:x:update")) != null) return true;
            return false;
        }
        
        public static string GetVCardHash(Presence presence)
        {
            Element node;
            if ((node = presence.SelectSingleElement("x", "vcard-temp:x:update")) != null)
            {
                Element hashNode;
                if ((hashNode = node.SelectSingleElement("photo")) != null) return hashNode.InnerXml;
            }
            return null;
        }
    }
}
