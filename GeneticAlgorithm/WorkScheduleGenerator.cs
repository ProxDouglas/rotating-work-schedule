using rotating_work_schedule.Models;

namespace rotating_work_schedule.GeneticAlgorithm
{
   public class WorkScheduleGenerator
   {

      private Employee[] Employees { get; set; }
      private int OperatingSchedule { get; set; }
      private const int PopulationSize = 100;   
      private const int IntervalosPorDia = 4;
      private int Days = 7;
      private Random random = new Random();
      private int columnsSize = 0;
      private int rowsSize = 0;

      public WorkScheduleGenerator() { }
      
      public WorkScheduleGenerator(Employee[] employees, OperatingSchedule operatingSchedule, int days)
      {
         Employees = employees;
         OperatingSchedule = Convert.ToInt32(operatingSchedule.End.TotalMinutes - operatingSchedule.Start.TotalMinutes);
         Days = days;


         this.rowsSize = this.Employees.Count() * this.Days;
         if(OperatingSchedule % 30 > 0){
            this.columnsSize = (int)(OperatingSchedule / 30);
            this.columnsSize++;
         } else {
            this.columnsSize = OperatingSchedule / 30;
         }

      }

      
      public async Task<List<int[,]>> GeneratePopulationAsync()
        {
         List<Task<int[,]>> tasks = new List<Task<int[,]>>(PopulationSize);

         for (int p = 0; p < PopulationSize; p++)
         {
               tasks.Add(Task.Run(() => GenerateChromosome()));
         }

         var chromosomes = await Task.WhenAll(tasks);

         return new List<int[,]>(chromosomes);
      }

      private int[,] generateChromosome(){
         int[,] chromosome = new int[columnsSize, rowsSize];

         for(int column = 0; column < this.columnsSize; column++)
         {
            for(int row = 0; row < this.rowsSize; row++)
            {
               chromosome[column, row] = random.Next(0, 2);
            }
         }

         return chromosome;
      }  
   }
}