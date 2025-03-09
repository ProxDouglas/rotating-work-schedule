using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rotating_work_schedule.Models
{
   public class Unavailability
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

      [Required] // Campo obrigatório
      [StringLength(200)] // Tamanho máximo de 200 caracteres
      public string Reason { get; set; }

      [Required] // Campo obrigatório
      public DateTime EffectiveDate { get; set; }

      [Required] // Campo obrigatório
      public DateTime Validity { get; set; }

      // Relacionamento com Employee
      public Employee Employee { get; set; }
   }
}