using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test.api.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Description { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        public string Images { get; set; } = "[]";
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [StringLength(150)]
        public string CreatedBy { get; set; }
        public int CreatedById { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        [StringLength(150)]
        public string UpdatedBy { get; set; }
        public int UpdatedById { get; set; }
    }
}
