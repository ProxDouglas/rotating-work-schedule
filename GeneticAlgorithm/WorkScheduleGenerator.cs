namespace rotating_work_schedule.GeneticAlgorithm;

using rotating_work_schedule.GeneticAlgorithm.Processes;
using rotating_work_schedule.Models;

public class WorkScheduleGenerator
{
   private ConfigurationSchedule Configuration { get; set; }
   private GeneratePopulation GeneratePopulation { get; set; }
   private Mutate Mutate { get; set; }
   private CrossOver CrossOver { get; set; }

   public WorkScheduleGenerator(ConfigurationSchedule configurationSchedule)
   {
      Configuration = configurationSchedule;
      this.GeneratePopulation = new GeneratePopulation();
      this.Mutate = new Mutate();
      this.CrossOver = new CrossOver();
   }

   public void SortPopulation()
   {
      Configuration.Population.Sort((a, b) => FitnessFunction(b).CompareTo(FitnessFunction(a)));
   }

   public List<int[,]> getBestSchedules()
   {
      return Configuration.Population.OrderByDescending(c => FitnessFunction(c)).Take(3).Select(c => c.Gene).ToList();
   }

   private TimeSpan CalculateTime(int column)
   {
      int totalMinutes = column * 30;
      int minutesInDay = totalMinutes % (24 * 60);
      int hour = minutesInDay / 60;
      int minute = minutesInDay % 60;

      return new TimeSpan(hour, minute, 0);
   }

   public async Task<Chromosome> RunGeneticAlgorithmAsync()
   {
      this.Configuration.Population = await this.GeneratePopulation.Run(this.Configuration);
      // this.SortPopulation();

      int generation = 0;
      // for (generation = 0; generation < Configuration.Generations && Configuration.Population[0].Fitness < Configuration.MaxFitness; generation++)
      while (Configuration.Population[0].Fitness < Configuration.MaxFitness && generation < Configuration.Generations)
      {
         await this.CalculationFitness(Configuration);
         Configuration.Population = await EvolveGeneration(Configuration, generation);

         // if (generation % 100 == 0)
         // {
         //    Console.WriteLine($"Generation {generation}: Best fitness = {Configuration.Population[0].Fitness}");
         // }

         generation++;
         Configuration.GenarationValue = generation;
      }
      // Console.WriteLine("==========================");
      // Console.WriteLine("Generation finalizada: " + generation);
      // Console.WriteLine("==========================");

      Chromosome bestChromosome = Configuration.Population.OrderByDescending(c => FitnessFunction(c)).First();
      // PrintConsole printConsole = new PrintConsole();
      // printConsole.printMatrixList(Configuration.Population.OrderByDescending(c => FitnessFunction(c)).Take(1).ToList(), Configuration.Employees, Configuration.WorkDays[0].EffectiveDate, Configuration.WorkDays.ToList());
      return bestChromosome;
   }

   private async Task CalculationFitness(ConfigurationSchedule configuration)
   {
      var tasks = configuration.Population.Select(chromosome => Task.Run(() => FitnessFunction(chromosome)));
      await Task.WhenAll(tasks);
   }

   private async Task<List<Chromosome>> EvolveGeneration(ConfigurationSchedule configuration, int generation)
   {
      var newPopulation = new List<Chromosome>(configuration.PopulationSize);

      // Elitism: keeps the top 10% of individuals
      var bestIndividuals = configuration.Population.OrderByDescending(c => c.Fitness).Take(10).ToList();
      newPopulation.AddRange(bestIndividuals.Select(c => c.Clone()));

      var tasks = new List<Task<Chromosome>>();

      while (newPopulation.Count + tasks.Count < configuration.PopulationSize)
      {

         var child = Task.Run(() => GenarateChild(configuration, generation));
         tasks.Add(child);

         // var child = await GenarateChild(configuration, generation);
         // newPopulation.Add(child);
      }

      if (tasks.Count > 0)
      {
         var children = await Task.WhenAll(tasks);
         newPopulation.AddRange(children);
      }

      return newPopulation;
   }

   private Chromosome GenarateChild(ConfigurationSchedule configuration, int generation)
   {
      // Select parents using tournament selection
      var parent1 = TournamentSelection(configuration.Population);
      var parent2 = TournamentSelection(configuration.Population);

      // Crossover
      Chromosome child = CrossOver.Run(configuration, parent1, parent2);
      var childClone = child.Clone();

      // Mutation
      this.Mutate.Run(configuration, childClone);

      // Evaluate fitness
      FitnessFunction(child);
      FitnessFunction(childClone);

      if (childClone.Fitness > child.Fitness)
      {
         child = childClone;
      }
      else if (childClone.Fitness == child.Fitness && (child.Fitness > parent1.Fitness || child.Fitness > parent2.Fitness))
      {
         child = childClone;
      }
      else if (childClone.Fitness == child.Fitness && child.Fitness > parent1.Fitness && child.Fitness > parent2.Fitness)
      {
         // newPopulation.Add(childClone);
         child = childClone;
      }
      else
      {
         var newCromosome = this.GeneratePopulation.GenerateChromosome(configuration);
         FitnessFunction(newCromosome);
         child = newCromosome;
      }

      return child;
   }

