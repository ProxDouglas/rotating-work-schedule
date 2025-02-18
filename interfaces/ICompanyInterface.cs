using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rotating_work_schedule.Controllers;
using rotating_work_schedule.Models;

namespace rotating_work_schedule.Interfaces
{
    public interface ICompanyInterface
    {
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(int id);
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(int id);
    }
}