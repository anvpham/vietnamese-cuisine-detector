using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodService.Models
{
    public class Food
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string EnglishName { get; set; }
        
        [Column(TypeName = "nvarchar(50)")]
        public string VietnameseName { get; set; }

        public string Description { get; set; }
    }
}