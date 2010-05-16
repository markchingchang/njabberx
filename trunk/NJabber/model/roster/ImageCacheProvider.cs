using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NJabber.model.roster
{
    public class ImageCacheProvider
    {
        private readonly IDictionary<string, string> maps = new Dictionary<string, string>();
        public static readonly ImageCacheProvider Instance = new ImageCacheProvider();
        public ImageCacheProvider()
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
            string text;

            string appPath = Util.GetAppPath();
            var reader = new StreamReader(string.Format("{0}\\imagecache.txt", appPath));
            while (!string.IsNullOrEmpty(text = reader.ReadLine()))
            {
                var message = text.Split(":".ToCharArray());
                maps.Add(message[0], message[1]);
            }
            reader.Close();
        }
        public void Add(string hash, string path)
        {
            if(maps.ContainsKey(hash)) return;
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            var writer = new StreamWriter(string.Format("{0}\\imagecache.txt", appPath));
            writer.WriteLine(hash + ":" + path);
            writer.Close();
            maps.Add(hash,path);
        }
        public IDictionary<string, string> GetAll()
        {
            return maps;
        }
        public String GetImagePath(string hash)
        {
            return (maps.ContainsKey(hash)) ? maps[hash] : null;
        }
    }
}


