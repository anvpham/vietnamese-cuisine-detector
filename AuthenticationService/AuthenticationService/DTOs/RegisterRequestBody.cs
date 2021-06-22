using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.DTOs
{
    public class RegisterRequestBody
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}