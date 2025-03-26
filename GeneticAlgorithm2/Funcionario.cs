
namespace rotating_work_schedule.GeneticAlgorithm2
{

   public class Funcionario
    {
        public int Id { get; set; }
        public string Cargo { get; set; }
        public List<DayOfWeek> DiasFolga { get; set; } = new List<DayOfWeek>();
    }
}