   private Chromosome TournamentSelection(List<Chromosome> populacao)
   {
      List<Chromosome> tornament = [];
      for (int i = 0; i < 3; i++)
      {
         int randomIndex = Configuration.Random.Next(0, populacao.Count);
         tornament.Add(populacao[randomIndex]);
      }
      return tornament.OrderByDescending(e => e.Fitness).First().Clone();
   }

   private int FitnessFunction(Chromosome chromosome)
   {
      if (chromosome.calculated == false)
      {
         chromosome.Fitness = CalculateFitness(Configuration, chromosome);
         chromosome.calculated = true;
      }

      return chromosome.Fitness;
   }

   private int CalculateFitness(ConfigurationSchedule configuration, Chromosome chromosome)
   {
      int fitness = configuration.MaxFitness; // Start with a high fitness value

      // var tasks = new List<Task<Chromosome>>();

      // var validateSameStartWorkingTask = Task.Run(() => this.ValidateSameStartWorking(configuration, chromosome));
      // var validateJobPositionLimitsTask = Task.Run(() => this.ValidateJobPositionLimits(configuration, chromosome));

      // Task.WaitAll(interruptDaysTask, validateSameStartWorkingTask, validateJobPositionLimitsTask);

      // fitness += interruptDaysTask.Result;
      // fitness += validateSameStartWorkingTask.Result;
      // fitness += validateJobPositionLimitsTask.Result;

      // fitness += this.ValidateSameStartWorking(configuration, chromosome);
      fitness += this.ValidateJobPositionLimits(configuration, chromosome) * 10;

      return fitness;
   }

   private int ValidateSameStartWorking(ConfigurationSchedule configuration, Chromosome chromosome)
   {
      int fitness = 0;

      for (int row = 0; row < configuration.RowsSize; row++)
      {
         List<int> startWorkColumn = new List<int>();

         for (int day = 0; day < configuration.WorkDays.Count(); day++)
         {
            bool startWork = false;

            WorkDay dayWork = configuration.WorkDays[day];
            OperatingSchedule schedule = dayWork.OperatingSchedule;

            // Calculate the start and end columns for the current day
            int startColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
            int endColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);

            int column = startColumn;
            while (column < endColumn && startWork == false)
            {
               if (chromosome.Gene[row, column] == 1)
               {
                  startWorkColumn.Add(column);
                  startWork = true;
               }
               else if (column < configuration.ColumnsSize && chromosome.Gene[row, column] == 0 && chromosome.Gene[row, column + 1] == 1)
               {
                  startWorkColumn.Add(column + 1);
                  startWork = true;
               }
               column++;
            }
         }

         if (startWorkColumn.Count > 0)
         {
            TimeSpan startHour = this.CalculateTime(startWorkColumn[0]);

            for (int start = 1; start < startWorkColumn.Count; start++)
            {
               var startHour2 = this.CalculateTime(startWorkColumn[start]);

               if (startHour2 != startHour)
               {
                  fitness -= 5; // Decrease fitness for each working hour within the schedule
               }
            }
         }
      }

      return fitness; // All work hours match the operating schedule
   }

   private int ValidateJobPositionLimits(ConfigurationSchedule configuration, Chromosome chromosome)
   {
      int fitness = 0;

      for (int day = 0; day < configuration.WorkDays.Count(); day++)
      {
         WorkDay dayWork = configuration.WorkDays[day];
         OperatingSchedule schedule = dayWork.OperatingSchedule;

         // Calculate the start and end columns for the current day
         int startColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
         int endColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);

         for (int column = startColumn; column < endColumn; column++)
         {
            var jobPositionCounts = new Dictionary<string, int>();
            var jobPositionEmployeesCounts = new Dictionary<string, int>();

            //add the number of workers at the same time
            for (int row = 0; row < configuration.RowsSize; row++)
            {
               var jobPosition = configuration.Employees[row].JobPosition;

               if (!jobPositionEmployeesCounts.ContainsKey(jobPosition.Name))
                  jobPositionEmployeesCounts[jobPosition.Name] = 0;
               jobPositionEmployeesCounts[jobPosition.Name]++;

               if (chromosome.Gene[row, column] == 1)
               {
                  if (!jobPositionCounts.ContainsKey(jobPosition.Name))
                     jobPositionCounts[jobPosition.Name] = 0;
                  jobPositionCounts[jobPosition.Name]++;
               }
            }

            foreach (var jobPosition in configuration.JobPositions)
            {
               jobPositionEmployeesCounts.TryGetValue(jobPosition.Name, out int countEmployees);
               jobPositionCounts.TryGetValue(jobPosition.Name, out int count);

               // Conta quantos colaboradores desse jobPosition estão de folga neste turno
               int offCount = 0;
               foreach (var employee in configuration.Employees.Where(e => e.JobPosition.Name == jobPosition.Name))
               {
                  if (employee.WorkOffs.Any(wd => wd.EffectiveDate == dayWork.EffectiveDate))
                  {
                     offCount++;
                  }
               }

               int effeiveEmployeeCount = (countEmployees - offCount) / 2;

               if (count < effeiveEmployeeCount)
               {
                  // Penaliza para cada funcionário abaixo do mínimo necessário
                  fitness -= (effeiveEmployeeCount - count) * 2 * 3;
               }
            }
         }

      }

      return fitness;
   }
}
