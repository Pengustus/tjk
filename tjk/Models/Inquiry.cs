using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tjk.Models
{
    [Table("Inquiries")]
    public class Inquiry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Company { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? SelectedProduct { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string Status { get; set; } = "Ново";

        [MaxLength(100)]
        public string? IpAddress { get; set; }

        [MaxLength(1000)]
        public string? AdminNotes { get; set; }
    }
}
