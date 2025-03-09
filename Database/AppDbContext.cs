

using Microsoft.EntityFrameworkCore;
using rotating_work_schedule.Controllers;
using rotating_work_schedule.Models;

namespace rotating_work_schedule.Database
{
   public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
   {
      public DbSet<Branch> Branch { get; set; }
      public DbSet<Company> Company { get; set; }
      public DbSet<Employee> Employee { get; set; }
      public DbSet<JobPosition> JobPosition { get; set; }
      public DbSet<OperatingSchedule> OperatingSchedule { get; set; }
      public DbSet<Tenant> Tenant { get; set; }
      public DbSet<Unavailability> Unavailability { get; set; }
      public DbSet<WorkSchedule> WorkSchedule { get; set; }
   }
}