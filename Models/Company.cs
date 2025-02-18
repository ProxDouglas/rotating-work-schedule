using System.ComponentModel.DataAnnotations;

namespace rotating_work_schedule.Models
{
   public class Company
   {
      [Key]
      public int Id { get; set; }
      [Required]
      public string Name { get; set; } = string.Empty;
   }
}