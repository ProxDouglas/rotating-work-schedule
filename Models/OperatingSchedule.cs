using System.ComponentModel.DataAnnotations;
using RotatingWorkSchedule.Enums;

namespace rotating_work_schedule.Models
{
   public class OperatingSchedule
   {
      [Required] // Campo obrigatório
      public TimeSpan Start { get; set; }

      [Required] // Campo obrigatório
      public TimeSpan End { get; set; }

      [Required] // Campo obrigatório
      public DayOperating DayOperating { get; set; }
   }
}