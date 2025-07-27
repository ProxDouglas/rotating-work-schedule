namespace WorkSchedule.GeneticAlgorithm.Processes;

using WorkSchedule.Entities;
public class Mutate
{
   public void Run(ConfigurationSchedule configuration, Chromosome chromosome)
   {
      // Seleciona aleatoriamente uma linha (funcionário)
      int row = configuration.Random.Next(0, configuration.RowsSize);
      var employee = configuration.Employees[row];
      int workload = employee.JobPosition?.Workload ?? 0; // Workload in hours
      int totalSlots = workload * 2; // Convert hours to 30-minute slots

      // Seleciona aleatoriamente um dia
      int day = configuration.Random.Next(0, configuration.WorkDays.Count());
      WorkDay dayWork = configuration.WorkDays[day];

      // Verifica se o funcionário está indisponível neste dia
      bool isUnavailable = employee.WorkOffs.Any(wd => wd.EffectiveDate == dayWork.EffectiveDate);

      // Aplica mutação com base na taxa de mutação e disponibilidade
      var MutationRate = configuration.MutationRate;
      if (configuration.GenarationValue > 100)
         MutationRate = 1.0;

      if (!isUnavailable && configuration.Random.NextDouble() < configuration.MutationRate)
      {
         this.FillDay(dayWork, chromosome, configuration, row, totalSlots);
      }
      
      chromosome.calculated = false;
   }

   private void FillDay(WorkDay dayWork, Chromosome chromosome, ConfigurationSchedule configuration, int row, int totalSlots)
   {
      OperatingSchedule schedule = configuration.GetOperatingScheduleByDay(dayWork.DayOperating);
      int startColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
      int endColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);
      int limitSlot = endColumn - totalSlots;

      int startSlot = configuration.Random.Next(startColumn / 2, limitSlot / 2 + 1) * 2;

      for (int column = startColumn; column < endColumn; column++)
      {
         chromosome.Gene[row, column] = 0;
      }

      for (int column = startSlot; column < startSlot + totalSlots && column < endColumn; column++)
      {
         chromosome.Gene[row, column] = 1;
      }
   }
}