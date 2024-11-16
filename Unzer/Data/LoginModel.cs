using System;
using System.ComponentModel.DataAnnotations;

namespace Unzer.Data
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}

