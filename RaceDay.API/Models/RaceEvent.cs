using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class RaceEvent
    {
        [Key]
        public int EventId { get; set; }
        [Required] public string EventName { get; set; } = string.Empty;
        [Required] public DateTime EventDate { get; set; }
        [Required] public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required] public string EventType { get; set; } = string.Empty;

        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}