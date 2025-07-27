namespace WorkSchedule.GeneticAlgorithm;

using WorkSchedule.Entities;
using RotatingWorkSchedule.Enums;

public class ConfigurationSchedule
{
   public double MutationRate { get; } = 0.2;
   public int PopulationSize { get; private set; } = 100;
   public int Generations { get; private set; } = 5000;
   public int MaxFitness { get; private set; } = 100000;
   public Random Random { get; } = new Random();
   public int ColumnsSize { get; private set; }
   public int RowsSize { get; private set; }

   public int GenarationValue { get; set; } = 0;
   public DateOnly StartDate { get; }
   public List<Chromosome> Population { get; set; } = new List<Chromosome>();
   public List<Employee> Employees { get; }
   public List<JobPosition> JobPositions { get; }
   public List<OperatingSchedule> OperatingSchedule { get; }
   public WorkDay[] WorkDays { get; }

   public ConfigurationSchedule(
      List<Employee> employees,
      List<JobPosition> jobPositions,
      List<OperatingSchedule> operatingSchedule,
      WorkDay[] workDays,
      int generations
      )
   {
      Employees = employees.OrderBy(emp => emp?.Name).ToList();
      JobPositions = jobPositions;
      OperatingSchedule = operatingSchedule.OrderBy(os => os.DayOperating).ToList();
      WorkDays = workDays.OrderBy(os => os.EffectiveDate).ToArray();
      RowsSize = Employees.Count;
      ColumnsSize = WorkDays.Length * 24 * 2;
      StartDate = WorkDays[0].EffectiveDate;
      if (generations > 0)
         Generations = generations;
   }

   public int GetColumnFromDateTime(DateOnly date, TimeSpan time)
   {
      int daysOffset = date.DayNumber - this.StartDate.DayNumber;
      int totalMinutes = (daysOffset * 24 * 60) + (time.Hours * 60) + time.Minutes;
      return totalMinutes / 30;
   }

   public WorkDay? GetDateAndWorkDayFromColumn(int column)
   {
      int totalMinutes = column * 30;
      int daysOffset = totalMinutes / (24 * 60);
      DateOnly currentDate = this.StartDate.AddDays(daysOffset);

      return this.WorkDays.FirstOrDefault(wd => wd.EffectiveDate == currentDate);
   }

   public OperatingSchedule GetOperatingScheduleByDay(DayOperating dayOperating)
   {
      return OperatingSchedule.First(os => os.DayOperating == dayOperating);
   }
}
