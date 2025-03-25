namespace rotating_work_schedule.GeneticSchedule
{
   class Schedule
   {
      public Dictionary<(int Day, int TimeSlot), Task> WeeklySchedule { get; set; } = new();
      public double Fitness { get; set; }
   }

}
