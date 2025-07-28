namespace WorkSchedule.Entities;

public class WorkScheduleGenerated
{
   public Guid Id { get; set; }

   public string Email { get; set; } = string.Empty;
   public string WebHookUrl { get; set; } = string.Empty;
   public List<EmployeeSchedule> EmployeeSchedule { get; set; } = new();

   public bool IsValid { get; set; } = false;
   public string ErrorMessage { get; set; } = string.Empty;
}
