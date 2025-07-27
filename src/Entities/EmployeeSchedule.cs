using System.ComponentModel.DataAnnotations;

namespace WorkSchedule.Entities;

public class EmployeeSchedule
{
   public string Name { get; set; } = string.Empty;

   public List<WorkDaySchedule> WorkDays { get; set; } = new List<WorkDaySchedule>();
}

public class WorkDaySchedule
{
   public DateOnly Date { get; set; }

   public TimeSpan Start { get; set; } = TimeSpan.Zero;

   public TimeSpan End { get; set; } = TimeSpan.Zero;
}
