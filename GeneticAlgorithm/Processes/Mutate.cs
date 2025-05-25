namespace rotating_work_schedule.GeneticAlgorithm.Processes;

using rotating_work_schedule.Models;
public class Mutate
{
   // public void Run(ConfigurationSchedule configuration, Chromosome chromosome)
   // {
   //    for (int row = 0; row < configuration.RowsSize; row++)
   //    {
   //       var employee = configuration.Employees[row];
   //       int workload = configuration.Employees[row].JobPosition?.Workload ?? 0; // Workload in hours
   //       int totalSlots = workload * 2; // Convert hours to 30-minute slots

   //       for (int day = 0; day < configuration.WorkDays.Count(); day++)
   //       {
   //          if (configuration.Random.NextDouble() < configuration.MutationRate)
   //          {

   //             WorkDay dayWork = configuration.WorkDays[day];

   //             // Verifica se o funcionário está indisponível neste dia
   //             bool isUnavailable = employee.WorkOffs.Any(wd => wd.EffectiveDate.Date == dayWork.EffectiveDate.Date);

   //             if (!isUnavailable)
   //                this.FillDay(dayWork, chromosome, configuration, row, totalSlots);
   //          }
   //       }
   //    }
   // }

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
      bool isUnavailable = employee.WorkOffs.Any(wd => wd.EffectiveDate.Date == dayWork.EffectiveDate.Date);

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
      OperatingSchedule schedule = dayWork.OperatingSchedule;
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