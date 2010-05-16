using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using agsXMPP.protocol.client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJabber.uitl;

namespace njabbertest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class PresenceUtilTest
    {
        private const string AvatarPres1 = "<presence xmlns='jabber:client' from='dave@127.0.0.1/Pandion' to='bedanand@127.0.0.1/agsXMPP'><x xmlns='jabber:x:avatar'><hash>add221a1fe148d0ef6532a770ecd8e5f56104cc1</hash></x><priority>8</priority></presence>";
        private const string AvatarHash1 = "add221a1fe148d0ef6532a770ecd8e5f56104cc1";
        private const string AvatarPrec2 = "";
        private const string VCardPres1 = "<presence xmlns='jabber:client' from='admin@127.0.0.1/Bedanand Sharma’s MacBook' to='bedanand@127.0.0.1/agsXMPP'><priority>0</priority><c xmlns='http://jabber.org/protocol/caps' ext='ice recauth rdserver maudio audio rdclient mvideo auxvideo rdmuxing avcap avavail video' ver='745' node='http://www.apple.com/ichat/caps' /><x xmlns='vcard-temp:x:update' /></presence>";
        private const string VCardHash1 = "";
        private const string VCardPres2 = "";


        public PresenceUtilTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        
        [TestMethod]
        public void TestIsAvatar()
        {
            Assert.IsTrue(PresenceUtil.IsAvatar(GetPresence(AvatarPres1)));
            Assert.IsFalse(PresenceUtil.IsAvatar(GetPresence(VCardPres1)));
        }

        public void TestIsVCard()
        {
            Assert.IsTrue(PresenceUtil.IsVCard(GetPresence( VCardPres1)));
            Assert.IsFalse(PresenceUtil.IsAvatar(GetPresence(AvatarPres1)));
        }



        [TestMethod]
        public void TestGetAvatarHash()
        {

            //case 1
            Assert.AreEqual(AvatarHash1, PresenceUtil.GetAvatarHash(GetPresence(AvatarPres1)));


            //case 2

        }

        private static Presence GetPresence(string xml)
        {
            var presence = new Presence {InnerXml = xml};
            return presence;
            
        }
    }
}
