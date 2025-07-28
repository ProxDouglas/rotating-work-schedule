using WorkSchedule.Entities;
using WorkSchedule.GeneticAlgorithm;
using WorkSchedule.QueueRabbitMQ;

namespace WorkSchedule.Mappers;

class ChromosomeMapper
{
   public WorkScheduleGenerated toWorkScheduleGenerated(Chromosome chromosome, WorkScheduleRequest order, DateOnly startDate)
   {
      int[,] matrix = chromosome.Gene;
      var employeeSchedules = new List<EmployeeSchedule>();
      int rows = matrix.GetLength(0);
      int cols = matrix.GetLength(1);

      for (int i = 0; i < rows; i++)
      {
         var workDaySchedules = new List<WorkDaySchedule>();
         int j = 0;
         while (j < cols)
         {
            // Encontrar início de um bloco de trabalho
            if (matrix[i, j] == 1 && (j == 0 || matrix[i, j - 1] == 0))
            {
               int startCol = j;
               // Encontrar fim do bloco de trabalho
               while (j < cols && matrix[i, j] == 1)
               {
                  j++;
               }
               int endCol = j - 1;

               // Calcular data e horários
               int totalMinutesStart = startCol * 30;
               int daysOffsetStart = totalMinutesStart / (24 * 60);
               DateOnly currentDate = startDate.AddDays(daysOffsetStart);
               int minutesInDayStart = totalMinutesStart % (24 * 60);
               TimeSpan startTime = new TimeSpan(minutesInDayStart / 60, minutesInDayStart % 60, 0);

               int totalMinutesEnd = (endCol + 1) * 30;
               int minutesInDayEnd = totalMinutesEnd % (24 * 60);
               TimeSpan endTime = new TimeSpan(minutesInDayEnd / 60, minutesInDayEnd % 60, 0);

               workDaySchedules.Add(new WorkDaySchedule
               {
                  Date = currentDate,
                  Start = startTime,
                  End = endTime
               });
            }
            else
            {
               j++;
            }
         }

         employeeSchedules.Add(new EmployeeSchedule
         {
            Name = order.Employees[i].Name,
            WorkDays = workDaySchedules
         });
      }

      var schedule = new WorkScheduleGenerated
      {
         EmployeeSchedule = employeeSchedules,
         IsValid = true
      };

      return schedule;
   }
}