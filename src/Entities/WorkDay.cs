using System.ComponentModel.DataAnnotations;
using RotatingWorkSchedule.Enums;

namespace WorkSchedule.Entities;

public class WorkDay
{
   [Required]
   public DateOnly EffectiveDate { get; set; }

   [Required] // Campo obrigatório
   public DayOperating DayOperating { get; set; }
}
