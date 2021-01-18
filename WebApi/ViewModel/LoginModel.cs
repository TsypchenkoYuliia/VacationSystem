using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModel
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Login can't be empty")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
