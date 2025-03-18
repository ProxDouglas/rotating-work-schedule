using rotating_work_schedule.Models;

namespace rotating_work_schedule.GeneticAlgorithm
{
   public class WorkScheduleGenerator
   {
      const double MutationRate = 0.05;
      const double CrossoverRate = 0.8;

      private Employee[] Employees { get; set; }
      private OperatingSchedule[] OperatingSchedule { get; set; }
      private const int PopulationSize = 100;
      private const int IntervalosPorDia = 1;
      private int Days = 1;
      private readonly Random random = new Random();
      private int columnsSize = 0;
      private int rowsSize = 0;
      public List<int[,]> population = new List<int[,]>();
      private DateTime startDate;


      public WorkScheduleGenerator(Employee[] employees, OperatingSchedule[] operatingSchedules, DateTime? startDate = null)
      {
         Employees = employees;
         OperatingSchedule = operatingSchedules.OrderBy(os => os.DayOfWeek).ToArray();
         this.rowsSize = this.Employees.Count();
         this.columnsSize = this.Days * 24 * 2;
         this.startDate = startDate ?? DateTime.Now;
      }

      public List<int[,]> getPopulation()
      {
         return this.population;
      }

      public List<int[,]> getBestSchedules()
      {
         return this.population.OrderByDescending(c => fitnessFunction(c)).ToList();
      }

      public async Task RunGeneticAlgorithmAsync(int generations)
      {
         await generatePopulationAsync();

         for (int generation = 0; generation < generations; generation++)
         {
            List<int[,]> newPopulation = new List<int[,]>();

            for (int i = 0; i < PopulationSize; i++)
            {
               int[,] parent1 = selectionByTournament();
               int[,] parent2 = selectionByTournament();

               int[,] child;
               if (random.NextDouble() < CrossoverRate)
               {
                  child = crossover(parent1, parent2);
               }
               else
               {
                  child = parent1;
               }

               if (random.NextDouble() < MutationRate)
               {
                  child = mutate(child);
               }

               int parent1Fitness = fitnessFunction(parent1);
               int childFitness = fitnessFunction(child);

               if (childFitness > parent1Fitness)
               {
                  newPopulation.Add(child);
               }
               else
               {
                  newPopulation.Add(parent1);
               }
            }

            this.population = newPopulation;
         }
      }

      private Boolean IsWithinWorkingHours(int column)
      {
         int totalMinutes = column * 30;
         int minutesInDay = totalMinutes % (24 * 60);
         int hour = minutesInDay / 60;
         int minute = minutesInDay % 60;

         OperatingSchedule schedule = OperatingSchedule[(int)getDayOfWeekFromColumn(column)];

         if (schedule != null)
         {
            TimeSpan currentTime = new TimeSpan(hour, minute, 0);

            if (currentTime >= schedule.Start && currentTime <= schedule.End)
               return true;
         }
         return false;
      }

      public async
      Task
      generatePopulationAsync()
      {
         List<Task<int[,]>> tasks = new List<Task<int[,]>>(PopulationSize);

         for (int p = 0; p < PopulationSize; p++)
         {
            tasks.Add(Task.Run(() => this.generateChromosome()));
         }

         var chromosomes = await Task.WhenAll(tasks);

         this.population = new List<int[,]>(chromosomes);
      }

      private int[,] generateChromosome()
      {
         int[,] chromosome = new int[rowsSize, columnsSize];

         for (int row = 0; row < this.rowsSize; row++)
         {
            for (int column = 0; column < this.columnsSize; column++)
            {
               bool isWorkingHour = IsWithinWorkingHours(column);
               if (isWorkingHour)
               {
                  chromosome[row, column] = random.Next(0, 2);
               }
               else
               {
                  chromosome[row, column] = 0;
               }
            }
         }

         return chromosome;
      }

      private int[,] mutate(int[,] chromosome)
      {
         int mutationRate = 1; // 1% mutation rate
         for (int row = 0; row < rowsSize; row++)
         {
            for (int column = 0; column < columnsSize; column++)
            {
               if (random.Next(0, PopulationSize) < mutationRate)
               {
                  chromosome[row, column] = chromosome[row, column] == 0 ? 1 : 0;
               }
            }
         }
         return chromosome;
      }

