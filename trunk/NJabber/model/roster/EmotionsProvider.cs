using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJabber.Properties;

namespace NJabber.model.roster
{
    public class EmotionsProvider
    {
        private readonly IDictionary<string, string> maps = new Dictionary<string, string>();
        public  readonly static EmotionsProvider Instance = new EmotionsProvider();
        private readonly string basePath = Util.GetAppPath() + "/emotions/";
        public EmotionsProvider()
        {
            Load();
        }

        public void Reload()
        {
            Load();
        }
        private void Load()
        {
            maps.Clear();

            string text = Settings.Default.Emotions;

            foreach (string entry in text.Split("\r\n".ToCharArray()))
            {
                if (string.IsNullOrEmpty(entry)) continue;
                string[] entryvals = entry.Split(" ".ToCharArray(), 2);
                maps.Add(entryvals[0], entryvals[1]);
            }

        }

        public IDictionary<string, string> GetAll()
        {
            return maps;
        }
        public String GetEmotionPath(string code)
        {
            return (maps.ContainsKey(code)) ? basePath + maps[code] + ".gif" : null;
        }
    }

}
