using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Models
{
    public class UserCredential
    {
        [Key]
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}