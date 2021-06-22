using System.ComponentModel.DataAnnotations;
namespace UserService.Models
{
    public class Food
    {
        [Key]
        public string Name { get; set; }
    }
}