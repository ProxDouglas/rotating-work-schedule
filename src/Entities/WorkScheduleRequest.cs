namespace WorkSchedule.Entities;

public class WorkScheduleRequest
{
   public Guid Id { get; set; } = Guid.NewGuid();
   public List<Employee> Employees { get; set; } = new List<Employee>();

   public List<JobPosition> JobPositions { get; set; } = new List<JobPosition>();

   public List<OperatingSchedule> OperatingSchedule { get; set; } = new List<OperatingSchedule>();

   public List<WorkDay> WorkDay { get; set; } = new List<WorkDay>();
}
