namespace rotating_work_schedule.GeneticAlgorithm;

using rotating_work_schedule.Models;
public class ConfigurationSchedule
{
   public double MutationRate { get; } = 0.2;
   public int PopulationSize { get; private set; } = 100;
   public int Generations { get; private set; } = 1000;
   public int MaxFitness { get; private set; } = 100000;
   public Random Random { get; } = new Random();
   public int ColumnsSize { get; private set; }
   public int RowsSize { get; private set; }
   public DateTime StartDate { get; }
   public List<Chromosome> Population { get; set; } = new List<Chromosome>();
   public Employee[] Employees { get; }
   public JobPosition[] JobPositions { get; }
   public OperatingSchedule[] OperatingSchedule { get; }
   public WorkDay[] WorkDays { get; }

   public ConfigurationSchedule(Employee[] employees, JobPosition[] jobPositions, OperatingSchedule[] operatingSchedule, WorkDay[] workDays, int generations)
   {
      Employees = employees.OrderBy(emp => emp?.JobPosition?.Id).ToArray();
      JobPositions = jobPositions;
      OperatingSchedule = operatingSchedule.OrderBy(os => os.DayOperating).ToArray();
      WorkDays = workDays.OrderBy(os => os.EffectiveDate).ToArray();
      RowsSize = Employees.Length;
      ColumnsSize = WorkDays.Length * 24 * 2;
      StartDate = WorkDays[0].EffectiveDate;
      if (generations > 0)
         Generations = generations;
   }

   public int GetColumnFromDateTime(DateTime date, TimeSpan time)
   {
      int daysOffset = (date.Date - this.StartDate.Date).Days;
      int totalMinutes = (daysOffset * 24 * 60) + (time.Hours * 60) + time.Minutes;
      return totalMinutes / 30;
   }

   public WorkDay? GetDateAndWorkDayFromColumn(int column)
   {
      int totalMinutes = column * 30;
      int daysOffset = totalMinutes / (24 * 60);
      DateTime currentDate = this.StartDate.AddDays(daysOffset);

      return this.WorkDays.FirstOrDefault(wd => wd.EffectiveDate.Date == currentDate.Date);
   }
}
