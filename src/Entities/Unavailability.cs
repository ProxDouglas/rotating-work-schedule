using System.ComponentModel.DataAnnotations;

namespace WorkSchedule.Entities;

public class Unavailability
{
   [Required]
   public DateTime Start { get; set; }

   [Required]
   public DateTime End { get; set; }
}
