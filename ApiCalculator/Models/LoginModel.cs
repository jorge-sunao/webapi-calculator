using System.ComponentModel.DataAnnotations;

namespace ApiCalculator.Models
{
    /// <summary>
    /// Login information
    /// </summary>
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
