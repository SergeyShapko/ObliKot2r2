using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObliKot2r2.Models
{
    public class PointIO
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(20)]
        public string IdPointIOSAP { get; set; }

        [Required]
        public long IdGMO { get; set; }

        [Required]
        [MaxLength(200)]
        public string NamePointIO { get; set; }

        [MaxLength(16)]
        public string EIC { get; set; }

        [Required]
        public DateTime DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        [MaxLength(20)]
        public string IdAnother { get; set; }

        // 👉 это свойство — внешний ключ
        [Required]
        public int IdTypePointIO { get; set; }

        [ValidateNever]
        [ForeignKey(nameof(IdGMO))] // ⬅️ Привязываем вручную
        public virtual GasMeasuringObject GMO { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(IdTypePointIO))] // ⬅️ вот это ключевое!
        public virtual PointIOType PointIOType { get; set; }
    }
}