using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObliKot2r2.Models
{
    //public class MeasuringPipe
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }

    //    [Required]
    //    [StringLength(50)]
    //    public string Name { get; set; }

    //    [StringLength(20)]
    //    public string AnotherId { get; set; }

    //    [StringLength(20)]
    //    public string SapId { get; set; }

    //    // Внешний ключ на GMO
    //    public int GmoId { get; set; }

    //    [ForeignKey(nameof(GmoId))]
    //    public GasMeasuringObject GMO { get; set; }

    //}
    public class MeasuringPipe
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long IdGMO { get; set; }

        [Required]
        [MaxLength(50)]
        public string NameMPipe { get; set; }

        [MaxLength(20)]
        public string IdAnother { get; set; }

        public byte[]? ObjMPipe { get; set; }

        [ForeignKey(nameof(IdGMO))]
        public virtual GasMeasuringObject GMO { get; set; }
    }

}
