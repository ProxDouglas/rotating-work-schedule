using WorkSchedule.Entities;

namespace WorkSchedule.QueueRabbitMQ;

public class WorkScheduleGenerated
{
   public Guid Id { get; set; }
   public List<EmployeeSchedule> EmployeeSchedule { get; set; } = new();

   public bool IsValid { get; set; } = false;
   public string ErrorMessage { get; set; } = string.Empty;
}
