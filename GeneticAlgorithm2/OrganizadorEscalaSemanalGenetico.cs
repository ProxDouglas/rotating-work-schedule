
namespace rotating_work_schedule.GeneticAlgorithm2
{

    public class OrganizadorEscalaSemanalGenetico
    {
        private readonly ConfiguracaoEscala _config;
        private readonly Random _random = new Random();

        public OrganizadorEscalaSemanalGenetico(ConfiguracaoEscala config)
        {
            _config = config;
        }

        public EscalaSemanal EncontrarMelhorEscala()
        {
            var populacao = InicializarPopulacao();

            for (int geracao = 0; geracao < _config.NumeroGeracoes; geracao++)
            {
                populacao = EvoluirPopulacao(populacao);

                if (geracao % 100 == 0)
                {
                    Console.WriteLine($"Geração {geracao}: Melhor fitness = {populacao.Max(e => e.Fitness)}");
                }
            }

            return populacao.OrderByDescending(e => e.Fitness).First();
        }

        private List<EscalaSemanal> InicializarPopulacao()
        {
            var populacao = new List<EscalaSemanal>();

            for (int i = 0; i < _config.TamanhoPopulacao; i++)
            {
                var escala = CriarEscalaVazia();

                foreach (var funcionario in _config.Funcionarios)
                {
                    AlocarFuncionarioComHorarioContinuo(escala, funcionario);
                }

                CalcularFitness(escala);
                populacao.Add(escala);
            }

            return populacao;
        }

        private EscalaSemanal CriarEscalaVazia()
        {
            var escala = new EscalaSemanal
            {
                Alocacoes = new Dictionary<(DayOfWeek Dia, TimeSpan Horario), List<Funcionario>>()
            };

            foreach (var dia in _config.DiasDaSemana)
            {
                foreach (var horario in _config.HorariosDiarios)
                {
                    escala.Alocacoes[(dia, horario)] = new List<Funcionario>();
                }
            }

            return escala;
        }

        private void AlocarFuncionarioComHorarioContinuo(EscalaSemanal escala, Funcionario funcionario)
        {
            var diasDisponiveis = _config.DiasDaSemana
                .Except(funcionario.DiasFolga)
                .OrderBy(x => _random.Next())
                .Take(_config.DiasTrabalhadosPorSemana)
                .ToList();

            foreach (var dia in diasDisponiveis)
            {
                // Encontra um bloco contínuo de 8 horas disponível
                var horariosDia = _config.HorariosDiarios
                    .Where(h => escala.Alocacoes[(dia, h)].Count < _config.Funcionarios.Count) // Limite não atingido
                    .OrderBy(h => h)
                    .ToList();

                // Tenta encontrar um bloco contínuo de 8 horas
                var horarioInicio = EncontrarBlocoContinuo(horariosDia, escala, dia, funcionario);

                if (horarioInicio != null)
                {
                    // Aloca as 8 horas contínuas
                    for (int i = 0; i < _config.HorasDiariasPorFuncionario; i++)
                    {
                        var horarioAtual = horarioInicio.Value.Add(TimeSpan.FromHours(i));
                        if (_config.HorariosDiarios.Contains(horarioAtual))
                        {
                            escala.Alocacoes[(dia, horarioAtual)].Add(funcionario);
                        }
                    }
                }
            }
        }

