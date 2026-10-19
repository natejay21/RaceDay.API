using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required] public string CategoryName { get; set; } = string.Empty;
        [Required] public decimal DistanceKM { get; set; }
        [Required] public decimal EntryFee { get; set; }

        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public RaceEvent RaceEvent { get; set; } = null!;
    }
}