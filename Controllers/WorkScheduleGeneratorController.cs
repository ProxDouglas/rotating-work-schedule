namespace rotating_work_schedule.Controllers;

using Microsoft.AspNetCore.Mvc;
using rotating_work_schedule.Models;
using rotating_work_schedule.GeneticAlgorithm;
using RotatingWorkSchedule.Enums;
using rotating_work_schedule.GeneticAlgorithm.PrePocessing;

[ApiController]
[Route("api/workschedule")]
public class WorkScheduleGeneratorController() : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> GetAll()
    {

        JobPosition caixa = new JobPosition { Name = "Caixa", Workload = 8, MaximumConsecutiveDays = 6 };
        JobPosition repositor = new JobPosition { Name = "Repositor", Workload = 8, MaximumConsecutiveDays = 6 };
        JobPosition supervisor = new JobPosition { Name = "Supervisor", Workload = 8, MaximumConsecutiveDays = 6 };
        JobPosition estoquista = new JobPosition { Name = "Estoquista", Workload = 8, MaximumConsecutiveDays = 6 };

        List<JobPosition> jobPositions = new List<JobPosition>
        {
            caixa,
            repositor,
            supervisor,
            estoquista
        };

        Unavailability unavailability = new Unavailability { Start = new DateTime(2025, 3, 17, 0, 0, 0), End = new DateTime(2025, 3, 17, 0, 0, 0), EffectiveDate = new DateTime(2025, 3, 17, 0, 0, 0), Validity = new DateTime(2025, 3, 17, 0, 0, 0) };
        Unavailability unavailability2 = new Unavailability { Start = new DateTime(2025, 3, 18, 0, 0, 0), End = new DateTime(2025, 3, 18, 0, 0, 0), EffectiveDate = new DateTime(2025, 3, 18, 0, 0, 0), Validity = new DateTime(2025, 3, 18, 0, 0, 0) };
        Unavailability unavailability3 = new Unavailability { Start = new DateTime(2025, 3, 19, 0, 0, 0), End = new DateTime(2025, 3, 19, 0, 0, 0), EffectiveDate = new DateTime(2025, 3, 19, 0, 0, 0), Validity = new DateTime(2025, 3, 19, 0, 0, 0) };
        Unavailability unavailability4 = new Unavailability { Start = new DateTime(2025, 3, 20, 0, 0, 0), End = new DateTime(2025, 3, 23, 0, 0, 0), EffectiveDate = new DateTime(2025, 3, 20, 0, 0, 0), Validity = new DateTime(2025, 3, 20, 0, 0, 0) };

        List<Employee> employees = new List<Employee>
        {
            new Employee { Name = "Caixa 1", JobPosition = caixa, Unavailabilities = [unavailability] },
            new Employee { Name = "Caixa 2", JobPosition = caixa, Unavailabilities = [unavailability2] },
            new Employee { Name = "Caixa 3", JobPosition = caixa, Unavailabilities = [unavailability3] },
            new Employee { Name = "Caixa 4", JobPosition = caixa, Unavailabilities = [unavailability4] },
            new Employee { Name = "Caixa 5", JobPosition = caixa, Unavailabilities = [] },
            new Employee { Name = "Caixa 6", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 7", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 8", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 9", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 10", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 11", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 12", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 13", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 14", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 15", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 16", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 17", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 18", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 19", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 20", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 21", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 22", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 23", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 24", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 25", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 26", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 27", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 28", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 29", JobPosition = caixa, Unavailabilities = [] },
            // new Employee { Name = "Caixa 30", JobPosition = caixa, Unavailabilities = [] },
            new Employee { Name = "Repositor 5", JobPosition = repositor, Unavailabilities = [] },
            new Employee { Name = "Repositor 6", JobPosition = repositor, Unavailabilities = [] },
            new Employee { Name = "Repositor 7", JobPosition = repositor, Unavailabilities = [] },
            new Employee { Name = "Repositor 8", JobPosition = repositor, Unavailabilities = [] },
            new Employee { Name = "Employee 4", JobPosition = supervisor, Unavailabilities = [] },
            new Employee { Name = "Estoquista 9", JobPosition = estoquista, Unavailabilities = [] },
            new Employee { Name = "Estoquista 10", JobPosition = estoquista, Unavailabilities = [] },
        };

        List<OperatingSchedule> operatingSchedules = new List<OperatingSchedule>
        {
            new OperatingSchedule { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Sunday },
            new OperatingSchedule { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Monday },
            new OperatingSchedule { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Tuesday },
            new OperatingSchedule { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Saturday },
            new OperatingSchedule { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Thursday },
            new OperatingSchedule { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Friday },
            new OperatingSchedule { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Wednesday },
            new OperatingSchedule { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Holiday },
        };

        List<WorkDay> workDays = new List<WorkDay>
        {
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 17, 0, 0, 0), DayOperating = DayOperating.Sunday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 18, 0, 0, 0), DayOperating = DayOperating.Monday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 19, 0, 0, 0), DayOperating = DayOperating.Tuesday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 20, 0, 0, 0), DayOperating = DayOperating.Saturday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 21, 0, 0, 0), DayOperating = DayOperating.Thursday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 22, 0, 0, 0), DayOperating = DayOperating.Friday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 23, 0, 0, 0), DayOperating = DayOperating.Wednesday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 24, 0, 0, 0), DayOperating = DayOperating.Holiday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 25, 0, 0, 0), DayOperating = DayOperating.Sunday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 26, 0, 0, 0), DayOperating = DayOperating.Monday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 27, 0, 0, 0), DayOperating = DayOperating.Tuesday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 28, 0, 0, 0), DayOperating = DayOperating.Saturday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 29, 0, 0, 0), DayOperating = DayOperating.Thursday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 30, 0, 0, 0), DayOperating = DayOperating.Friday},
            new WorkDay { EffectiveDate = new DateTime(2025, 3, 31, 0, 0, 0), DayOperating = DayOperating.Wednesday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 1, 0, 0, 0), DayOperating = DayOperating.Holiday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 2, 0, 0, 0), DayOperating = DayOperating.Sunday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 3, 0, 0, 0), DayOperating = DayOperating.Monday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 4, 0, 0, 0), DayOperating = DayOperating.Tuesday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 5, 0, 0, 0), DayOperating = DayOperating.Saturday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 6, 0, 0, 0), DayOperating = DayOperating.Thursday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 7, 0, 0, 0), DayOperating = DayOperating.Friday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 8, 0, 0, 0), DayOperating = DayOperating.Wednesday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 9, 0, 0, 0), DayOperating = DayOperating.Holiday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 10, 0, 0, 0), DayOperating = DayOperating.Sunday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 11, 0, 0, 0), DayOperating = DayOperating.Monday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 12, 0, 0, 0), DayOperating = DayOperating.Tuesday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 13, 0, 0, 0), DayOperating = DayOperating.Saturday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 14, 0, 0, 0), DayOperating = DayOperating.Thursday},
            new WorkDay { EffectiveDate = new DateTime(2025, 4, 15, 0, 0, 0), DayOperating = DayOperating.Friday},
        };

        var generateDayOff = new GenerateDayOff();
        generateDayOff.Run(employees, workDays);

        // Geração de todos os dias jusntos
        var configuration = new ConfigurationSchedule(
            employees,
            jobPositions,
            operatingSchedules,
            workDays.ToArray(),
            0
            );

        var workScheduleGenerator = new WorkScheduleGenerator(configuration);
        var bestSchedule = await workScheduleGenerator.RunGeneticAlgorithmAsync();

        // Geração por dia
        // var tasks = workDays.Select(async workDay =>
        // {
        //     var configuration = new ConfigurationSchedule(
        //     employees,
        //     jobPositions,
        //     operatingSchedules,
        //     [workDay],
        //     0
        //     );

        //     var workScheduleGenerator = new WorkScheduleGenerator(configuration);
        //     var bestSchedule = await workScheduleGenerator.RunGeneticAlgorithmAsync();

        //     // workScheduleGenerator.printMatrixList([bestSchedule]);
        //     return bestSchedule;
        // }).ToList();

        // var schedules = await Task.WhenAll(tasks);

        // Chromosome bestSchedule = null;

        // foreach (var schedule in schedules)
        // {
        //     if (bestSchedule == null)
        //     {
        //         bestSchedule = schedule;
        //     }
        //     else
        //     {
        //         bestSchedule.AppendColumnsFrom(schedule);
        //     }
        // }

        PrintConsole printConsole = new PrintConsole();
        printConsole.printMatrixList([bestSchedule], employees, workDays[0].EffectiveDate, workDays);


        return Ok(new
        {
            Message = "Work schedule generated successfully.",
            Employees = employees,
            JobPositions = jobPositions,
            OperatingSchedules = operatingSchedules,
            WorkDays = workDays
        });
    }
}
