using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILS.UserManagement.Models
    {
    public class UserModel
    {
        //[Required]
        //[MaxLength(20, ErrorMessage = "Maximum Length Should be Equal to 20 Characters")]
        //[RegularExpression("^([a-zA-Z0-9]+)$", ErrorMessage = "Only Alphabets and Numbers allowed.")]

        public string UserId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(70, MinimumLength = 2)]
        public string Name { get; set; }
         

        [Required(ErrorMessage = "This field is required.")]
        [StringLength(13, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z0-9]+[-]?[0-9]+$", ErrorMessage = "Please Enter Valid Pattern")]
        public string Pno { get; set; }

        [DataType(DataType.PhoneNumber)]
        [MaxLength(11, ErrorMessage = "5 to 11 Digits only"), MinLength(5, ErrorMessage = "5 to 11 Digits only")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please Enter Digits only")]
        public string Contact { get; set; }

    
        [StringLength(100, ErrorMessage = "Invalid Password!", MinimumLength = 2)]
       // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Invalid Password!. Please check Password Policy")]
        //[DataType(DataType.Password)]
        public string Password { get; set; }

       // [DataType(DataType.Password)]
       // [Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    [MetadataType(typeof(UserModel))]
    public partial class tbl_User
    {
      public string ConfirmPassword { get; set; }
    }
}
