using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace UserService.Models
{
    public class UserFood
    {
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }
        
        
        [ForeignKey("Food")]
        public string FoodName { get; set; }
        
        public Food Food { get; set; }
    }
}