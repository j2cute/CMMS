using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClassLibrary.Common
{
    public class ValidateFileAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int MaxContentLength = 1024 * 1024 * 5; //5 MB
            string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };

            var files = value as IEnumerable< HttpPostedFileBase>;

            if (((HttpPostedFileBase[])files)[0] == null)
            {
                return true;
            }
            foreach (var i in files)
            {
                if (!AllowedFileExtensions.Contains(i.FileName.Substring(i.FileName.LastIndexOf('.'))))
                {
                    ErrorMessage = "Please upload, photo of type: " + string.Join(", ", AllowedFileExtensions);
                    return false;
                }
                else if (i.ContentLength > MaxContentLength)
                {
                    ErrorMessage = "Your uploaded photos, exceeds maximum limit, maximum allowed size is : " + (MaxContentLength / 1024).ToString() + "MB";
                    return false;
                }
            }
            return true;
        }
    }
}
