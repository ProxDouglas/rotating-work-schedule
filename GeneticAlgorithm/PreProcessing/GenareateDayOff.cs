using rotating_work_schedule.Models;

namespace rotating_work_schedule.GeneticAlgorithm.PrePocessing;

public class GenerateDayOff
{
   public List<Employee> Run(List<Employee> employees, List<WorkDay> workDays)
   {
      // Ordenar os dias de trabalho por data
      var sortedWorkDays = workDays.OrderBy(w => w.EffectiveDate).ToList();

      // Dicionário para armazenar os dias de folga de cada funcionário
      var employeeDaysOff = new Dictionary<Employee, List<WorkDay>>();

      // Inicializar o dicionário para cada funcionário
      foreach (var employee in employees)
      {
         employeeDaysOff[employee] = new List<WorkDay>();
      }

      // Agrupar funcionários por cargo
      var employeesByPosition = employees.GroupBy(e => e.JobPosition.Id)
                                       .ToDictionary(g => g.Key, g => g.ToList());

      // Para cada dia de trabalho, distribuir as folgas
      for (int i = 0; i < sortedWorkDays.Count; i++)
      {
         // Processar cada grupo de cargo separadamente
         foreach (var positionGroup in employeesByPosition)
         {
            var positionEmployees = positionGroup.Value;

            // Encontrar funcionários deste cargo que precisam de folga
            var candidates = positionEmployees.Where(emp =>
                NeedsDayOff(emp, employeeDaysOff, sortedWorkDays, i)).ToList();

            // Se muitos precisam de folga, selecionar os que mais precisam

            // Atribuir folgas
            int candidateIndex = 0;
            int dayOff = 0;
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

      // Atualizar a lista WorkOffs de cada funcionário com os dias de folga calculados
      foreach (var employee in employees)
      {
         if (employeeDaysOff.TryGetValue(employee, out var daysOff))
            employee.WorkOffs = new List<WorkDay>(daysOff);
         else
            employee.WorkOffs = new List<WorkDay>();
      }

      return employees;
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
      // Verificar se o funcionário já trabalhou 6 dias consecutivos
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

      // Retroceder a partir do dia atual para contar dias trabalhados consecutivos
      for (int i = currentDayIndex; i >= 0; i--)
      {
         var day = workDays[i];

         // Se o funcionário está de folga neste dia, parar a contagem
         if (employeeDaysOff[employee].Contains(day))
            break;

         consecutiveDays++;

         // Parar após verificar 7 dias (o máximo que nos interessa)
         if (consecutiveDays >= 7)
            break;
      }

      return consecutiveDays;
   }
}