using System.ComponentModel.DataAnnotations;
namespace UserService.DTOs
{
    public class CreateUserRequestBody
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public string UserName { get; set; }
    }
}