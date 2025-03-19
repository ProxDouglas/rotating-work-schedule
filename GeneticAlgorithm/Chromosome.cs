using rotating_work_schedule.Models;

namespace rotating_work_schedule.GeneticAlgorithm
{
   public class Chromosome
   {
      public int Fitness = 0;
      public bool calculated = false;

      public int[,] Gene { get; set; }

      public Chromosome() { }

      public Chromosome(int rowsSize, int columnsSize)
      {
         Gene = new int[rowsSize, columnsSize];
      }
   }
}