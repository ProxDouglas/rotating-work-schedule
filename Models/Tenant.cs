using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rotating_work_schedule.Models
{
   public class Tenant
   {
      [Key] // PK
      public int Id { get; set; }

      [Required] // Campo obrigatório
      [ForeignKey("Company")] // Define a chave estrangeira
      public int CompanyId { get; set; } // FK

      [Required] // Campo obrigatório
      [StringLength(100)] // Tamanho máximo de 100 caracteres
      public string Name { get; set; }

      // Relacionamento com Company
      public Company Company { get; set; }
   }
}