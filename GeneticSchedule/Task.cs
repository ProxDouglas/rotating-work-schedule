namespace rotating_work_schedule.GeneticSchedule
{
   class Task
   {
      public string Name { get; set; }
      public int Difficulty { get; set; } // 1 a 5
      public int CognitiveLoad { get; set; } // 1 a 5
      public int Affinity { get; set; } // 1 a 5
      public List<(int Day, int TimeSlot)> ForbiddenSlots { get; set; } // Horários proibidos
   }
}