using rotating_work_schedule.Models;

namespace rotating_work_schedule.GeneticAlgorithm;
public class PrintConsole
{

   private TimeSpan CalculateTime(int column)
   {
      int totalMinutes = column * 30;
      int minutesInDay = totalMinutes % (24 * 60);
      int hour = minutesInDay / 60;
      int minute = minutesInDay % 60;

      return new TimeSpan(hour, minute, 0);
   }

   public WorkDay? GetDateAndWorkDayFromColumn(int column, DateTime startDate, List<WorkDay> workDays)
   {
      int totalMinutes = column * 30;
      int daysOffset = totalMinutes / (24 * 60);
      DateTime currentDate = startDate.AddDays(daysOffset);

      return workDays.FirstOrDefault(wd => wd.EffectiveDate.Date == currentDate.Date);
   }
   public void printMatrixList(List<Chromosome> best, List<Employee> employees, DateTime startDate, List<WorkDay> workDays)
   {
      // List<Chromosome> best = Configuration.Population.OrderByDescending(c => FitnessFunction(c)).Take(1).ToList();

      for (int i = 0; i < best.Count; i++)
      {
         Console.WriteLine($"Matriz {i + 1}:");
         Console.WriteLine($"Fitness {best[i].Fitness}:");
         Console.WriteLine($"Calculated {best[i].calculated}:");
         this.printMatrix(best[i].Gene);
         this.printSchedule(best[i].Gene, employees, startDate, workDays);
         Console.WriteLine();
      }
   }

   public void printMatrix(int[,] matrix)
   {
      int rows = matrix.GetLength(0);
      int cols = matrix.GetLength(1);

      for (int i = 0; i < rows; i++)
      {
         for (int j = 0; j < cols; j++)
         {
            Console.Write(matrix[i, j] + " ");
         }
         Console.WriteLine();
      }
   }

   public void printSchedule(int[,] matrix, List<Employee> employees, DateTime startDate, List<WorkDay> workDays)
   {
      int workingHours;
      int rows = matrix.GetLength(0);
      int cols = matrix.GetLength(1);

      for (int i = 0; i < rows; i++)
      {
         workingHours = 0;

         Employee employee = employees[i];
         JobPosition jobPosition = employee.JobPosition;

         Console.WriteLine("Employee: " + employees[i].Name);
         Console.WriteLine("Job: " + jobPosition.Name);
         // Console.Write("           ");


         for (int j = 0; j < cols; j++)
         {
            TimeSpan currentTime = this.CalculateTime(j);

            if (matrix[i, j] == 1)
               workingHours++;

            if ((matrix[i, j] == 1 && matrix[i, j - 1] == 0) || (matrix[i, j] == 1 && j == 0))
            {
               Console.Write(this.GetDateAndWorkDayFromColumn(j, startDate, workDays).EffectiveDate.ToString("dd/MM/yyyy") + ": ");
               Console.Write(" ( " + currentTime.ToString().Substring(0, 5) + " - ");
               // Console.Write("( " + (currentTime - new TimeSpan(0, 30, 0)).ToString().Substring(0, 5) + " - ");
               if (matrix[i, j + 1] == 0 || j == cols - 1)
               {
                  Console.Write((currentTime + new TimeSpan(0, 30, 0)).ToString().Substring(0, 5) + ") ");
               }
            }
            else if ((matrix[i, j] == 1 && matrix[i, j + 1] == 0) || (matrix[i, j] == 1 && j == cols - 1))
            {
               Console.Write((currentTime + new TimeSpan(0, 30, 0)).ToString().Substring(0, 5) + ") ");
            }

            if ((j % 48 == 0 && j != 0) || j == cols - 1)
            {
               Console.Write(" - " + new TimeSpan(workingHours / 2, (workingHours % 2) * 30, 0).ToString().Substring(0, 5));
               Console.WriteLine();

               workingHours = 0;
            }
         }
         Console.WriteLine();
      }
   }
}