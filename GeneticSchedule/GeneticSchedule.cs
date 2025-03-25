namespace rotating_work_schedule.GeneticSchedule
{
   class GeneticScheduler
   {
      private List<Task> tasks;
      private int days = 7;
      private int timeSlotsPerDay = 8; // Exemplo: 8 períodos por dia
      private int populationSize = 100;
      private int generations = 1000;
      private double mutationRate = 0.1;
      private Random random = new();

      public GeneticScheduler(List<Task> tasks)
      {
         this.tasks = tasks;
      }

      public Schedule Run()
      {
         List<Schedule> population = InitializePopulation();
         for (int i = 0; i < generations; i++)
         {
            population = Evolve(population);
         }
         return population.OrderByDescending(s => s.Fitness).First();
      }

      private List<Schedule> InitializePopulation()
      {
         List<Schedule> population = new();
         for (int i = 0; i < populationSize; i++)
         {
            Schedule schedule = GenerateRandomSchedule();
            schedule.Fitness = EvaluateFitness(schedule);
            population.Add(schedule);
         }
         return population;
      }

      private Schedule GenerateRandomSchedule()
      {
         Schedule schedule = new();
         Dictionary<string, int> taskWeeklyCount = new();
         Dictionary<(int Day, string TaskName), int> taskDailyCount = new();

         foreach (var task in tasks)
         {
            int assignedHours = 0;
            taskWeeklyCount[task.Name] = 0;

            while (assignedHours < task.WeeklyHours && taskWeeklyCount[task.Name] < task.MaxRepetitions)
            {
               int day, timeSlot;
               do
               {
                  day = random.Next(days);
                  timeSlot = random.Next(timeSlotsPerDay);
               } while (task.ForbiddenSlots.Contains((day, timeSlot)) || schedule.WeeklySchedule.ContainsKey((day, timeSlot)));

               if (!taskDailyCount.ContainsKey((day, task.Name)))
                  taskDailyCount[(day, task.Name)] = 0;

               if (taskDailyCount[(day, task.Name)] < task.MaxDailyHours)
               {
                  schedule.WeeklySchedule[(day, timeSlot)] = task;
                  taskDailyCount[(day, task.Name)]++;
                  taskWeeklyCount[task.Name]++;
                  assignedHours++;
               }
            }
         }
         return schedule;
      }

      private double EvaluateFitness(Schedule schedule)
      {
         double score = 0;
         Dictionary<int, int> cognitiveLoadPerDay = new();
         Dictionary<string, int> taskWeeklyCount = new();

         foreach (var kv in schedule.WeeklySchedule)
         {
            var task = kv.Value;
            score += task.Affinity * 2; // Valorizando afinidade
            score += 5 - task.Difficulty; // Menos difícil = melhor

            if (!cognitiveLoadPerDay.ContainsKey(kv.Key.Day))
               cognitiveLoadPerDay[kv.Key.Day] = 0;

            cognitiveLoadPerDay[kv.Key.Day] += task.CognitiveLoad;

            if (!taskWeeklyCount.ContainsKey(task.Name))
               taskWeeklyCount[task.Name] = 0;

            taskWeeklyCount[task.Name]++;
         }

         foreach (var load in cognitiveLoadPerDay.Values)
         {
            if (load > 10) score -= (load - 10) * 2; // Penalização por sobrecarga
         }

         foreach (var task in tasks)
         {
            if (taskWeeklyCount.ContainsKey(task.Name) && taskWeeklyCount[task.Name] > task.MaxRepetitions)
               score -= (taskWeeklyCount[task.Name] - task.MaxRepetitions) * 5; // Penalização por excesso de repetições
         }

         return score;
      }

      private List<Schedule> Evolve(List<Schedule> population)
      {
         List<Schedule> newPopulation = new();
         population = population.OrderByDescending(s => s.Fitness).ToList();
         newPopulation.AddRange(population.Take(10)); // Elitismo

         while (newPopulation.Count < populationSize)
         {
            var parent1 = SelectParent(population);
            var parent2 = SelectParent(population);
            var child = Crossover(parent1, parent2);
            if (random.NextDouble() < mutationRate)
               Mutate(child);
            child.Fitness = EvaluateFitness(child);
            newPopulation.Add(child);
         }

         return newPopulation;
      }

      private Schedule SelectParent(List<Schedule> population)
      {
         return population[random.Next(population.Count / 2)]; // Seleção por torneio truncado
      }

      private Schedule Crossover(Schedule parent1, Schedule parent2)
      {
         Schedule child = new();
         foreach (var kv in parent1.WeeklySchedule)
         {
            if (random.NextDouble() < 0.5)
               child.WeeklySchedule[kv.Key] = kv.Value;
         }
         foreach (var kv in parent2.WeeklySchedule)
         {
            if (!child.WeeklySchedule.ContainsKey(kv.Key) && random.NextDouble() < 0.5)
               child.WeeklySchedule[kv.Key] = kv.Value;
         }
         return child;
      }

      private void Mutate(Schedule schedule)
      {
         var keys = schedule.WeeklySchedule.Keys.ToList();
         if (keys.Count > 0)
         {
            var randomKey = keys[random.Next(keys.Count)];
            var task = schedule.WeeklySchedule[randomKey];
            schedule.WeeklySchedule.Remove(randomKey);

            int day, timeSlot;
            do
            {
               day = random.Next(days);
               timeSlot = random.Next(timeSlotsPerDay);
            } while (task.ForbiddenSlots.Contains((day, timeSlot)) || schedule.WeeklySchedule.ContainsKey((day, timeSlot)));

            schedule.WeeklySchedule[(day, timeSlot)] = task;
         }
      }

      public string GetTimeRange(int timeSlot)
      {
         string[] timeRanges = {
        "08:00 - 09:30", "09:30 - 11:00", "11:00 - 12:30",
        "12:30 - 14:00", "14:00 - 15:30", "15:30 - 17:00",
        "17:00 - 18:30", "18:30 - 21:00"
    };
         return timeRanges[timeSlot];
      }

      public string GetDayName(int day)
      {
         string[] days = { "Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado" };
         return days[day];
      }

      public void PrintSchedule(Schedule schedule)
      {
         foreach (var kv in schedule.WeeklySchedule)
         {
            int day = kv.Key.Day;
            int timeSlot = kv.Key.TimeSlot;
            string timeRange = GetTimeRange(timeSlot);

            Console.WriteLine($"Dia {day} ({GetDayName(day)}), {timeRange}: {kv.Value.Name}");
         }
      }
   }
}