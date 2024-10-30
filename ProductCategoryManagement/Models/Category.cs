using System.ComponentModel.DataAnnotations;

namespace ProductCategoryManagement.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; } // Primary key for category table

        [Required(ErrorMessage = "Category Name is required.")]
        public string? CategoryName { get; set; }
    }
}
