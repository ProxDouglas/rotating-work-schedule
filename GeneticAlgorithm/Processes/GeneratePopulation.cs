namespace rotating_work_schedule.GeneticAlgorithm.Processes;

using rotating_work_schedule.Models;
public class GeneratePopulation
{
   public async Task<List<Chromosome>> Run(ConfigurationSchedule configuration)
   {
      List<Task<Chromosome>> tasks = new List<Task<Chromosome>>(configuration.PopulationSize);
      // List<Chromosome> chromosomes = new List<Chromosome>(Configuration.PopulationSize);

      for (int p = 0; p < configuration.PopulationSize; p++)
      {
         tasks.Add(Task.Run(() => this.GenerateChromosome(configuration)));
         // chromosomes.Add(this.GenerateChromosome());
      }

      var chromosomes = await Task.WhenAll(tasks);

      return [.. chromosomes];
   }

   private Chromosome GenerateChromosome(ConfigurationSchedule configuration)
   {
      Chromosome chromosome = new Chromosome(configuration.RowsSize, configuration.ColumnsSize);

      for (int row = 0; row < configuration.RowsSize; row++)
      {
         int workload = configuration.Employees[row].JobPosition?.Workload ?? 0; // Workload in hours
         int totalSlots = workload * 2; // Convert hours to 30-minute slots

         for (int day = 0; day < configuration.WorkDays.Count(); day++)
         {
            WorkDay dayWork = configuration.WorkDays[day];
            OperatingSchedule schedule = dayWork.OperatingSchedule;
            // Calculate the start and end columns for the current day
            int startColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
            int endColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);
            int limitSlot = endColumn - totalSlots;

            int startSlot = configuration.Random.Next(startColumn/2, limitSlot/2 + 1) * 2; // Randomly select a column within the working hours

            for (int column = startSlot; column < startSlot + totalSlots && column < endColumn; column++)
            {
               chromosome.Gene[row, column] = 1;
            }
         }
      }

      return chromosome;
   }
}