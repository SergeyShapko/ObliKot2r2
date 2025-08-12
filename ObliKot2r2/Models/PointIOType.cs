using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObliKot2r2.Models
{
    public class PointIOType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTypePointIO { get; set; }

        [Required]
        [MaxLength(100)]
        public string NameTypePointIO { get; set; }

        public virtual ICollection<PointIO> PointIOs { get; set; }
    }
}