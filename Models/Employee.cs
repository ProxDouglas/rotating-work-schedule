using System.ComponentModel.DataAnnotations;

namespace rotating_work_schedule.Models
{
   public class Employee
   {
      [Key] // PK
      public int Id { get; set; }

      [Required]
      [StringLength(100)]
      public required string Name { get; set; }

      public List<WorkDay> WorkOffs { get; set; } = new List<WorkDay>();

      public JobPosition? JobPosition { get; set; }

      public ICollection<Unavailability>? Unavailabilities { get; set; }
   }
}