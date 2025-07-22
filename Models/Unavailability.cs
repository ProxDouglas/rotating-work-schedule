using System.ComponentModel.DataAnnotations;

namespace rotating_work_schedule.Models
{
   public class Unavailability
   {
      [Required] 
      public DateTime Start { get; set; }

      [Required] 
      public DateTime End { get; set; }

      [Required] 
      public DateTime EffectiveDate { get; set; }

      [Required] 
      public DateTime Validity { get; set; }
   }
}