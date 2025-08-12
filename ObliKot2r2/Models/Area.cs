using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObliKot2r2.Models
{
    public class Area
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdArea { get; set; }

        [Required]
        public int IdSubdivision { get; set; }

        [Required]
        [StringLength(200)]
        public string NameArea { get; set; } = "";

        [StringLength(20)]
        public string? IdAreaSAP { get; set; }

        [StringLength(20)]
        public string? IdAnother { get; set; }

        public byte[]? ObjArea { get; set; }

        [ForeignKey("IdSubdivision")]
        public Subdivision? Subdivision { get; set; }

        public string FullName => $"{Subdivision?.Philia?.NamePhilia} → {Subdivision?.NameSubdivision} → {NameArea}";
        public string FullPath =>
    $"{Subdivision?.Philia?.NamePhilia} → {Subdivision?.NameSubdivision} → {NameArea}";

    }
}
