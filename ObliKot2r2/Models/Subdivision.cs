using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObliKot2r2.Models
{
    public class Subdivision
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSubdivision { get; set; }

        [Required]
        public int IdPhilia { get; set; }

        [Required]
        [StringLength(200)]
        public string NameSubdivision { get; set; } = "";

        [StringLength(20)]
        public string? IdSubdivisionSAP { get; set; }

        [StringLength(20)]
        public string? IdAnother { get; set; }

        public byte[]? ObjSubdivision { get; set; }

        [ForeignKey("IdPhilia")]
        public Philia? Philia { get; set; }

        public List<Area> Areas { get; set; } = new();

        public string FullName => $"{Philia?.NamePhilia} → {NameSubdivision}";
    }
}
