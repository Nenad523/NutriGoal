using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NutriGoal.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Potvrda lozinke je obavezna")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}