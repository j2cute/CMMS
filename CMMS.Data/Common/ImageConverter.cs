using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Common
{
    public static class ImageConverter
    {
        public static string GetImageBase64(string imgPath)
        {
            string base64 = null;
            if (!string.IsNullOrEmpty(imgPath))
            {
                if (File.Exists(imgPath))
                {              
                    byte[] img = File.ReadAllBytes(imgPath);
                    base64 = Convert.ToBase64String(img, Base64FormattingOptions.None);
                    if (img != null)
                    {                
                        img = null;
                    }
                    base64 = string.Format("data:image/jpg;base64,{0}", base64);
                }
            }
            return base64;
        }
    }
}
