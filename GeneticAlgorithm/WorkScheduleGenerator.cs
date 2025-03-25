using rotating_work_schedule.Models;

namespace rotating_work_schedule.GeneticAlgorithm
{
   public class WorkScheduleGenerator
   {
      const double MutationRate = 0.2;

      private Employee[] Employees { get; set; }
      private OperatingSchedule[] OperatingSchedule { get; set; }
      private const int PopulationSize = 100;
      private const int LocalPopulation = 5;
      private int Days = 1;
      private readonly Random random = new Random();
      private int columnsSize = 0;
      private int rowsSize = 0;
      public List<Chromosome> population = new List<Chromosome>();
      private DateTime startDate;

      public WorkScheduleGenerator(Employee[] employees, OperatingSchedule[] operatingSchedules, DateTime? startDate = null)
      {
         Employees = employees.OrderBy(emp => emp?.JobPosition?.Id).ToArray();
         OperatingSchedule = operatingSchedules.OrderBy(os => os.DayOfWeek).ToArray();
         this.rowsSize = this.Employees.Count();
         this.columnsSize = this.Days * 24 * 2;
         this.startDate = startDate ?? DateTime.Now;
      }

      private int GetMaxFitnees()
      {
         return 25600 * this.Employees.Count() * this.Days;
      }

      public void SortPopulation()
      {
         this.population.Sort((a, b) => FitnessFunction(b).CompareTo(FitnessFunction(a)));
      }

      public List<int[,]> getBestSchedules()
      {
         return this.population.OrderByDescending(c => FitnessFunction(c)).Take(3).Select(c => c.Gene).ToList();
      }

      private DayOfWeek getDayOfWeekFromColumn(int column)
      {
         int totalMinutes = column * 30;
         int daysOffset = totalMinutes / (24 * 60);
         DateTime currentDate = startDate.AddDays(daysOffset);
         return currentDate.DayOfWeek;
      }

      private TimeSpan CalculateTime(int column)
      {
         int totalMinutes = column * 30;
         int minutesInDay = totalMinutes % (24 * 60);
         int hour = minutesInDay / 60;
         int minute = minutesInDay % 60;

         return new TimeSpan(hour, minute, 0);
      }

      private Boolean IsWithinWorkingHours(int column)
      {
         OperatingSchedule schedule = OperatingSchedule[(int)getDayOfWeekFromColumn(column)];

         if (schedule != null)
         {
            TimeSpan currentTime = this.CalculateTime(column);

            if (currentTime > schedule.Start && currentTime <= schedule.End)
               return true;
         }
         return false;
      }

      public async Task RunGeneticAlgorithmAsync(int generations)
      {
         bool stopProcess = false;
         int generation = 0;

         await generatePopulation();
         this.SortPopulation();

         while (!stopProcess && generation < generations)
         {

            for (int i = 0; i < PopulationSize; i++)
            {
               Chromosome parent1 = this.population[i];
               Chromosome parent2 = this.population[PopulationSize - i - 1];

               Chromosome child;
               Chromosome child2;

               child = Crossover(parent1, parent2);
               child2 = Crossover(parent1, parent2);

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
                  this.population.Add(child2);
               }
            }

            this.SortPopulation();
            if (this.population.Count > PopulationSize)
            {
               this.population.RemoveRange(PopulationSize, this.population.Count - PopulationSize);

               int teste = this.GetMaxFitnees();
               // if (this.population.Any(c => c.Fitness == this.GetMaxFitnees()))
               // {
               //    Console.WriteLine("Generation: " + generation);
               //    stopProcess = true;
               // }
            }

            // if (this.population.Count <= PopulationSize)
            // {
            //    int length = PopulationSize - this.population.Count;
            //    List<Task> tasks = new List<Task>(length);
            //    for (int i = 0; i <= length; i++)
            //    {
            //       tasks.Add(Task.Run(() => this.population.Add(generateChromosome())));
            //    }

            //    await Task.WhenAll(tasks);
            // }

            generation++;
         }
      }

      public async Task generatePopulation()
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
         // int group = -1;

