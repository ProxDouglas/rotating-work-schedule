using rotating_work_schedule.Models;

namespace rotating_work_schedule.GeneticAlgorithm
{
   public class ConfigurationSchedule
   {
      public double MutationRate { get; } = 0.2;
      public int PopulationSize { get; private set; } = 100;
      public int Generations { get; private set; } = 100;
      public Random Random { get; } = new Random();
      public int ColumnsSize { get; private set; }
      public int RowsSize { get; private set; }
      public DateTime StartDate { get; }
      public List<Chromosome> Population { get; set; } = new List<Chromosome>();
      public Employee[] Employees { get; }
      public OperatingSchedule[] OperatingSchedule { get; }
      public WorkDay[] WorkDays { get; }

      public ConfigurationSchedule(Employee[] employees, OperatingSchedule[] operatingSchedule, WorkDay[] workDays, int generations)
      {
         Employees = employees.OrderBy(emp => emp?.JobPosition?.Id).ToArray();
         OperatingSchedule = operatingSchedule.OrderBy(os => os.DayOperating).ToArray();
         WorkDays = workDays.OrderBy(os => os.EffectiveDate).ToArray();
         RowsSize = Employees.Length;
         ColumnsSize = WorkDays.Length * 24 * 2;
         StartDate = WorkDays[0].EffectiveDate;
         Generations = generations > 0 ? generations : 100;
      }
   }
}