using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rotating_work_schedule.Models
{
   public class Company
    {
        [Key] // PK
        public int Id { get; set; }

        [Required] // Campo obrigatório
        [StringLength(100)] // Tamanho máximo de 100 caracteres
        public string Name { get; set; }

        // Relacionamento com Tenant
        public ICollection<Tenant> Tenants { get; set; }
    }
}