using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        [ForeignKey("EnrolmentId")] public Enrolment Enrolment { get; set; } = null!;
        [Required] public TimeSpan FinishTime { get; set; }
        public int? OverallPosition { get; set; }
        public int? CategoryPosition { get; set; }
    }
}