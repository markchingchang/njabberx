using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using NJabber.model.roster;

namespace NJabber
{
   public  class Util
    {
       public static ImageWHash Base64ToImage(string base64String)
       {
           // Convert Base64 String to byte[]
           byte[] imageBytes = Convert.FromBase64String(base64String);
           var ms = new MemoryStream(imageBytes, 0,
             imageBytes.Length);

           // Convert byte[] to Image
           ms.Write(imageBytes, 0, imageBytes.Length);
           var image = new ImageWHash {Image = Image.FromStream(ms, true),Hash= ComputeHash(imageBytes)};
           return image;
       }
       public static string ImageToBase64(Image image,System.Drawing.Imaging.ImageFormat format)
       {
           using (var ms = new MemoryStream())
           {
               // Convert Image to byte[]
               image.Save(ms, format);
               byte[] imageBytes = ms.ToArray();

               // Convert byte[] to Base64 String
               string base64String = Convert.ToBase64String(imageBytes);
               return base64String;
           }
       }
       public static string ComputeHash(byte[] bytes)
       {
           SHA1 sha = SHA1.Create();
           byte[] hash = sha.ComputeHash(bytes);
           return HexToString(hash);
       }
       public static string HexToString(byte[] buf)
       {
           var sb = new StringBuilder();
           foreach (byte b in buf)
           {
               sb.Append(b.ToString("x2"));
           }
           return sb.ToString();
       }

       public static string GetAppPath()
       {
           return  Path.GetDirectoryName(Application.ExecutablePath);
           ;
       }
    }
}
