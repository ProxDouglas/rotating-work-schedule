using System.ComponentModel.DataAnnotations;

namespace rotating_work_schedule.Models
{
   public class WorkSchedule
   {
      [Required] 
      public DateTime Start { get; set; }

      [Required] 
      public DateTime End { get; set; }
   }
}