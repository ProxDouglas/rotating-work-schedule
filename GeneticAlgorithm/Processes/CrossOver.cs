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
            OperatingSchedule schedule = dayWork.OperatingSchedule;
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

   // public Chromosome Run(ConfigurationSchedule configuration, Chromosome parent1, Chromosome parent2)
   // {
   //    Chromosome child = new(configuration.RowsSize, configuration.ColumnsSize);

   //    Chromosome highFitnessParent, lowFitnessParent;
   //    if (parent1.Fitness >= parent2.Fitness)
   //    {
   //       highFitnessParent = parent1;
   //       lowFitnessParent = parent2;
   //    }
   //    else
   //    {
   //       highFitnessParent = parent2;
   //       lowFitnessParent = parent1;
   //    }

   //    for (int row = 0; row < configuration.RowsSize; row++)
   //    {
   //       for (int day = 0; day < configuration.WorkDays.Count(); day++)
   //       {
   //          // 70% chance de pegar do parent de maior fitness, 30% do menor
   //          bool fromHighFitness = configuration.Random.NextDouble() < 0.7;
   //          var selectedParent = fromHighFitness ? highFitnessParent : lowFitnessParent;

   //          WorkDay dayWork = configuration.WorkDays[day];
   //          OperatingSchedule schedule = dayWork.OperatingSchedule;
   //          // Calculate the start and end columns for the current day
   //          int startColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
   //          int endColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);

   //          for (int column = startColumn; column < endColumn; column++)
   //          {
   //             child.Gene[row, column] = selectedParent.Gene[row, column];
   //          }
   //       }
   //    }

   //    return child;
   // }
}