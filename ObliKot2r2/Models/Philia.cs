using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObliKot2r2.Models
{
    public class Philia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPhilia { get; set; }

        [Required]
        [StringLength(100)]
        public string NamePhilia { get; set; }

        [StringLength(20)]
        public string? IdPhiliaSAP { get; set; }

        [StringLength(20)]
        public string? IdAnother { get; set; }

        public byte[]? ObjPhilia { get; set; }

        public List<Subdivision> Subdivisions { get; set; } = new();
    }
}
