namespace rotating_work_schedule.GeneticAlgorithm.Processes;

using rotating_work_schedule.Models;
public class GeneratePopulation
{
   public async Task<List<Chromosome>> Run(ConfigurationSchedule configuration, int size = 0)
   {
      List<Task<Chromosome>> tasks = new List<Task<Chromosome>>(size > 0 ? size : configuration.PopulationSize);

      for (int p = 0; p < configuration.PopulationSize; p++)
      {
         tasks.Add(Task.Run(() => this.GenerateChromosome(configuration)));
      }

      var chromosomes = await Task.WhenAll(tasks);

      return [.. chromosomes];
   }

   public Chromosome GenerateChromosome(ConfigurationSchedule configuration)
   {
      Chromosome chromosome = new Chromosome(configuration.RowsSize, configuration.ColumnsSize);

      for (int row = 0; row < configuration.RowsSize; row++)
      {
         var employee = configuration.Employees[row];
         int workload = employee.JobPosition?.Workload ?? 0; // Workload in hours
         int totalSlots = workload * 2; // Convert hours to 30-minute slots

         for (int day = 0; day < configuration.WorkDays.Count(); day++)
         {
            WorkDay dayWork = configuration.WorkDays[day];

            // Verifica se o funcionário está indisponível neste dia
            bool isUnavailable = employee.WorkOffs.Any(wd => wd.EffectiveDate.Date == dayWork.EffectiveDate.Date);

            if (!isUnavailable)
               this.FillDay(dayWork, chromosome, configuration, row, totalSlots);
         }
      }

      return chromosome;
   }

   private void FillDay(WorkDay dayWork, Chromosome chromosome, ConfigurationSchedule configuration, int row, int totalSlots)
   {
      OperatingSchedule schedule = dayWork.OperatingSchedule;
      int startColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
      int endColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);
      int limitSlot = endColumn - totalSlots;

      int startSlot = configuration.Random.Next(startColumn / 2, limitSlot / 2 + 1) * 2;

      for (int column = startSlot; column < startSlot + totalSlots && column < endColumn; column++)
      {
         chromosome.Gene[row, column] = 1;
      }
   }
}