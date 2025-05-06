using rotating_work_schedule.Models;

namespace rotating_work_schedule.GeneticAlgorithm
{
   public class WorkScheduleGenerator
   {
      private ConfigurationSchedule Configuration { get; set; }

      public WorkScheduleGenerator(ConfigurationSchedule configurationSchedule)
      {
         Configuration = configurationSchedule;
      }

      public void SortPopulation()
      {
         Configuration.Population.Sort((a, b) => FitnessFunction(b).CompareTo(FitnessFunction(a)));
      }

      public List<int[,]> getBestSchedules()
      {
         return Configuration.Population.OrderByDescending(c => FitnessFunction(c)).Take(3).Select(c => c.Gene).ToList();
      }

      private WorkDay? GetDateAndWorkDayFromColumn(int column)
      {
         int totalMinutes = column * 30;
         int daysOffset = totalMinutes / (24 * 60);
         DateTime currentDate = Configuration.StartDate.AddDays(daysOffset);

         return Configuration.WorkDays.FirstOrDefault(wd => wd.EffectiveDate.Date == currentDate.Date);
      }

      private TimeSpan CalculateTime(int column)
      {
         int totalMinutes = column * 30;
         int minutesInDay = totalMinutes % (24 * 60);
         int hour = minutesInDay / 60;
         int minute = minutesInDay % 60;

         return new TimeSpan(hour, minute, 0);
      }

      private int GetColumnFromDateTime(DateTime date, TimeSpan time)
      {
         int daysOffset = (date.Date - Configuration.StartDate.Date).Days;
         int totalMinutes = (daysOffset * 24 * 60) + (time.Hours * 60) + time.Minutes;
         return totalMinutes / 30;
      }

      public async Task<Chromosome> RunGeneticAlgorithmAsync()
      {
         await GeneratePopulation();
         this.SortPopulation();

         for (int generation = 0; generation < Configuration.Generations; generation++)
         {

            Configuration.Population = EvolveGeneration(Configuration);

            if (generation % 100 == 0)
            {
               Console.WriteLine($"Generation {generation}: Best fitness = {Configuration.Population.Max(e => e.Fitness)}");
            }

         }
         Chromosome bestChromosome = Configuration.Population.OrderByDescending(c => FitnessFunction(c)).FirstOrDefault();
         return bestChromosome;
      }

      private List<Chromosome> EvolveGeneration(ConfigurationSchedule configuration)
      {
         var newPopulation = new List<Chromosome>(configuration.PopulationSize);

         // Elitism: keeps the top 10% of individuals
         var bestIndividuals = configuration.Population.OrderByDescending(c => FitnessFunction(c)).Take(configuration.PopulationSize / 10).ToList();
         newPopulation.AddRange(bestIndividuals.Select(c => c.Clone()));

         while (newPopulation.Count < configuration.PopulationSize)
         {
            // Select parents using tournament selection
            var parent1 = TournamentSelection(configuration.Population);
            var parent2 = TournamentSelection(configuration.Population);

            // Crossover
            // var child = Crossover(parent1, parent2);
            var child = parent1.Clone();

            // Mutation
            child = Mutate(child);

            //Evaluete fitness
            FitnessFunction(child);

            // Add child to new Population
            newPopulation.Add(child);
         }

         return newPopulation;
      }

      private Chromosome TournamentSelection(List<Chromosome> populacao)
      {
         var torneio = populacao.OrderBy(x => Configuration.Random.Next()).Take(5).ToList();
         return torneio.OrderByDescending(e => e.Fitness).First().Clone();
      }

      public async Task GeneratePopulation()
      {
         List<Task<Chromosome>> tasks = new List<Task<Chromosome>>(Configuration.PopulationSize);
         List<Chromosome> chromosomes = new List<Chromosome>(Configuration.PopulationSize);

         for (int p = 0; p < Configuration.PopulationSize; p++)
         {
            // tasks.Add(Task.Run(() => this.GenerateChromosome()));
            chromosomes.Add(this.GenerateChromosome());
         }

         // var chromosomes = await Task.WhenAll(tasks);

         Configuration.Population = new List<Chromosome>(chromosomes);
      }

      private Chromosome GenerateChromosome()
      {
         Chromosome chromosome = new Chromosome(Configuration.RowsSize, Configuration.ColumnsSize);

         for (int row = 0; row < Configuration.RowsSize; row++)
         {
            int workload = Configuration.Employees[row].JobPosition?.Workload ?? 0; // Workload in hours
            int totalSlots = workload * 2; // Convert hours to 30-minute slots

            for (int day = 0; day < Configuration.WorkDays.Count(); day++)
            {
               WorkDay dayWork = Configuration.WorkDays[day];
               OperatingSchedule schedule = dayWork.OperatingSchedule;
               // Calculate the start and end columns for the current day
               int startColumn = GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
               int endColumn = GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);
               int limitSlot = endColumn - totalSlots;

               int startSlot = Configuration.Random.Next(startColumn, limitSlot + 1); // Randomly select a column within the working hours

               for (int column = startSlot; column < startSlot + totalSlots && column < endColumn; column++)
               {
                  chromosome.Gene[row, column] = 1;
               }
            }
         }
         
         return chromosome;
      }

