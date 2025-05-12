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
      this.SortPopulation();

      int generation = 0;
      // for (generation = 0; generation < Configuration.Generations && Configuration.Population[0].Fitness < Configuration.MaxFitness; generation++)
      for (generation = 0; Configuration.Population[0].Fitness < Configuration.MaxFitness; generation++)
      {

         Configuration.Population = EvolveGeneration(Configuration);

         if (generation % 100 == 0)
         {
            Console.WriteLine($"Generation {generation}: Best fitness = {Configuration.Population[0].Fitness}");
         }

      }
      Console.WriteLine("==========================");
      Console.WriteLine("Generation finalizada: " + generation);
      Console.WriteLine("==========================");

      Chromosome bestChromosome = Configuration.Population.OrderByDescending(c => FitnessFunction(c)).FirstOrDefault();
      return bestChromosome;
   }

   private List<Chromosome> EvolveGeneration(ConfigurationSchedule configuration)
   {
      var newPopulation = new List<Chromosome>(configuration.PopulationSize);

      // Elitism: keeps the top 10% of individuals
      var bestIndividuals = configuration.Population.OrderByDescending(c => FitnessFunction(c)).Take(configuration.PopulationSize / 10).ToList();
      newPopulation.AddRange(bestIndividuals.Select(c => c.Clone()));

      var tasks = new List<Task<Chromosome>>();

      while (newPopulation.Count + tasks.Count < configuration.PopulationSize)
      {
         tasks.Add(Task.Run(() =>
         {
            // Select parents using tournament selection
            var parent1 = TournamentSelection(configuration.Population);
            var parent2 = TournamentSelection(configuration.Population);

            // Crossover
            var child = CrossOver.Run(configuration, parent1, parent2);

            // Mutation
            this.Mutate.Run(configuration, child);

            // Evaluate fitness
            FitnessFunction(child);

            return child;
         }));
      }

      if (tasks.Count > 0)
      {
         var children = Task.WhenAll(tasks).GetAwaiter().GetResult();
         newPopulation.AddRange(children);
      }

      return newPopulation;
   }

   private Chromosome TournamentSelection(List<Chromosome> populacao)
   {
      var torneio = populacao.OrderBy(x => Configuration.Random.Next()).Take(5).ToList();
      return torneio.OrderByDescending(e => e.Fitness).First().Clone();
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

      // var interruptDaysTask = Task.Run(() => this.InterruptDays(configuration, chromosome));
      // var validateSameStartWorkingTask = Task.Run(() => this.ValidateSameStartWorking(configuration, chromosome));
      // var validateJobPositionLimitsTask = Task.Run(() => this.ValidateJobPositionLimits(configuration, chromosome));

      // Task.WaitAll(interruptDaysTask, validateSameStartWorkingTask, validateJobPositionLimitsTask);

      // fitness += interruptDaysTask.Result;
      // fitness += validateSameStartWorkingTask.Result;
      // fitness += validateJobPositionLimitsTask.Result;

      fitness += this.InterruptDays(configuration, chromosome);
      fitness += this.ValidateSameStartWorking(configuration, chromosome);
      fitness += this.ValidateJobPositionLimits(configuration, chromosome) * 10;

      return fitness;
   }

   private int InterruptDays(ConfigurationSchedule configuration, Chromosome chromosome)
   {
      int fitness = 0;

      //7 dias interruptos de trabalho
      for (int row = 0; row < configuration.RowsSize; row++)
      {
         int successiveDays = 0;
         Employee employee = configuration.Employees[row];

         for (int days = 0; days < configuration.WorkDays.Count(); days++)
         {
            WorkDay dayWork = configuration.WorkDays[days];
            OperatingSchedule schedule = dayWork.OperatingSchedule;
            // Calculate the start and end columns for the current day
            int startColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.Start);
            int endColumn = configuration.GetColumnFromDateTime(dayWork.EffectiveDate, schedule.End);

            int column = startColumn;

            while (column < endColumn && chromosome.Gene[row, column] == 0)
            {
               column++;
            }

            if (chromosome.Gene[row, column - 1] == 1 && column < endColumn)
            {
               successiveDays++; // Decrease fitness for each working hour within the schedule
               if (successiveDays == employee.JobPosition.MaximumConsecutiveDays)
               {
                  fitness -= 20;
                  successiveDays = 0;
               }
            }
         }
      }

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

               if (count < countEmployees / 2)
               {
                  // Penaliza para cada funcionário abaixo do mínimo necessário
                  fitness -= jobPosition.Workload * 2 * 3;
               }
            }
         }

      }

      return fitness;
   }



   // public Dictionary<Employee, List<int>> GenerateRestDaysPerEmployee()
   // {
   //    var restDays = new Dictionary<Employee, List<int>>();
   //    int totalDays = Configuration.WorkDays.Length;
   //    int maxConsecutive = 6;

   //    // Agrupa funcionários por JobPosition
   //    var jobGroups = Configuration.Employees
   //       .GroupBy(e => e.JobPosition!.Name)
   //       .ToDictionary(g => g.Key, g => g.ToList());

   //    foreach (var group in jobGroups)
   //    {
   //       int jobCount = group.Value.Count;
   //       for (int i = 0; i < jobCount; i++)
   //       {
   //          var employee = group.Value[i];
   //          var employeeRestDays = new List<int>();

   //          // Considera indisponibilidades do funcionário
   //          if (employee.Unavailabilities != null)
   //          {
   //             foreach (var un in employee.Unavailabilities)
   //             {
   //                // Marca todos os dias do período de indisponibilidade como folga
   //                for (int day = 0; day < totalDays; day++)
   //                {
   //                   var workDay = Configuration.WorkDays[day];
   //                   if (workDay.EffectiveDate.Date >= un.Start.Date && workDay.EffectiveDate.Date <= un.End.Date)
   //                   {
   //                      employeeRestDays.Add(day);
   //                   }
   //                }
   //             }
   //          }

   //          // Garante que não exceda 6 dias seguidos
   //          int lastRest = -maxConsecutive - 1;
   //          employeeRestDays = employeeRestDays.Distinct().OrderBy(d => d).ToList();
   //          int dayIndex = 0;
   //          while (dayIndex < totalDays)
   //          {
   //             if (employeeRestDays.Contains(dayIndex))
   //             {
   //                lastRest = dayIndex;
   //                dayIndex++;
   //                continue;
   //             }

   //             if (dayIndex - lastRest > maxConsecutive)
   //             {
   //                // Offset para evitar folga de todos no mesmo dia
   //                int offset = i % maxConsecutive;
   //                int restDay = Math.Min(dayIndex + offset, totalDays - 1);

   //                // Evita sobrepor folga já marcada
   //                while (employeeRestDays.Contains(restDay) && restDay < totalDays - 1)
   //                   restDay++;

   //                employeeRestDays.Add(restDay);
   //                lastRest = restDay;
   //                dayIndex = restDay + 1;
   //             }
   //             else
   //             {
   //                dayIndex++;
   //             }
   //          }

   //          employeeRestDays = employeeRestDays.Distinct().OrderBy(d => d).ToList();
   //          restDays[employee] = employeeRestDays;
   //       }
   //    }

   //    return restDays;
   // }

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

   public void printSchedule(int[,] matrix)
   {
      int workingHours;
      int rows = matrix.GetLength(0);
      int cols = matrix.GetLength(1);

      for (int i = 0; i < rows; i++)
      {
         workingHours = 0;

         Employee employee = Configuration.Employees[i];
         JobPosition jobPosition = employee.JobPosition;

         Console.WriteLine("Employee: " + Configuration.Employees[i].Name);
         Console.WriteLine("Job: " + jobPosition.Name);
         // Console.Write("           ");


         for (int j = 0; j < cols; j++)
         {
            TimeSpan currentTime = this.CalculateTime(j);

            if (matrix[i, j] == 1)
               workingHours++;

            if ((matrix[i, j] == 1 && matrix[i, j - 1] == 0) || (matrix[i, j] == 1 && j == 0))
            {
               Console.Write(Configuration.GetDateAndWorkDayFromColumn(j).EffectiveDate.ToString("dd/MM/yyyy") + ": ");
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

   public Dictionary<Employee, List<WorkDay>> GenerateEmployeeDaysOff(
        List<Employee> employees,
        List<WorkDay> workDays)
   {
      // Ordenar os dias de trabalho por data
      var sortedWorkDays = workDays.OrderBy(w => w.EffectiveDate).ToList();

      // Dicionário para armazenar os dias de folga de cada funcionário
      var employeeDaysOff = new Dictionary<Employee, List<WorkDay>>();

      // Inicializar o dicionário para cada funcionário
      foreach (var employee in employees)
      {
         employeeDaysOff[employee] = new List<WorkDay>();
      }

      // Agrupar funcionários por cargo
      var employeesByPosition = employees.GroupBy(e => e.JobPosition.Id)
                                       .ToDictionary(g => g.Key, g => g.ToList());

      // Para cada dia de trabalho, distribuir as folgas
      for (int i = 0; i < sortedWorkDays.Count; i++)
      {


         // Processar cada grupo de cargo separadamente
         foreach (var positionGroup in employeesByPosition)
         {
            var positionEmployees = positionGroup.Value;
            // var sameJobPi = CountEmployeesByJobPosition(JobPosition jobPosition, List<Employee> employees);
            var maxSameDayOff = Math.Max(1, positionEmployees.Count / 3); // Máximo ~30% do grupo pode folgar junto

            // Encontrar funcionários deste cargo que precisam de folga
            var candidates = positionEmployees.Where(emp =>
                NeedsDayOff(emp, employeeDaysOff, sortedWorkDays, i)).ToList();

            // Se muitos precisam de folga, selecionar os que mais precisam
            // if (candidates.Count > maxSameDayOff)
            // {
            //    candidates = candidates.OrderByDescending(emp =>
            //        ConsecutiveWorkDays(emp, employeeDaysOff, sortedWorkDays, i))
            //        .Take(maxSameDayOff)
            //        .ToList();
            // }

            // Atribuir folgas
            int candidateIndex = 0;
            foreach (var employee in candidates)
            {
               // if (candidateIndex < maxSameDayOff)
               // {
               var currentDay = sortedWorkDays[i - candidateIndex];
               employeeDaysOff[employee].Add(currentDay);
               // }
               candidateIndex++;
            }
         }
      }

      return employeeDaysOff;
   }

   private bool NeedsDayOff(
       Employee employee,
       Dictionary<Employee, List<WorkDay>> employeeDaysOff,
       List<WorkDay> workDays,
       int currentDayIndex)
   {
      // Verificar se o funcionário já trabalhou 6 dias consecutivos
      var consecutiveDays = ConsecutiveWorkDays(employee, employeeDaysOff, workDays, currentDayIndex);
      return consecutiveDays >= 7;
   }

   private int ConsecutiveWorkDays(
       Employee employee,
       Dictionary<Employee, List<WorkDay>> employeeDaysOff,
       List<WorkDay> workDays,
       int currentDayIndex)
   {
      int consecutiveDays = 0;

      // Retroceder a partir do dia atual para contar dias trabalhados consecutivos
      for (int i = currentDayIndex; i >= 0; i--)
      {
         var day = workDays[i];

         // Se o funcionário está de folga neste dia, parar a contagem
         if (employeeDaysOff[employee].Contains(day))
            break;

         consecutiveDays++;

         // Parar após verificar 7 dias (o máximo que nos interessa)
         if (consecutiveDays >= 7)
            break;
      }

      return consecutiveDays;
   }
}
