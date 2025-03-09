using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rotating_work_schedule.Models
{
   public class OperatingSchedule
   {
      [Key] // PK
      public int Id { get; set; }

      [Required] // Campo obrigatório
      public TimeSpan Start { get; set; }

      [Required] // Campo obrigatório
      public TimeSpan End { get; set; }

      [Required] // Campo obrigatório
      public DayOfWeek DayWeek { get; set; }

      public DateTime? Canceled { get; set; } // Pode ser nulo

      public ICollection<JobPosition> JobPosition { get; set; }
   }
}