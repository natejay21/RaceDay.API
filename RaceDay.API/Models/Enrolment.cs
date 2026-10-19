using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentId { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")] public User User { get; set; } = null!;
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")] public Category Category { get; set; } = null!;
        [Required] public string Status { get; set; } = "Pending";
        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;
    }
}