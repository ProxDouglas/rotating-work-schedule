using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rotating_work_schedule.Models
{
   public class Employee
   {
      [Key] // PK
      public int Id { get; set; }

      [Required] // Campo obrigatório
      [ForeignKey("Branch")] // Define a chave estrangeira
      public int BranchId { get; set; } // FK

      [Required] // Campo obrigatório
      [ForeignKey("JobPosition")] // Define a chave estrangeira
      public int JobPositionId { get; set; } // FK

      [Required] // Campo obrigatório
      [StringLength(100)] // Tamanho máximo de 100 caracteres
      public string Name { get; set; }

      [Required] // Campo obrigatório
      [EmailAddress] // Valida o formato de e-mail
      [StringLength(100)] // Tamanho máximo de 100 caracteres
      public string Email { get; set; }

      // Relacionamento com Branch
      public Branch Branch { get; set; }

      // Relacionamento com JobPosition
      public JobPosition JobPosition { get; set; }

      // Relacionamento com Unavailability
      public ICollection<Unavailability> Unavailabilities { get; set; }

      // Relacionamento com WorkSchedule
      public ICollection<WorkSchedule> WorkSchedules { get; set; }
   }
}