using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace BookApiDapper.Models
{
    public class BookModel
    {
        [BindNever]
        public int Id { get; set; }

        [Required]
        
        public string Publisher { get; set; }

        [Required]
        
        public string Title { get; set; }

        [Required]
        
        public string AuthorLastName { get; set; }

        [Required]
        
        public string AuthorFirstName { get; set; }

        [Required]
       
        public decimal Price { get; set; }

        [BindNever]
        public Role role { get; set; }
        public enum Role
        {
            Admin = 1,
            User = 2
        }
    }
}
