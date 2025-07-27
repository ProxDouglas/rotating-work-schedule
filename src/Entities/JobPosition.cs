using System.ComponentModel.DataAnnotations;

namespace WorkSchedule.Entities;

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
