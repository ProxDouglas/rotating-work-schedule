using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rotating_work_schedule.Models
{
   public class Branch
   {
      [Key] // PK
      public int Id { get; set; }

      [Required] // Campo obrigatório
      [StringLength(100)] // Tamanho máximo de 100 caracteres
      public string Name { get; set; }

      [Required] // Campo obrigatório
      [StringLength(50)] // Tamanho máximo de 50 caracteres
      public string Country { get; set; }

      [StringLength(50)] // Tamanho máximo de 50 caracteres
      public string State { get; set; }

      [StringLength(50)] // Tamanho máximo de 50 caracteres
      public string City { get; set; }

      // Relacionamento com Employee
      public ICollection<Employee> Employees { get; set; }

   }
}