      int[,] selectionByTournament()
      {
         int tournamentSize = 5;
         int bestFitness = int.MinValue;
         int[,] bestChromosome = new int[rowsSize, columnsSize];

         for (int i = 0; i < tournamentSize; i++)
         {
            int randomIndex = random.Next(0, PopulationSize);
            int[,] chromosome = this.population[randomIndex];
            int fitness = fitnessFunction(chromosome);

            if (fitness > bestFitness)
            {
               bestFitness = fitness;
               bestChromosome = chromosome;
            }
         }

         return bestChromosome;
      }

      private int[,] crossover(int[,] parent1, int[,] parent2)
      {
         int[,] child = new int[rowsSize, columnsSize];


         for (int f = 0; f < rowsSize; f++)
         {
            for (int t = 0; t < columnsSize * this.Days; t++)
            {
               int cutoffPoint = random.Next() % 2;
               child[f, t] = cutoffPoint == 1 ? parent1[f, t] : parent2[f, t];
            }
         }

         return child;
      }

      private int fitnessFunction(int[,] chromosome)
      {
         int fitness = 0;
         int hoursWorked;

         for (int row = 0; row < rowsSize; row++)
         {

            hoursWorked = 0;

            for (int column = 0; column < columnsSize; column++)
            {
               bool isWorkingHour = IsWithinWorkingHours(column);

               fitness += isWithinOperatingHours(chromosome, row, column);

               if (isWorkingHour)
               {
                  fitness += consecutiveTimes(chromosome, row, column);

                  if (chromosome[row, column] == 1)
                  {
                     hoursWorked += 1;
                  }
               }
            }

            fitness += ValueWorkHours(row, hoursWorked);
         }

         return fitness;
      }

      private int consecutiveTimes(int[,] chromosome, int row, int column)
      {
         if (chromosome[row, column] == 1 && column > 1 && chromosome[row, column - 1] == 1)
         {
            return 2; // More points for consecutive 1s
         }
         return 0;
      }

      private DayOfWeek getDayOfWeekFromColumn(int column)
      {
         int totalMinutes = column * 30;
         int daysOffset = totalMinutes / (24 * 60);
         DateTime currentDate = startDate.AddDays(daysOffset);
         return currentDate.DayOfWeek;
      }

      private int isWithinOperatingHours(int[,] chromosome, int row, int column)
      {
         bool isWorkingHour = IsWithinWorkingHours(column);
         if (chromosome[row, column] == 0)
            return 0;

         return isWorkingHour ? 10 : -10;
      }

      private int ValueWorkHours(int row, int hoursWorked)
      {
         JobPosition? jobPosition = this.Employees[row].JobPosition;
         if (jobPosition == null)
            return 0;

         // Raízes da função quadrática
         double root1 = jobPosition.Workload - 2;
         double root2 = jobPosition.Workload + 2;

         // Coordenada x do vértice (ponto médio das raízes)
         double xVertex = (root1 + root2) / 2;

         // Como o vértice é o ponto de máximo, substituímos x = xVertex na função
         // f(xVertex) = a
         // A função é f(x) = k(x - root1)(x - root2)
         // No vértice, f(xVertex) = k(xVertex - root1)(xVertex - root2) = a

         // Calcula k
         double k = jobPosition.Workload / ((xVertex - root1) * (xVertex - root2));

         // Como a parábola tem ponto de máximo, k deve ser negativo
         k = -Math.Abs(k);

         // Forma expandida da função quadrática
         double b = -k * (root1 + root2);
         double c = k * root1 * root2;

         //fatored form {k}(x - {root1})(x - {root2})
         //exponded form {k}x² + {b}x + {c}
         double functionValue = k * (hoursWorked * hoursWorked) + b*hoursWorked + c;

         return (int)Math.Round(functionValue);
      }

      public void printMatrixList(List<int[,]> matrices)
      {
         for (int i = 0; i < matrices.Count; i++)
         {
            Console.WriteLine($"Matriz {i + 1}:");
            this.printMatrix(matrices[i]);
            Console.WriteLine();
         }
      }

      private void printMatrix(int[,] matrix)
      {
         int rows = matrix.GetLength(0);
         int cols = matrix.GetLength(1);

         for (int i = 0; i < rows; i++)
         {
            // Console.Write("Employee " + Employees[i].Name + ": ");
            for (int j = 0; j < cols; j++)
            {
               Console.Write(matrix[i, j] + " ");
            }
            Console.WriteLine();
         }
      }
   }
}