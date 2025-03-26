
namespace rotating_work_schedule.GeneticAlgorithm2
{

    public class EscalaSemanal
    {
        public Dictionary<(DayOfWeek Dia, TimeSpan Horario), List<Funcionario>> Alocacoes { get; set; }
        public double Fitness { get; set; }

        public EscalaSemanal Clone()
        {
            return new EscalaSemanal
            {
                Alocacoes = this.Alocacoes.ToDictionary(entry => entry.Key, entry => entry.Value.ToList()),
                Fitness = this.Fitness
            };
        }

        public override string ToString()
        {
            var result = "";
            foreach (var kvp in Alocacoes.OrderBy(x => x.Key.Dia).ThenBy(x => x.Key.Horario))
            {
                result += $"{kvp.Key.Dia} {kvp.Key.Horario:hh\\:mm}: ";
                result += string.Join(", ", kvp.Value.Select(f => $"{f.Id}({f.Cargo})"));
                result += "\n";
            }
            result += $"Fitness: {Fitness}\n";
            return result;
        }
    }
}