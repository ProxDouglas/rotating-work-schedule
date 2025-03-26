
namespace rotating_work_schedule.GeneticAlgorithm2
{

   public class ConfiguracaoEscala
   {
      public List<DayOfWeek> DiasDaSemana { get; set; }
      public List<TimeSpan> HorariosDiarios { get; set; }
      public List<Funcionario> Funcionarios { get; set; }
      public Dictionary<string, (int Min, int Max)> RequisitosPorHorario { get; set; }
      public int HorasDiariasPorFuncionario { get; set; }
      public int DiasTrabalhadosPorSemana { get; set; }
      public int TamanhoPopulacao { get; set; }
      public int NumeroGeracoes { get; set; }
      public double TaxaMutacao { get; set; }
   }
}