        private TimeSpan? EncontrarBlocoContinuo(List<TimeSpan> horariosDisponiveis, EscalaSemanal escala, DayOfWeek dia, Funcionario funcionario)
        {
            TimeSpan horarioInicial;
            bool blocoValido = true;
            // Verifica se há um bloco contínuo de 8 horas disponível
            // for (int i = 0; i <= horariosDisponiveis.Count - _config.HorasDiariasPorFuncionario; i++)
            // {
                horarioInicial = horariosDisponiveis[_random.Next(0, horariosDisponiveis.Count - _config.HorasDiariasPorFuncionario)];
                blocoValido = true;

                // Verifica se as próximas 7 horas também estão disponíveis
                for (int j = 1; j < _config.HorasDiariasPorFuncionario; j++)
                {
                    var horarioVerificar = horarioInicial.Add(TimeSpan.FromHours(j));
                    if (!horariosDisponiveis.Contains(horarioVerificar))
                    {
                        blocoValido = false;
                        break;
                    }
                }

                if (blocoValido)
                {
                    return horarioInicial;
                }
            // }

            // Se não encontrou bloco perfeito, tenta o maior possível (com penalização no fitness)
            for (int horas = _config.HorasDiariasPorFuncionario; horas >= 4; horas--)
            {
                for (int i = 0; i <= horariosDisponiveis.Count - horas; i++)
                {
                    horarioInicial = horariosDisponiveis[i];
                    blocoValido = true;

                    for (int j = 1; j < horas; j++)
                    {
                        var horarioVerificar = horarioInicial.Add(TimeSpan.FromHours(j));
                        if (!horariosDisponiveis.Contains(horarioVerificar))
                        {
                            blocoValido = false;
                            break;
                        }
                    }

                    if (blocoValido)
                    {
                        return horarioInicial;
                    }
                }
            }

            return null;
        }

        private void CalcularFitness(EscalaSemanal escala)
        {
            double fitness = 1000; // Valor base

            // 1. Verificar requisitos por horário
            foreach (var alocacao in escala.Alocacoes)
            {
                var funcionariosNoHorario = alocacao.Value;
                var gruposPorCargo = funcionariosNoHorario.GroupBy(f => f.Cargo);

                foreach (var grupo in gruposPorCargo)
                {
                    var cargo = grupo.Key;
                    var quantidade = grupo.Count();

                    if (_config.RequisitosPorHorario.TryGetValue(cargo, out var requisitos))
                    {
                        // Penalização por não atender ao mínimo
                        if (quantidade < requisitos.Min)
                        {
                            fitness -= (requisitos.Min - quantidade) * 10;
                        }

                        // Penalização por exceder o máximo
                        if (quantidade > requisitos.Max)
                        {
                            fitness -= (quantidade - requisitos.Max) * 10;
                        }
                    }
                }

                // Penalização por horário sem cobertura
                if (!funcionariosNoHorario.Any())
                {
                    fitness -= 20;
                }
            }

            // 2. Verificar jornada diária dos funcionários (contínua e completa)
            var horasPorDiaPorFuncionario = new Dictionary<(int FuncionarioId, DayOfWeek Dia), (int Horas, bool Continuo)>();

            foreach (var funcionario in _config.Funcionarios)
            {
                foreach (var dia in _config.DiasDaSemana)
                {
                    horasPorDiaPorFuncionario[(funcionario.Id, dia)] = (0, true);
                }
            }

            foreach (var funcionario in _config.Funcionarios)
            {
                foreach (var dia in _config.DiasDaSemana)
                {
                    var horariosTrabalhados = escala.Alocacoes
                        .Where(a => a.Key.Dia == dia && a.Value.Contains(funcionario))
                        .Select(a => a.Key.Horario)
                        .OrderBy(h => h)
                        .ToList();

                    if (horariosTrabalhados.Any())
                    {
                        // Verifica se é contínuo
                        bool continuo = true;
                        for (int i = 1; i < horariosTrabalhados.Count; i++)
                        {
                            if (horariosTrabalhados[i] != horariosTrabalhados[i - 1].Add(TimeSpan.FromHours(1)))
                            {
                                continuo = false;
                                break;
                            }
                        }

                        horasPorDiaPorFuncionario[(funcionario.Id, dia)] = (horariosTrabalhados.Count, continuo);
                    }
                }
            }

            foreach (var kvp in horasPorDiaPorFuncionario)
            {
                var funcionario = _config.Funcionarios.First(f => f.Id == kvp.Key.FuncionarioId);

                // Se o funcionário não deveria trabalhar neste dia (folga)
                if (funcionario.DiasFolga.Contains(kvp.Key.Dia))
                {
                    if (kvp.Value.Horas > 0)
                    {
                        fitness -= kvp.Value.Horas * 15; // Penalização alta por alocação em dia de folga
                    }
                }
                else // Dia normal de trabalho
                {
                    // Penalização por não trabalhar as horas diárias completas
                    if (kvp.Value.Horas > 0 && kvp.Value.Horas != _config.HorasDiariasPorFuncionario)
                    {
                        fitness -= Math.Abs(kvp.Value.Horas - _config.HorasDiariasPorFuncionario) * 5;
                    }

                    // Penalização por horário não contínuo
                    if (kvp.Value.Horas > 1 && !kvp.Value.Continuo)
                    {
                        fitness -= 20; // Penalização alta por horário fragmentado
                    }
                }
            }

            // 3. Verificar jornada semanal dos funcionários
            foreach (var funcionario in _config.Funcionarios)
            {
                var horasSemanais = horasPorDiaPorFuncionario
                    .Where(x => x.Key.FuncionarioId == funcionario.Id)
                    .Sum(x => x.Value.Horas);

                var horasEsperadas = _config.HorasDiariasPorFuncionario * _config.DiasTrabalhadosPorSemana;

                if (horasSemanais != horasEsperadas)
                {
                    fitness -= Math.Abs(horasSemanais - horasEsperadas) * 3;
                }
            }

            escala.Fitness = fitness;
        }

