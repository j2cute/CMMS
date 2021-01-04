using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClassLibrary.Common
{
    public class FileHelper
    {
       // [Display(Name = "Upload Images")]
     //   [ValidateFile]
        public HttpPostedFileBase File { get; set; }
    }
}

