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

      public Chromosome Clone()
      {
         var clone = new Chromosome(Gene.GetLength(0), Gene.GetLength(1));
         for (int i = 0; i < Gene.GetLength(0); i++)
         {
            for (int j = 0; j < Gene.GetLength(1); j++)
            {
               clone.Gene[i, j] = this.Gene[i, j];
            }
         }
         clone.Fitness = this.Fitness;
         clone.calculated = this.calculated;
         return clone;
      }
   }
}