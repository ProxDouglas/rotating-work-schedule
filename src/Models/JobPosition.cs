using System.ComponentModel.DataAnnotations;

namespace rotating_work_schedule.Models
{
   public class JobPosition
   {
      [Required]
      [StringLength(100)]
      public required string Name { get; set; }

      [Required]
      public int Workload { get; set; }

      [Required]
      public int MaximumConsecutiveDays { get; set; }
   }
}