namespace rotating_work_schedule.Controllers;
using Microsoft.AspNetCore.Mvc;
using rotating_work_schedule.Models;
using rotating_work_schedule.GeneticAlgorithm;
using rotating_work_schedule.GeneticAlgorithm2;
using RotatingWorkSchedule.Enums;

[ApiController]
[Route("api/workschedule")]
public class WorkScheduleGeneratorController() : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {

        JobPosition jobPosition = new JobPosition { Id = 1, Name = "Caixa", Workload = 9, MaximumConsecutiveDays = 6 };
        JobPosition jobPosition2 = new JobPosition { Id = 2, Name = "Repositor", Workload = 9, MaximumConsecutiveDays = 6 };
        JobPosition jobPosition3 = new JobPosition { Id = 3, Name = "Supervisor", Workload = 9, MaximumConsecutiveDays = 6 };
        JobPosition jobPosition4 = new JobPosition { Id = 4, Name = "Estoquista", Workload = 9, MaximumConsecutiveDays = 6 };

        // Unavailability unavailability = new Unavailability { Id = 1, EmployeeId = 1, Start = new DateTime(2025, 3, 17, 0, 0, 0), End = new DateTime(2025, 3, 17, 0, 0, 0), Reason = "Folga", EffectiveDate = new DateTime(2025, 3, 17, 0, 0, 0), Validity = new DateTime(2025, 3, 17, 0, 0, 0) };

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
            new OperatingSchedule {Id = 1, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Sunday },
            new OperatingSchedule {Id = 2, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Monday },
            new OperatingSchedule {Id = 3, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Tuesday },
            new OperatingSchedule {Id = 4, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Saturday },
            new OperatingSchedule {Id = 5, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Thursday },
            new OperatingSchedule {Id = 6, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Friday },
            new OperatingSchedule {Id = 7, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Wednesday },
            new OperatingSchedule {Id = 7, Start = new TimeSpan(8, 0, 0), End = new TimeSpan(21, 0, 0), DayOperating = DayOperating.Holiday },
        };

        WorkDay[] workDay = new WorkDay[]
        {
            new WorkDay { Id = 1, EffectiveDate = new DateTime(2025, 3, 17, 0, 0, 0), DayOperating = DayOperating.Sunday, OperatingSchedule = operatingSchedules[0] },
            new WorkDay { Id = 2, EffectiveDate = new DateTime(2025, 3, 18, 0, 0, 0), DayOperating = DayOperating.Monday, OperatingSchedule = operatingSchedules[1] },
            new WorkDay { Id = 3, EffectiveDate = new DateTime(2025, 3, 19, 0, 0, 0), DayOperating = DayOperating.Tuesday, OperatingSchedule = operatingSchedules[2] },
            new WorkDay { Id = 4 , EffectiveDate = new DateTime(2025 ,3 ,20 ,0 ,0 ,0) , DayOperating=DayOperating.Saturday , OperatingSchedule=operatingSchedules[3]},
            new WorkDay { Id = 5, EffectiveDate = new DateTime(2025, 3, 21, 0, 0, 0), DayOperating = DayOperating.Thursday, OperatingSchedule = operatingSchedules[4] },
            new WorkDay { Id = 6, EffectiveDate = new DateTime(2025, 3, 22, 0, 0, 0), DayOperating = DayOperating.Friday , OperatingSchedule = operatingSchedules[5] },
            new WorkDay { Id = 7, EffectiveDate = new DateTime(2025, 3, 23, 0, 0, 0), DayOperating = DayOperating.Wednesday, OperatingSchedule = operatingSchedules[6] },
        };

        var configuration = new ConfigurationSchedule(
            employees,
            operatingSchedules,
            workDay,
            100
        );

        var workScheduleGenerator = new WorkScheduleGenerator(configuration);
        var bestSchedule = await workScheduleGenerator.RunGeneticAlgorithmAsync();

        workScheduleGenerator.printMatrix(bestSchedule.Gene);
        workScheduleGenerator.printMatrixList();

        // Exemplo de uso do algoritmo genético para escalas semanais
        /*
        var config = new ConfiguracaoEscala
        {
            DiasDaSemana = new List<DayOfWeek> {
                    DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                    DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday
                },
            HorariosDiarios = Enumerable.Range(8, 14) // Das 8h às 17h (10 horários)
                    .Select(h => TimeSpan.FromHours(h))
                    .ToList(),
            Funcionarios = new List<Funcionario> {
                    new Funcionario { Id = 1, Cargo = "Atendente", DiasFolga = new List<DayOfWeek> { DayOfWeek.Sunday } },
                    new Funcionario { Id = 2, Cargo = "Atendente", DiasFolga = new List<DayOfWeek> { DayOfWeek.Sunday } },
                    new Funcionario { Id = 3, Cargo = "Atendente", DiasFolga = new List<DayOfWeek> { DayOfWeek.Sunday } },
                    new Funcionario { Id = 4, Cargo = "Atendente", DiasFolga = new List<DayOfWeek> { DayOfWeek.Sunday } },
                    new Funcionario { Id = 5, Cargo = "Atendente", DiasFolga = new List<DayOfWeek> { DayOfWeek.Sunday } },
                    new Funcionario { Id = 6, Cargo = "Supervisor", DiasFolga = new List<DayOfWeek> { DayOfWeek.Sunday } },
                    new Funcionario { Id = 7, Cargo = "Supervisor", DiasFolga = new List<DayOfWeek> { DayOfWeek.Sunday } },
                    new Funcionario { Id = 8, Cargo = "Gerente", DiasFolga = new List<DayOfWeek> { DayOfWeek.Sunday, DayOfWeek.Saturday } }
                },
            RequisitosPorHorario = new Dictionary<string, (int Min, int Max)>
            {
                ["Atendente"] = (2, 4),
                ["Supervisor"] = (1, 2),
                ["Gerente"] = (0, 1)
            },
            HorasDiariasPorFuncionario = 8,
            DiasTrabalhadosPorSemana = 5,
            TamanhoPopulacao = 100,
            NumeroGeracoes = 1000,
            TaxaMutacao = 0.1
        };

        var organizador = new OrganizadorEscalaSemanalGenetico(config);
        var melhorEscala = organizador.EncontrarMelhorEscala();

        Console.WriteLine("Melhor escala semanal com horários contínuos encontrada:");
        Console.WriteLine(melhorEscala);

        // Imprime a escala por funcionário
        Console.WriteLine("\nEscala detalhada por funcionário:");
        foreach (var func in config.Funcionarios.OrderBy(f => f.Cargo).ThenBy(f => f.Id))
        {
            Console.WriteLine($"\nFuncionário {func.Id} ({func.Cargo}):");
            var horariosFunc = melhorEscala.Alocacoes
                .Where(a => a.Value.Contains(func))
                .Select(a => a.Key)
                .GroupBy(a => a.Dia)
                .OrderBy(g => g.Key);

            foreach (var dia in horariosFunc)
            {
                var horarios = dia.Select(x => x.Horario).OrderBy(h => h).ToList();
                var inicio = horarios.First();
                var fim = horarios.Last().Add(TimeSpan.FromHours(1)); // +1 hora para mostrar o término
                Console.WriteLine($"{dia.Key}: {inicio:hh\\:mm} às {fim:hh\\:mm}");
            }
        }
        */

        return Ok("Ok");
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
