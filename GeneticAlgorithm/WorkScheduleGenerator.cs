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
      private int Days = 2;
      private readonly Random random = new Random();
      private int columnsSize = 0;
      private int rowsSize = 0;
      public List<Chromosome> population = new List<Chromosome>();
      private DateTime startDate;


      public WorkScheduleGenerator(Employee[] employees, OperatingSchedule[] operatingSchedules, DateTime? startDate = null)
      {
         Employees = employees;
         OperatingSchedule = operatingSchedules.OrderBy(os => os.DayOfWeek).ToArray();
         this.rowsSize = this.Employees.Count();
         this.columnsSize = this.Days * 24 * 2;
         this.startDate = startDate ?? DateTime.Now;
      }

      public void SortPopulation()
      {
         this.population.Sort((a, b) => FitnessFunction(a).CompareTo(FitnessFunction(b)));
      }

      public List<int[,]> getBestSchedules()
      {
         // return this.population.OrderByDescending(c => FitnessFunction(c)).Take(3).Select(c => c.Gene).ToList();
         return this.population.OrderByDescending(c => FitnessFunction(c)).Select(c => c.Gene).ToList();
      }

      public async Task RunGeneticAlgorithmAsync(int generations)
      {
         await generatePopulationAsync();
         this.SortPopulation();

         for (int generation = 0; generation < generations; generation++)
         {

            for (int i = 0; i < PopulationSize; i++)
            {
               // Chromosome parent1 = selectionByTournament();
               // Chromosome parent2 = selectionByTournament();
               int menor = PopulationSize - i;
               Chromosome parent1 = this.population[i];
               Chromosome parent2 = this.population[PopulationSize - i - 1];

               Chromosome child;
               Chromosome child2;
               // if (random.NextDouble() < CrossoverRate)
               // {
               child = Crossover(parent1, parent2);
               child2 = Crossover(parent1, parent2);
               // }
               // else
               // {
               //    child = parent1;
               // }

               if (random.NextDouble() < MutationRate)
               {
                  child = Mutate(child);
               }
               if (random.NextDouble() < MutationRate)
               {
                  child2 = Mutate(child2);
               }

               int parent1Fitness = FitnessFunction(parent1);
               int parent2Fitness = FitnessFunction(parent2);
               int childFitness = FitnessFunction(child);
               int child2Fitness = FitnessFunction(child2);

               if (childFitness > parent1Fitness && childFitness > parent2Fitness)
               {
                  this.population.Add(child);
               }

               if (child2Fitness > parent1Fitness && child2Fitness > parent2Fitness)
               {
                  this.population.Add(child);
               }
            }

            this.SortPopulation();
            if (this.population.Count > PopulationSize)
               this.population.RemoveRange(this.population.Count - (PopulationSize + 1), this.population.Count - PopulationSize);
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
         List<Task<Chromosome>> tasks = new List<Task<Chromosome>>(PopulationSize);

         for (int p = 0; p < PopulationSize; p++)
         {
            tasks.Add(Task.Run(() => this.generateChromosome()));
         }

         var chromosomes = await Task.WhenAll(tasks);

         this.population = new List<Chromosome>(chromosomes);
      }

      private Chromosome generateChromosome()
      {
         Chromosome chromosome = new Chromosome(rowsSize, columnsSize);

         for (int row = 0; row < this.rowsSize; row++)
         {
            for (int column = 0; column < this.columnsSize; column++)
            {
               bool isWorkingHour = IsWithinWorkingHours(column);
               if (isWorkingHour)
               {
                  chromosome.Gene[row, column] = random.Next(0, 2);
               }
               else
               {
                  chromosome.Gene[row, column] = 0;
               }
            }
         }

         return chromosome;
      }

      private Chromosome Mutate(Chromosome chromosome)
      {
         int mutationRate = 1; // 1% mutation rate
         for (int row = 0; row < rowsSize; row++)
         {
            for (int column = 0; column < columnsSize; column++)
            {
               if (this.IsWithinWorkingHours(column) && random.Next(0, PopulationSize) < mutationRate)
               {
                  chromosome.Gene[row, column] = chromosome.Gene[row, column] == 0 ? 1 : 0;
               }
            }
         }
         return chromosome;
      }

      Chromosome selectionByTournament()
      {
         int tournamentSize = 5;
         int bestFitness = int.MinValue;
         Chromosome bestChromosome = new(rowsSize, columnsSize);

         for (int i = 0; i < tournamentSize; i++)
         {
            int randomIndex = random.Next() % PopulationSize;
            Chromosome chromosome = this.population[randomIndex];
            int fitness = FitnessFunction(chromosome);

            if (fitness > bestFitness)
            {
               bestFitness = fitness;
               bestChromosome = chromosome;
            }
         }

         return bestChromosome;
      }

      private Chromosome Crossover(Chromosome parent1, Chromosome parent2)
      {
         Chromosome child = new(rowsSize, columnsSize);

         for (int f = 0; f < rowsSize; f++)
         {
            for (int t = 0; t < columnsSize; t++)
            {
               int cutoffPoint = random.Next() % 2;
               child.Gene[f, t] = cutoffPoint == 1 ? parent1.Gene[f, t] : parent2.Gene[f, t];
            }
         }

         return child;
      }

      private int FitnessFunction(Chromosome chromosome)
      {
         if (chromosome.calculated == false)
         {
            chromosome.Fitness = calculateFitness(chromosome);
            chromosome.calculated = true;
         }

         return chromosome.Fitness;
      }

      private int calculateFitness(Chromosome chromosome)
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

                  if (chromosome.Gene[row, column] == 1)
                  {
                     hoursWorked += 1;
                  }
               }
               else
               {
                  fitness += ValueWorkHours(row, hoursWorked);
                  hoursWorked = 0;
               }
            }


         }

         chromosome.calculated = true;
         chromosome.Fitness = fitness;

         return fitness;
      }

      private int consecutiveTimes(Chromosome chromosome, int row, int column)
      {
         if (chromosome.Gene[row, column] == 1 && column > 1 && chromosome.Gene[row, column - 1] == 1)
         {
            return 5; // More points for consecutive 1s
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

      private int isWithinOperatingHours(Chromosome chromosome, int row, int column)
      {
         bool isWorkingHour = IsWithinWorkingHours(column);
         if (chromosome.Gene[row, column] == 0)
            return 0;

         return isWorkingHour ? 10 : -100;
      }

      private int ValueWorkHours(int row, int hoursWorked)
      {
         int workload = 0;
         JobPosition? jobPosition = this.Employees[row].JobPosition;
         if (jobPosition == null)
            return 0;

         workload = jobPosition.Workload * 2;

         // Raízes da função quadrática
         double root1 = workload - 1;
         double root2 = workload + 1;

         // Coordenada x do vértice (ponto médio das raízes)
         double xVertex = (root1 + root2) / 2;

         // Como o vértice é o ponto de máximo, substituímos x = xVertex na função
         // f(xVertex) = a
         // A função é f(x) = k(x - root1)(x - root2)
         // No vértice, f(xVertex) = k(xVertex - root1)(xVertex - root2) = a

         // Calcula k
         double k = workload / ((xVertex - root1) * (xVertex - root2));

         // Como a parábola tem ponto de máximo, k deve ser negativo
         k = -Math.Abs(k);

         // Forma expandida da função quadrática
         double b = -k * (root1 + root2);
         double c = k * root1 * root2;

         //fatored form {k}(x - {root1})(x - {root2})
         //exponded form {k}x² + {b}x + {c}
         double functionValue = k * (hoursWorked * hoursWorked) + b * hoursWorked + c;

         if (functionValue > 0)
            functionValue += functionValue * 10;

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