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

        JobPosition caixa = new JobPosition { Id = 1, Name = "Caixa", Workload = 8, MaximumConsecutiveDays = 6, MaximumEmployees = 4, MinimumEmployees = 2 };
        JobPosition repositor = new JobPosition { Id = 2, Name = "Repositor", Workload = 8, MaximumConsecutiveDays = 6, MaximumEmployees = 4, MinimumEmployees = 2 };
        JobPosition supervisor = new JobPosition { Id = 3, Name = "Supervisor", Workload = 8, MaximumConsecutiveDays = 6, MaximumEmployees = 2, MinimumEmployees = 0 };
        JobPosition estoquista = new JobPosition { Id = 4, Name = "Estoquista", Workload = 8, MaximumConsecutiveDays = 6, MaximumEmployees = 2, MinimumEmployees = 1 };

        JobPosition[] jobPositions = new JobPosition[]
        {
            caixa,
            repositor,
            supervisor,
            estoquista
        };

        // Unavailability unavailability = new Unavailability { Id = 1, EmployeeId = 1, Start = new DateTime(2025, 3, 17, 0, 0, 0), End = new DateTime(2025, 3, 17, 0, 0, 0), Reason = "Folga", EffectiveDate = new DateTime(2025, 3, 17, 0, 0, 0), Validity = new DateTime(2025, 3, 17, 0, 0, 0) };

        Employee[] employees = new Employee[]
        {
            new Employee { Id = 1, Name = "Caixa 1", JobPosition = caixa },
            new Employee { Id = 2, Name = "Caixa 2", JobPosition = caixa },
            new Employee { Id = 3, Name = "Caixa 3", JobPosition = caixa },
            new Employee { Id = 4, Name = "Caixa 4", JobPosition = caixa },
            new Employee { Id = 20, Name = "Repositor 5", JobPosition = repositor },
            new Employee { Id = 21, Name = "Repositor 6", JobPosition = repositor },
            new Employee { Id = 22, Name = "Repositor 7", JobPosition = repositor },
            new Employee { Id = 23, Name = "Repositor 8", JobPosition = repositor },
            new Employee { Id = 30, Name = "Employee 4", JobPosition = supervisor },
            new Employee { Id = 40, Name = "Estoquista 9", JobPosition = estoquista },
            new Employee { Id = 41, Name = "Estoquista 10", JobPosition = estoquista },
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

        WorkDay[] workDays = new WorkDay[]
        {
            new WorkDay { Id = 1, EffectiveDate = new DateTime(2025, 3, 17, 0, 0, 0), DayOperating = DayOperating.Sunday, OperatingSchedule = operatingSchedules[0] },
            new WorkDay { Id = 2, EffectiveDate = new DateTime(2025, 3, 18, 0, 0, 0), DayOperating = DayOperating.Monday, OperatingSchedule = operatingSchedules[1] },
            new WorkDay { Id = 3, EffectiveDate = new DateTime(2025, 3, 19, 0, 0, 0), DayOperating = DayOperating.Tuesday, OperatingSchedule = operatingSchedules[2] },
            new WorkDay { Id = 4, EffectiveDate = new DateTime(2025 ,3 ,20 ,0 ,0 ,0), DayOperating = DayOperating.Saturday , OperatingSchedule=operatingSchedules[3] },
            new WorkDay { Id = 5, EffectiveDate = new DateTime(2025, 3, 21, 0, 0, 0), DayOperating = DayOperating.Thursday, OperatingSchedule = operatingSchedules[4] },
            new WorkDay { Id = 6, EffectiveDate = new DateTime(2025, 3, 22, 0, 0, 0), DayOperating = DayOperating.Friday , OperatingSchedule = operatingSchedules[5] },
            new WorkDay { Id = 7, EffectiveDate = new DateTime(2025, 3, 23, 0, 0, 0), DayOperating = DayOperating.Wednesday, OperatingSchedule = operatingSchedules[6] },
            new WorkDay { Id = 8, EffectiveDate = new DateTime(2025, 3, 24, 0, 0, 0), DayOperating = DayOperating.Holiday, OperatingSchedule = operatingSchedules[7] },
            new WorkDay { Id = 9, EffectiveDate = new DateTime(2025, 3, 25, 0, 0, 0), DayOperating = DayOperating.Sunday, OperatingSchedule = operatingSchedules[0] },
            new WorkDay { Id = 10, EffectiveDate = new DateTime(2025, 3, 26, 0, 0, 0), DayOperating = DayOperating.Monday, OperatingSchedule = operatingSchedules[1] },
            new WorkDay { Id = 11, EffectiveDate = new DateTime(2025, 3, 27, 0, 0, 0), DayOperating = DayOperating.Tuesday, OperatingSchedule = operatingSchedules[2] },
            new WorkDay { Id = 12, EffectiveDate = new DateTime(2025, 3, 28, 0, 0, 0), DayOperating = DayOperating.Saturday, OperatingSchedule = operatingSchedules[3] },
            new WorkDay { Id = 13, EffectiveDate = new DateTime(2025, 3, 29, 0, 0, 0), DayOperating = DayOperating.Thursday, OperatingSchedule = operatingSchedules[4] },
            new WorkDay { Id = 14, EffectiveDate = new DateTime(2025, 3, 30, 0, 0, 0), DayOperating = DayOperating.Friday, OperatingSchedule = operatingSchedules[5] },
            new WorkDay { Id = 15, EffectiveDate = new DateTime(2025, 3, 31, 0, 0, 0), DayOperating = DayOperating.Wednesday, OperatingSchedule = operatingSchedules[6] },
            new WorkDay { Id = 16, EffectiveDate = new DateTime(2025, 4, 1, 0, 0, 0), DayOperating = DayOperating.Holiday, OperatingSchedule = operatingSchedules[7] },
            new WorkDay { Id = 17, EffectiveDate = new DateTime(2025, 4, 2, 0, 0, 0), DayOperating = DayOperating.Sunday, OperatingSchedule = operatingSchedules[0] },
            new WorkDay { Id = 18, EffectiveDate = new DateTime(2025, 4, 3, 0, 0, 0), DayOperating = DayOperating.Monday, OperatingSchedule = operatingSchedules[1] },
            new WorkDay { Id = 19, EffectiveDate = new DateTime(2025, 4, 4, 0, 0, 0), DayOperating = DayOperating.Tuesday, OperatingSchedule = operatingSchedules[2] },
            new WorkDay { Id = 20, EffectiveDate = new DateTime(2025, 4, 5, 0, 0, 0), DayOperating = DayOperating.Saturday, OperatingSchedule = operatingSchedules[3] },
            new WorkDay { Id = 21, EffectiveDate = new DateTime(2025, 4, 6, 0, 0, 0), DayOperating = DayOperating.Thursday, OperatingSchedule = operatingSchedules[4] },
            new WorkDay { Id = 22, EffectiveDate = new DateTime(2025, 4, 7, 0, 0, 0), DayOperating = DayOperating.Friday, OperatingSchedule = operatingSchedules[5] },
            new WorkDay { Id = 23, EffectiveDate = new DateTime(2025, 4, 8, 0, 0, 0), DayOperating = DayOperating.Wednesday, OperatingSchedule = operatingSchedules[6] },
            new WorkDay { Id = 24, EffectiveDate = new DateTime(2025, 4, 9, 0, 0, 0), DayOperating = DayOperating.Holiday, OperatingSchedule = operatingSchedules[7] },
            new WorkDay { Id = 25, EffectiveDate = new DateTime(2025, 4, 10, 0, 0, 0), DayOperating = DayOperating.Sunday, OperatingSchedule = operatingSchedules[0] },
            new WorkDay { Id = 26, EffectiveDate = new DateTime(2025, 4, 11, 0, 0, 0), DayOperating = DayOperating.Monday, OperatingSchedule = operatingSchedules[1] },
            new WorkDay { Id = 27, EffectiveDate = new DateTime(2025, 4, 12, 0, 0, 0), DayOperating = DayOperating.Tuesday, OperatingSchedule = operatingSchedules[2] },
            new WorkDay { Id = 28, EffectiveDate = new DateTime(2025, 4, 13, 0, 0, 0), DayOperating = DayOperating.Saturday, OperatingSchedule = operatingSchedules[3] },
            new WorkDay { Id = 29, EffectiveDate = new DateTime(2025, 4, 14, 0, 0, 0), DayOperating = DayOperating.Thursday, OperatingSchedule = operatingSchedules[4] },
            new WorkDay { Id = 30, EffectiveDate = new DateTime(2025, 4, 15, 0, 0, 0), DayOperating = DayOperating.Friday, OperatingSchedule = operatingSchedules[5] },
        };

        var configuration = new ConfigurationSchedule(
            employees,
            jobPositions,
            operatingSchedules,
            workDays,
            0
        );

        var workScheduleGenerator = new WorkScheduleGenerator(configuration);
        var indisponibilidade = workScheduleGenerator.GenerateEmployeeDaysOff(employees.ToList(), workDays.ToList());
        var bestSchedule = await workScheduleGenerator.RunGeneticAlgorithmAsync();

        // workScheduleGenerator.printMatrix(bestSchedule.Gene);
        // workScheduleGenerator.printSchedule(bestSchedule.Gene);
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