         for (int row = 0; row < this.rowsSize; row++)
         {
            // group = -1;

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

               // if (group > -1 && isWorkingHour)
               // {
               //    chromosome.Gene[row, column] = 1;
               //    group++;
               //    if (group == 4)
               //    {
               //       group = -1;
               //    }
               // }
               // else if (isWorkingHour)
               // {
               //    chromosome.Gene[row, column] = random.Next(0, 2);
               //    if (chromosome.Gene[row, column] == 1)
               //       group++;
               // }
               // else
               // {
               //    chromosome.Gene[row, column] = 0;
               // }
            }
         }

         return chromosome;
      }

      private Chromosome Mutate(Chromosome chromosome)
      {
         for (int row = 0; row < rowsSize; row++)
         {
            for (int column = 0; column < columnsSize; column++)
            {
               if (this.IsWithinWorkingHours(column) && random.NextDouble() < MutationRate)
               {
                  // if (chromosome.Gene[row, column] == 0)
                  // {
                  //    if (column < columnsSize && chromosome.Gene[row, column + 1] == 1)
                  //    {
                  //       chromosome.Gene[row, column] = 1;
                  //    }
                  //    else if (column > 0 && chromosome.Gene[row, column - 1] == 1)
                  //    {
                  //       chromosome.Gene[row, column] = 1;
                  //    }
                  //    else if (column > 0 && column < columnsSize && chromosome.Gene[row, column - 1] == 0 && chromosome.Gene[row, column + 1] == 0)
                  //    {
                  //       chromosome.Gene[row, column] = 0;
                  //    }
                  // }
                  // else if (chromosome.Gene[row, column] == 1)
                  // {
                  //    if (column < columnsSize && chromosome.Gene[row, column + 1] == 0)
                  //    {
                  //       chromosome.Gene[row, column] = 0;
                  //    }
                  //    else if (column > 0 && chromosome.Gene[row, column - 1] == 0)
                  //    {
                  //       chromosome.Gene[row, column] = 0;
                  //    }
                  //    else if (column > 0 && column < columnsSize && chromosome.Gene[row, column - 1] == 1 && chromosome.Gene[row, column + 1] == 1)
                  //    {
                  //       chromosome.Gene[row, column] = 1;
                  //    }
                  // }
                  chromosome.Gene[row, column] = chromosome.Gene[row, column] == 0 ? 1 : 0;
               }
            }
         }
         return chromosome;
      }

      private Chromosome Crossover(Chromosome parent1, Chromosome parent2)
      {
         Chromosome child = new(rowsSize, columnsSize);
         int parent1Fitness = FitnessFunction(parent1);
         int parent2Fitness = FitnessFunction(parent2);

         for (int f = 0; f < rowsSize; f++)
         {
            for (int t = 0; t < columnsSize; t++)
            {
               if (parent1Fitness > parent2Fitness)
               {
                  child.Gene[f, t] = random.NextDouble() < 0.7 ? parent1.Gene[f, t] : parent2.Gene[f, t];
               }
               else
               {
                  child.Gene[f, t] = random.NextDouble() < 0.7 ? parent2.Gene[f, t] : parent1.Gene[f, t];
               }
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
         int consecutiveTimes;
         int groupedHours;
         int start;
         int end;

         for (int row = 0; row < rowsSize; row++)
         {

            hoursWorked = 0;
            groupedHours = 0;
            consecutiveTimes = 0;
            start = 0;
            end = 0;

            for (int column = 0; column < columnsSize; column++)
            {
               bool isWorkingHour = IsWithinWorkingHours(column);

               if (isWorkingHour)
               {
                  // consecutiveTimes += ConsecutiveTimes(chromosome, row, column);
                  // groupedHours += GroupedHours(chromosome, row, column);

                  if (chromosome.Gene[row, column] == 1)
                     end = column;

                  if (start == 0 && chromosome.Gene[row, column] == 1)
                     start = column;

                  if (chromosome.Gene[row, column] == 1)
                     hoursWorked += 1;
               }
               else if ((column > 0 && column % 48 == 0) || (column + 1 == columnsSize))
               {

                  fitness += ValueWorkHours(row, hoursWorked);
                  fitness += ValueWorkHours(row, Math.Abs(start - end) + 1); // analyzes the distance between the start and end hours worked
                  fitness += WorkShiftSchedule(chromosome, row, column) * 5;
                  // fitness += groupedHours;
                  hoursWorked = 0;
                  consecutiveTimes = 0;
                  groupedHours = 0;
                  start = 0;
                  end = 0;
               }
            }
         }

         return fitness;
      }

      // private int ConsecutiveTimes(Chromosome chromosome, int row, int column)
      // {
      //    if (chromosome.Gene[row, column] == 1 && (column < columnsSize - 1 || chromosome.Gene[row, column + 1] == 1 || chromosome.Gene[row, column - 1] == 1))
      //    {
      //       return 1; // More points for consecutive 1s
      //    }
      //    return 0;
      // }

      private int GroupedHours(Chromosome chromosome, int row, int column)
      {
         // if (chromosome.Gene[row, column] == 1 && (column < columnsSize - 1 || chromosome.Gene[row, column + 1] == 1 || chromosome.Gene[row, column - 1] == 1))
         // {
         //    return 5; // More points for consecutive 1s
         // }
         if (chromosome.Gene[row, column] == 1)
         {
            if (column < columnsSize && chromosome.Gene[row, column + 1] == 1)
            {
               return 1;
            }
            else if (column > 0 && chromosome.Gene[row, column - 1] == 1)
            {
               return 1;
            }
            else if (column > 0 && column < columnsSize && chromosome.Gene[row, column - 1] == 0 && chromosome.Gene[row, column + 1] == 0)
            {
               return -1;
            }
         }
         else if (chromosome.Gene[row, column] == 0)
         {
            if (column < columnsSize && chromosome.Gene[row, column + 1] == 0)
            {
               return 1;
            }
            else if (column > 0 && chromosome.Gene[row, column - 1] == 0)
            {
               return 1;
            }
            else if (column > 0 && column < columnsSize && chromosome.Gene[row, column - 1] == 1 && chromosome.Gene[row, column + 1] == 1)
            {
               return -1;
            }
         }
         // if (chromosome.Gene[row, column] == 1 && (column < columnsSize - 1 || chromosome.Gene[row, column + 1] == 1 || chromosome.Gene[row, column - 1] == 1))
         // {
         //    return 5; // More points for consecutive 1s
         // }
         return 0;
      }

      private int ValueWorkHours(int row, int hoursWorked)
      {
         int workload;
         JobPosition? jobPosition = this.Employees[row].JobPosition;
         if (jobPosition == null)
            return 0;

         workload = jobPosition.Workload * 2;

         // Raízes da função quadrática
         double root1 = workload - 2; // Ajuste as raízes conforme necessário
         double root2 = workload + 2;

         // Coordenada x do vértice (ponto médio das raízes)
         double xVertex = (root1 + root2) / 2;

         // Valor y do vértice (ponto de máximo)
         double yVertex = 200; // Ajuste o valor do ponto de máximo conforme necessário

         // Calcula o coeficiente 'k' da função quadrática
         // A função é f(x) = k(x - root1)(x - root2)
         // No vértice, f(xVertex) = yVertex
         double k = yVertex / ((xVertex - root1) * (xVertex - root2));

         // Como a parábola tem ponto de máximo, k deve ser negativo
         k = -Math.Abs(k);

         // Forma expandida da função quadrática: f(x) = kx² + bx + c
         double b = -k * (root1 + root2);
         double c = k * root1 * root2;

         // Calcula o valor da função para o número de horas trabalhadas (hoursWorked)
         // double functionValue = k * (hoursWorked * hoursWorked) + b * hoursWorked + c;
         double functionValue = k * (hoursWorked * hoursWorked) + b * hoursWorked;

         // Retorna o valor arredondado para o inteiro mais próximo
         return (int)Math.Round(functionValue);
      }

      private int WorkShiftSchedule(Chromosome chromosome, int row, int column)
      {
         int points = 0;
         int index = 0;
         int start = -1;
         int end = -1;
         Employee lastEmployeeWithSameJobPosition = null;
         Employee employee = Employees[row];
         JobPosition jobPosition = employee.JobPosition;

         do
         {
            JobPosition jobPositionLine = Employees[index].JobPosition;
            if (jobPositionLine.Id == jobPosition.Id && start == -1)
            {
               start = index;
            }
            else if (start > -1 && (index == Employees.Length - 1 || Employees[index + 1]?.JobPosition?.Id == jobPosition.Id))
            {
               end = index + 1;
               lastEmployeeWithSameJobPosition = Employees[index];
            }

            index++;
         } while (index < Employees.Length && end == -1);


         if (lastEmployeeWithSameJobPosition != null && lastEmployeeWithSameJobPosition.Id != employee.Id && (end - start) > 0)
         {
            for (int col = 0; col < this.columnsSize; col++)
            {
               bool isWorkingHour = IsWithinWorkingHours(col);

               if (isWorkingHour)
               {
                  int countOnes = 0;
                  for (int i = start; i < end; i++)
                  {
                     if (chromosome.Gene[i, col] == 1)
                     {
                        countOnes++;
                     }
                  }
                  if (countOnes >= 1 && countOnes < 3)
                  {
                     if (countOnes == 1)
                     {
                        points += 100;
                     }
                     else
                     {
                        points += 50;
                     }
                  }
                  else if (countOnes == 3)
                  {
                     points += 1;
                  }
                  // else if (countOnes == 0)
                  // {
                  //    points = 0;
                  // }
               }

            }
         }

         return points;
      }

      public void printMatrixList()
      {
         List<Chromosome> best = this.population.OrderByDescending(c => FitnessFunction(c)).Take(10).ToList();

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

      private void printMatrix(int[,] matrix)
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

            Console.Write("Employee " + Employees[i].Name + ": ");
            Console.WriteLine();
            Console.Write("           ");


            for (int j = 0; j < cols; j++)
            {
               TimeSpan currentTime = this.CalculateTime(j);

               if (matrix[i, j] == 1)
                  workingHours++;

               if ((matrix[i, j] == 1 && matrix[i, j - 1] == 0) || (matrix[i, j] == 1 && j == 0))
               {
                  Console.Write("( " + currentTime.ToString().Substring(0, 5) + " - ");
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
                  Console.WriteLine();
                  Console.Write("           ");
                  Console.Write("Working Hours: " + new TimeSpan(workingHours / 2, (workingHours % 2) * 30, 0).ToString().Substring(0, 5));
                  // Console.Write("Working Hours: " + this.CalculateTime(workingHours).ToString().Substring(0, 5));
                  Console.WriteLine();
                  if (j != cols - 1)
                     Console.Write("           ");

                  workingHours = 0;
               }
            }
            Console.WriteLine();
         }
      }
   }
}