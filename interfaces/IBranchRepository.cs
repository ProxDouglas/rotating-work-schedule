using rotating_work_schedule.Models;

namespace YourNamespace.Repositories
{
   public interface IBranchRepository
   {
      Task<IEnumerable<Branch>> GetAllAsync();
      Task<Branch?> GetByIdAsync(int id);
   }
}