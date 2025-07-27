using System.ComponentModel.DataAnnotations;
using RotatingWorkSchedule.Enums;

namespace rotating_work_schedule.Models
{
   public class WorkDay
   {
      [Required]
      public DateOnly EffectiveDate { get; set; }

      [Required] // Campo obrigatório
      public DayOperating DayOperating { get; set; }
   }
}