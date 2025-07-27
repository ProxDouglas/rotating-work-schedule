using WorkSchedule.Entities;

namespace WorkSchedule.GeneticAlgorithm
{
   public class Chromosome
   {
      public int Fitness = 0;
      public bool calculated = false;

      public int[,] Gene { get; set; }

      public Chromosome(int rowsSize, int columnsSize)
      {
         Gene = new int[rowsSize, columnsSize];
      }

      public void AppendColumnsFrom(Chromosome other)
      {
         if (other == null || other.Gene == null)
            throw new ArgumentNullException(nameof(other));

         int rows = Gene.GetLength(0);
         int currentCols = Gene.GetLength(1);
         int otherCols = other.Gene.GetLength(1);

         if (rows != other.Gene.GetLength(0))
            throw new ArgumentException("Row counts must match.");

         int[,] newGene = new int[rows, currentCols + otherCols];

         for (int i = 0; i < rows; i++)
         {
            for (int j = 0; j < currentCols; j++)
            {
               newGene[i, j] = Gene[i, j];
            }
            for (int j = 0; j < otherCols; j++)
            {
               newGene[i, currentCols + j] = other.Gene[i, j];
            }
         }

         Gene = newGene;
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