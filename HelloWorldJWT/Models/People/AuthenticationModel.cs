using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorldJWT.Models.People
{
    public class AuthenticationModel
    {
        //Used in POST for people/authenticate
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
