namespace rotating_work_schedule.GeneticSchedule
{
   class Task
   {
      public string Name { get; set; }
      public int Difficulty { get; set; } // 1 a 5
      public int CognitiveLoad { get; set; } // 1 a 5
      public int Affinity { get; set; } // 1 a 5
      public int WeeklyHours { get; set; } // Carga horária total por semana
      public int MaxRepetitions { get; set; } // Máximo de vezes que pode se repetir por semana
      public int MaxDailyHours { get; set; } // Tempo máximo que pode ser praticado por dia
      public List<(int Day, int TimeSlot)> ForbiddenSlots { get; set; } // Horários proibidos
   }
}