        private List<EscalaSemanal> EvoluirPopulacao(List<EscalaSemanal> populacao)
        {
            var novaPopulacao = new List<EscalaSemanal>();

            // Elitismo: mantém os 10% melhores indivíduos
            var melhores = populacao.OrderByDescending(e => e.Fitness).Take(_config.TamanhoPopulacao / 10).ToList();
            novaPopulacao.AddRange(melhores.Select(e => e.Clone()));

            // Preenche o resto da população com cruzamento e mutação
            while (novaPopulacao.Count < _config.TamanhoPopulacao)
            {
                var pai1 = SelecionarPorTorneio(populacao);
                var pai2 = SelecionarPorTorneio(populacao);

                var filho = Cruzar(pai1, pai2);
                Mutar(filho);

                CalcularFitness(filho);
                novaPopulacao.Add(filho);
            }

            return novaPopulacao;
        }

        private EscalaSemanal SelecionarPorTorneio(List<EscalaSemanal> populacao)
        {
            var torneio = populacao.OrderBy(x => _random.Next()).Take(5).ToList();
            return torneio.OrderByDescending(e => e.Fitness).First().Clone();
        }

        private EscalaSemanal Cruzar(EscalaSemanal pai1, EscalaSemanal pai2)
        {
            var filho = CriarEscalaVazia();

            // Para cada funcionário, decide de qual pai herdar a escala
            foreach (var funcionario in _config.Funcionarios)
            {
                var escolherDoPai1 = _random.Next(2) == 0;
                var paiEscolhido = escolherDoPai1 ? pai1 : pai2;

                // Copia as alocações do pai escolhido para este funcionário
                foreach (var alocacao in paiEscolhido.Alocacoes)
                {
                    if (alocacao.Value.Contains(funcionario))
                    {
                        // Verifica se não excede o limite de funcionários no horário
                        if (filho.Alocacoes[alocacao.Key].Count < _config.Funcionarios.Count)
                        {
                            filho.Alocacoes[alocacao.Key].Add(funcionario);
                        }
                    }
                }
            }

            return filho;
        }

        private void Mutar(EscalaSemanal escala)
        {
            if (_random.NextDouble() > _config.TaxaMutacao)
                return;

            // Seleciona um funcionário aleatório para mutação
            var funcionario = _config.Funcionarios[_random.Next(_config.Funcionarios.Count)];

            // Remove todas as alocações deste funcionário
            foreach (var alocacao in escala.Alocacoes)
            {
                alocacao.Value.RemoveAll(f => f.Id == funcionario.Id);
            }

            // Realoca com horários contínuos
            AlocarFuncionarioComHorarioContinuo(escala, funcionario);
        }
    }
}