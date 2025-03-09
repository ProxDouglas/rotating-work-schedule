using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rotating_work_schedule.Models
{
   public class JobPosition
   {
      [Key] // PK
      public int Id { get; set; }

      [Required] // Campo obrigatório
      [StringLength(100)] // Tamanho máximo de 100 caracteres
      public string Name { get; set; }

      [Required] // Campo obrigatório
      public int Workload { get; set; }

      [Required] // Campo obrigatório
      public int MaximumConsecutiveDays { get; set; }

      // Relacionamento com Employee
      public ICollection<Employee> Employees { get; set; }

      public ICollection<OperatingSchedule> OperatingSchedule { get; set; }
   }
}