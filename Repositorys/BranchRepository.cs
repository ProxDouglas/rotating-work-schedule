using Microsoft.EntityFrameworkCore;
using rotating_work_schedule.Database;
using rotating_work_schedule.Models;
using YourNamespace.Repositories;

namespace rotating_work_schedule.Repositorys
{
    public class BranchRepository(AppDbContext _context) : IBranchRepository
    {
        public async Task<IEnumerable<Branch>> GetAllAsync()
        {
            return await _context.Branch.ToListAsync();
        }

        public async Task<Branch?> GetByIdAsync(int id)
        {
            return await _context.Branch.FindAsync(id);
        }
    }
}