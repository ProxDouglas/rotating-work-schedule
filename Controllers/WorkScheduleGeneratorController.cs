namespace rotating_work_schedule.Controllers;
using Microsoft.AspNetCore.Mvc;
using rotating_work_schedule.Models;
using rotating_work_schedule.GeneticAlgorithm;

[ApiController]
[Route("api/workschedule")]
public class WorkScheduleGeneratorController() : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {

        JobPosition jobPosition = new JobPosition { Id = 1, Name = "Caixa", Workload = 8, MaximumConsecutiveDays = 6 };
        JobPosition jobPosition2 = new JobPosition { Id = 2, Name = "Repositor", Workload = 8, MaximumConsecutiveDays = 6 };
        JobPosition jobPosition3 = new JobPosition { Id = 3, Name = "Supervisor", Workload = 8, MaximumConsecutiveDays = 6 };
        JobPosition jobPosition4 = new JobPosition { Id = 4, Name = "Estoquista", Workload = 8, MaximumConsecutiveDays = 6 };


        Employee[] employees = new Employee[]
        {
            new Employee { Id = 1, Name = "Employee 1", JobPosition = jobPosition },
            new Employee { Id = 2, Name = "Employee 2", JobPosition = jobPosition },
            new Employee { Id = 3, Name = "Employee 3", JobPosition = jobPosition },
            new Employee { Id = 4, Name = "Employee 4", JobPosition = jobPosition3 },
            new Employee { Id = 5, Name = "Employee 5", JobPosition = jobPosition2 },
            new Employee { Id = 6, Name = "Employee 6", JobPosition = jobPosition2 },
            new Employee { Id = 7, Name = "Employee 7", JobPosition = jobPosition4 }
        };

        OperatingSchedule[] operatingSchedules = new OperatingSchedule[]
        {
            new OperatingSchedule {Id = 1, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOfWeek = DayOfWeek.Sunday },
            new OperatingSchedule {Id = 2, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOfWeek = DayOfWeek.Monday },
            new OperatingSchedule {Id = 3, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOfWeek = DayOfWeek.Tuesday },
            new OperatingSchedule {Id = 4, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOfWeek = DayOfWeek.Saturday },
            new OperatingSchedule {Id = 5, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOfWeek = DayOfWeek.Thursday },
            new OperatingSchedule {Id = 6, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOfWeek = DayOfWeek.Friday },
            new OperatingSchedule {Id = 7, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOfWeek = DayOfWeek.Wednesday }
        };

        WorkScheduleGenerator workScheduleGenerator = new WorkScheduleGenerator(employees, operatingSchedules, new DateTime(2025, 3, 17, 0, 0, 0));
        await workScheduleGenerator.RunGeneticAlgorithmAsync(1000);


        // Chamada da função para imprimir no console
        workScheduleGenerator.printMatrixList(workScheduleGenerator.getBestSchedules());

        // Chamada da função que converte para lista de strings
        List<string> matrixStrings = ConvertMatrixToStringList(workScheduleGenerator.getBestSchedules());

        return Ok(matrixStrings);
    }

    /// <summary>
    /// Função que converte cada matriz em uma string e retorna uma lista de strings.
    /// </summary>
    private List<string> ConvertMatrixToStringList(List<int[,]> matrices)
    {
        List<string> matrixStrings = new List<string>();

        foreach (var matrix in matrices)
        {
            matrixStrings.Add(MatrixToString(matrix));
        }

        return matrixStrings;
    }

    /// <summary>
    /// Converte uma matriz em uma string formatada.
    /// </summary>
    private string MatrixToString(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        List<string> rowStrings = new List<string>();

        for (int i = 0; i < rows; i++)
        {
            List<string> rowValues = new List<string>();

            for (int j = 0; j < cols; j++)
            {
                rowValues.Add(matrix[i, j].ToString());
            }

            rowStrings.Add(string.Join(" ", rowValues));
        }

        return string.Join("\n", rowStrings);
    }
}
