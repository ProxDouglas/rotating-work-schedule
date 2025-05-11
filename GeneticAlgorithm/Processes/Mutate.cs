namespace rotating_work_schedule.GeneticAlgorithm.Processes;

using rotating_work_schedule.Models;
public class Mutate
{
   public void Run(ConfigurationSchedule configuration, Chromosome chromosome)
   {
      for (int row = 0; row < configuration.RowsSize; row++)
      {
         int workload = configuration.Employees[row].JobPosition?.Workload ?? 0; // Workload in hours
         int totalSlots = workload * 2; // Convert hours to 30-minute slots

         for (int day = 0; day < configuration.WorkDays.Count(); day++)
         {
            if (configuration.Random.NextDouble() < configuration.MutationRate)
            {

               WorkDay dayWork = configuration.WorkDays[day];
               OperatingSchedule schedule = dayWork.OperatingSchedule;
               // Calculate the start and end columns for the current day
               int startColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
               int endColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);
               int limitSlot = endColumn - totalSlots;

               int startSlot = configuration.Random.Next(startColumn/2, limitSlot/2 + 1) * 2; // Randomly select a column within the working hours

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
      }
   }
}