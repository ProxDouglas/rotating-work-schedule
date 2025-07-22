namespace rotating_work_schedule.GeneticAlgorithm.Processes;

using rotating_work_schedule.Models;
public class CrossOver
{
   public Chromosome Run(ConfigurationSchedule configuration, Chromosome parent1, Chromosome parent2)
   {
      Chromosome child = new(configuration.RowsSize, configuration.ColumnsSize);

      for (int row = 0; row < configuration.RowsSize; row++)
      {
         for (int day = 0; day < configuration.WorkDays.Count(); day++)
         {
            var escolherDoPai1 = configuration.Random.Next(2) == 0;
            var paiEscolhido = escolherDoPai1 ? parent1 : parent2;

            WorkDay dayWork = configuration.WorkDays[day];
            OperatingSchedule schedule = configuration.GetOperatingScheduleByDay(dayWork.DayOperating);
            // Calculate the start and end columns for the current day
            int startColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
            int endColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);

            for (int column = startColumn; column < endColumn; column++)
            {
               child.Gene[row, column] = paiEscolhido.Gene[row, column];
            }
         }
      }

      return child;
   }
}