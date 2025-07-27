using rotating_work_schedule.Models;

namespace rotating_work_schedule.GeneticAlgorithm.PrePocessing;

public class GenerateDayOff
{
   public List<Employee> Run(List<Employee> employees, List<WorkDay> workDays)
   {
      int candidateIndex, dayOff = 0;

      var sortedWorkDays = workDays.OrderBy(w => w.EffectiveDate).ToList();
      var employeeDaysOff = this.CreateDictionary(employees);

      var employeesByPosition = employees.GroupBy(e => e.JobPosition.Name)
                                         .ToDictionary(g => g.Key, g => g.ToList());

      for (int i = 0; i < sortedWorkDays.Count; i++)
      {
         foreach (var positionGroup in employeesByPosition)
         {
            var positionEmployees = positionGroup.Value;

            var candidates = positionEmployees.Where(emp =>
                NeedsDayOff(emp, employeeDaysOff, sortedWorkDays, i)).ToList();

            candidateIndex = 0;
            dayOff = 0;
            foreach (var employee in candidates)
            {
               var currentDay = sortedWorkDays[i - dayOff];
               employeeDaysOff[employee].Add(currentDay);
               if (candidateIndex > 0 && candidateIndex % this.CountDayOff(positionEmployees) == 0)
               {
                  dayOff++;
               }
               candidateIndex++;
            }
         }
      }

      this.UpdateWorkOffs(employees, employeeDaysOff, sortedWorkDays);

      return employees;
   }

   private Dictionary<Employee, List<WorkDay>> CreateDictionary(List<Employee> employees)
   {
      var employeeDaysOff = new Dictionary<Employee, List<WorkDay>>();
      foreach (var employee in employees)
      {
         employeeDaysOff[employee] = [];
      }
      return employeeDaysOff;
   }

   private void UpdateWorkOffs(List<Employee> employees, Dictionary<Employee, List<WorkDay>> employeeDaysOff, List<WorkDay> workDays)
   {
      foreach (var employee in employees)
      {
         var daysOff = employeeDaysOff.TryGetValue(employee, out var offs) ? offs : new List<WorkDay>();

         // Adiciona os dias de Unavailability como folga
         if (employee.Unavailabilities != null)
         {
            foreach (var unavailability in employee.Unavailabilities)
            {
               foreach (var day in workDays)
               {
                  if (day.EffectiveDate >= DateOnly.FromDateTime(unavailability.Start.Date) && day.EffectiveDate <= DateOnly.FromDateTime(unavailability.End.Date))
                  {
                     if (!daysOff.Contains(day))
                        daysOff.Add(day);
                  }
               }
            }
         }

         employee.WorkOffs = [.. daysOff.OrderBy(d => d.EffectiveDate)];
      }
   }

   private int CountDayOff(List<Employee> positionEmployees)
   {
      int employeesWithDayOff = positionEmployees.Count() / 7;
      if (positionEmployees.Count() % 7 > 0)
         employeesWithDayOff++;
      return employeesWithDayOff;
   }

   private bool NeedsDayOff(
       Employee employee,
       Dictionary<Employee, List<WorkDay>> employeeDaysOff,
       List<WorkDay> workDays,
       int currentDayIndex)
   {
      // Considera Unavailabilities como dias de folga
      var consecutiveDays = ConsecutiveWorkDays(employee, employeeDaysOff, workDays, currentDayIndex);
      return consecutiveDays >= 7;
   }

   private int ConsecutiveWorkDays(
       Employee employee,
       Dictionary<Employee, List<WorkDay>> employeeDaysOff,
       List<WorkDay> workDays,
       int currentDayIndex)
   {
      int consecutiveDays = 0;

      for (int i = currentDayIndex; i >= 0; i--)
      {
         var day = workDays[i];

         // Verifica se o funcionário está de folga ou indisponível neste dia
         bool isDayOff = employeeDaysOff[employee].Contains(day);
         bool isUnavailable = employee.Unavailabilities != null &&
            employee.Unavailabilities.Any(u =>
               day.EffectiveDate >= DateOnly.FromDateTime(u.Start.Date) && day.EffectiveDate <= DateOnly.FromDateTime(u.End.Date));

         if (isDayOff || isUnavailable)
            break;

         consecutiveDays++;

         if (consecutiveDays >= 7)
            break;
      }

      return consecutiveDays;
   }
}