using rotating_work_schedule.Interfaces;
using rotating_work_schedule.Models;

namespace YourNamespace.Repositories
{
   public interface ICompanyRepository
   {
      Task<IEnumerable<Company>> GetAllAsync();
      Task<Company?> GetByIdAsync(int id);
      Task AddAsync(Company company);
      Task UpdateAsync(Company company);
      Task DeleteAsync(int id);
   }
}