      private Chromosome Mutate(Chromosome chromosome)
      {
         for (int row = 0; row < Configuration.RowsSize; row++)
         {
            int workload = Configuration.Employees[row].JobPosition?.Workload ?? 0; // Workload in hours
            int totalSlots = workload * 2; // Convert hours to 30-minute slots

            for (int day = 0; day < Configuration.WorkDays.Count(); day++)
            {
               if (Configuration.Random.NextDouble() < Configuration.MutationRate)
               {

                  WorkDay dayWork = Configuration.WorkDays[day];
                  OperatingSchedule schedule = dayWork.OperatingSchedule;
                  // Calculate the start and end columns for the current day
                  int startColumn = GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
                  int endColumn = GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);
                  int limitSlot = endColumn - totalSlots;

                  int startSlot = Configuration.Random.Next(startColumn, limitSlot + 1); // Randomly select a column within the working hours

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
         }
         return chromosome;
      }

      private Chromosome Crossover(Chromosome parent1, Chromosome parent2)
      {
         Chromosome child = new(Configuration.RowsSize, Configuration.ColumnsSize);

         for (int row = 0; row < Configuration.RowsSize; row++)
         {
            for (int day = 0; day < Configuration.WorkDays.Count(); day++)
            {
               var escolherDoPai1 = Configuration.Random.Next(2) == 0;
               var paiEscolhido = escolherDoPai1 ? parent1 : parent2;

               WorkDay dayWork = Configuration.WorkDays[day];
               OperatingSchedule schedule = dayWork.OperatingSchedule;
               // Calculate the start and end columns for the current day
               int startColumn = GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
               int endColumn = GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);

               for (int column = startColumn; column < endColumn; column++)
               {
                  child.Gene[row, column] = paiEscolhido.Gene[row, column];
               }
            }
         }

         return child;
      }

      private int FitnessFunction(Chromosome chromosome)
      {
         if (chromosome.calculated == false)
         {
            chromosome.Fitness = CalculateFitness(chromosome);
            chromosome.calculated = true;
         }

         return chromosome.Fitness;
      }

      private int CalculateFitness(Chromosome chromosome)
      {
         int fitness = 100000; // Start with a high fitness value

         for (int row = 0; row < Configuration.RowsSize; row++)
         {
            int successiveDays = 0;

            for (int days = 0; days < Configuration.WorkDays.Count(); days++)
            {
               WorkDay dayWork = Configuration.WorkDays[days];
               OperatingSchedule schedule = dayWork.OperatingSchedule;
               // Calculate the start and end columns for the current day
               int startColumn = GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
               int endColumn = GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);

               int column = startColumn;

               while (column < endColumn && chromosome.Gene[row, column] == 0)
               {
                  column++;
               }

               if (chromosome.Gene[row, column - 1] == 1 && column < endColumn)
               {
                  successiveDays++; // Decrease fitness for each working hour within the schedule
                  if (successiveDays == 7)
                  {
                     fitness -= 20;
                     successiveDays = 0;
                  }
               }
            }
         }

         return fitness;
      }

      public void printMatrixList()
      {
         List<Chromosome> best = Configuration.Population.OrderByDescending(c => FitnessFunction(c)).Take(1).ToList();

         for (int i = 0; i < best.Count; i++)
         {
            Console.WriteLine($"Matriz {i + 1}:");
            Console.WriteLine($"Fitness {best[i].Fitness}:");
            Console.WriteLine($"Calculated {best[i].calculated}:");
            this.printMatrix(best[i].Gene);
            this.printSchedule(best[i].Gene);
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

      private void printSchedule(int[,] matrix)
      {
         int workingHours;
         int rows = matrix.GetLength(0);
         int cols = matrix.GetLength(1);

         for (int i = 0; i < rows; i++)
         {
            workingHours = 0;

            Console.Write("Employee " + Configuration.Employees[i].Name + ": ");
            Console.WriteLine();
            // Console.Write("           ");


            for (int j = 0; j < cols; j++)
            {
               TimeSpan currentTime = this.CalculateTime(j);

               if (matrix[i, j] == 1)
                  workingHours++;

               if ((matrix[i, j] == 1 && matrix[i, j - 1] == 0) || (matrix[i, j] == 1 && j == 0))
               {
                  Console.Write("           ");
                  Console.Write("Working Hours: ( " + currentTime.ToString().Substring(0, 5) + " - ");
                  // Console.Write("( " + (currentTime - new TimeSpan(0, 30, 0)).ToString().Substring(0, 5) + " - ");
                  if (matrix[i, j + 1] == 0 || j == cols - 1)
                  {
                     Console.Write((currentTime + new TimeSpan(0, 30, 0)).ToString().Substring(0, 5) + ") ");
                  }
               }
               else if ((matrix[i, j] == 1 && matrix[i, j + 1] == 0) || (matrix[i, j] == 1 && j == cols - 1))
               {
                  Console.Write(currentTime.ToString().Substring(0, 5) + ") ");
               }

               if ((j % 48 == 0 && j != 0) || j == cols - 1)
               {
                  Console.Write(" " + new TimeSpan(workingHours / 2, (workingHours % 2) * 30, 0).ToString().Substring(0, 5));
                  Console.WriteLine();
                  // if (j != cols - 1)
                  //    Console.Write("           ");

                  workingHours = 0;
               }
            }
            Console.WriteLine();
         }
      }
   }
}