using System.ComponentModel.DataAnnotations;

namespace WorkSchedule.Entities;

public class Employee
{
   [Required]
   [StringLength(100)]
   public required string Name { get; set; }

   public List<WorkDay> WorkOffs { get; set; } = new List<WorkDay>();

   public JobPosition? JobPosition { get; set; }

   public ICollection<Unavailability>? Unavailabilities { get; set; }
}
