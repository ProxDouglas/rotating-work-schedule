using System.ComponentModel.DataAnnotations;
using RotatingWorkSchedule.Enums;

namespace rotating_work_schedule.Models
{
   public class WorkDay
   {
      [Key] // PK
      public int Id { get; set; }

      [Required]
      public DateTime EffectiveDate { get; set; }

      [Required] // Campo obrigatório
      public DayOperating DayOperating { get; set; }

      public required OperatingSchedule OperatingSchedule { get; set; } // FK
   }
}