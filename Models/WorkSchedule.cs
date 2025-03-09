using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rotating_work_schedule.Models
{
   public class WorkSchedule
   {
      [Key] // PK
      public int Id { get; set; }

      [Required] // Campo obrigatório
      [ForeignKey("Employee")] // Define a chave estrangeira
      public int EmployeeId { get; set; } // FK

      [Required] // Campo obrigatório
      public DateTime Start { get; set; }

      [Required] // Campo obrigatório
      public DateTime End { get; set; }

      public DateTime? Canceled { get; set; } // Pode ser nulo

      // Relacionamento com Employee
      public Employee Employee { get; set; }
   }
}