using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObliKot2r2.Models
{
    public class GasMeasuringObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdGMO { get; set; }

        [Required]
        public int IdArea { get; set; }

        [Required]
        [StringLength(200)]
        public string NameGMO { get; set; } = string.Empty;

        [StringLength(40)]
        public string? IdGmoSAP { get; set; }

        [StringLength(20)]
        public string? IdAnother { get; set; }

        public byte[]? ObjGMO { get; set; }

        [ForeignKey("IdArea")]
        public Area? Area { get; set; }
        //public List<MeasuringPipe> MeasuringPipes { get; set; }
        public List<MeasuringPipe> MeasuringPipes { get; set; } = new();
    }
}
