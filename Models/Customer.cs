using System.ComponentModel.DataAnnotations;

namespace ATMManagementApplication.Models
{
    public class Customer{
        // Annotation => primary key
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        public decimal Balance { get; set; }

        // Thêm trường email
        [Required]
        [EmailAddress]  // Annotation để kiểm tra định dạng email hợp lệ
        public string Email { get; set; }
    }    
}
