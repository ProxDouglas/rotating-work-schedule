

using Microsoft.EntityFrameworkCore;
using rotating_work_schedule.Controllers;
using rotating_work_schedule.Models;

namespace rotating_work_schedule.Database
{
   public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
   {
      public DbSet<Company> Companies { get; set; }
   }
}