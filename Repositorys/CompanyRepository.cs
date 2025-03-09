using Microsoft.EntityFrameworkCore;
using rotating_work_schedule.Database;
using rotating_work_schedule.Models;
using YourNamespace.Repositories;

namespace rotating_work_schedule.Repositorys
{
    public class CompanyRepository(AppDbContext _context) : ICompanyRepository
    {
        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Company.ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Company.FindAsync(id);
        }

        public async Task AddAsync(Company company)
        {
            _context.Company.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Company.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var company = await _context.Company.FindAsync(id);
            if (company != null)
            {
                _context.Company.Remove(company);
                await _context.SaveChangesAsync();
            }
        }
    }
}