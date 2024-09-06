using DesafioProjetoHospedagem.Models;
using Newtonsoft.Json;

// Inicialização de listas
List<Pessoa> hospedes = new List<Pessoa>();
List<Suite> listaSuites = new List<Suite>();

// Função para adicionar um hóspede
void AdicionarHospede(List<Pessoa> hospedes)
{
    Console.WriteLine("Digite o nome do hóspede:");
    string nomeHospede = Console.ReadLine().ToUpper();
    Console.WriteLine("Digite o CPF do hóspede (Apenas números):");
    string cpfHospede = Console.ReadLine().ToLower();

    try
    {
        Pessoa hospede = new Pessoa(nomeHospede, cpfHospede);
        hospedes.Add(hospede);
        Console.WriteLine("Hóspede adicionado com sucesso!");
    }
    catch (Exception ex)
    {
        throw new Exception($"Erro ao adicionar hóspede: " + ex.Message);
    }
}

// Função para adicionar uma suíte
void AdicionarSuite(List<Suite> suites)
{
    bool adicionarMaisSuites = true;
    Console.Write("Deseja adicionar suítes? (s/n):");
    string resposta = Console.ReadLine();
    adicionarMaisSuites = resposta?.Trim().ToLower() == "s";
    if (adicionarMaisSuites)
    {

        Console.Write("Digite o tipo de suíte: ");
        string tipoSuite = Console.ReadLine();

        Console.Write("Digite a capacidade da suíte: ");
        if (!int.TryParse(Console.ReadLine(), out int capacidade) || capacidade <= 0)
        {
            Console.WriteLine("Capacidade inválida. Deve ser um número positivo.");
            return;
        }

        Console.Write("Digite o valor da diária: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal valorDiaria) || valorDiaria <= 0)
        {
            Console.WriteLine("Valor da diária inválido. Deve ser um número positivo.");
            return;
        }

        Suite suite = new Suite(tipoSuite: tipoSuite, capacidade: capacidade, valorDiaria: valorDiaria);
        suites.Add(suite);
        Console.WriteLine("Suíte adicionada com sucesso!");

        // Perguntar se deseja adicionar mais suítes
        while (adicionarMaisSuites)
        {
            Console.Write("Deseja adicionar mais suítes? (s/n): ");
            resposta = Console.ReadLine();
            adicionarMaisSuites = resposta?.Trim().ToLower() == "s";
            if (adicionarMaisSuites)
            {
                AdicionarSuite(suites);
            }
        }
    }
}
//RESERVA:
// Função para exibir opções e permitir seleção de hóspede
List<Pessoa> SelecionarHospede(List<Pessoa> hospedes)
{
    List<Pessoa> hospedesSelecionados = new List<Pessoa>();
    bool continuarSelecionando = true;

    while (continuarSelecionando)
    {
        Console.WriteLine("Selecione um hóspede:");
        for (int i = 0; i < hospedes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {hospedes[i].Nome} ({hospedes[i].Cpf})");
        };

        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= hospedes.Count)
        {
            hospedesSelecionados.Add(hospedes[index - 1]);
        }
        else
        {
            throw new ArgumentException("Hóspede inválido.");
        };
        Console.Write("Deseja selecionar mais hóspedes? (s/n): ");
        continuarSelecionando = Console.ReadLine()?.Trim().ToLower() == "s";
    };
    return hospedesSelecionados;
}

// Função para exibir opções e permitir seleção de suíte
Suite SelecionarSuite(List<Suite> suites)
{
    string conteudoArquivo = File.ReadAllText("Arquivos/Suites.json");
    suites = JsonConvert.DeserializeObject<List<Suite>>(conteudoArquivo);
    Console.WriteLine("Selecione uma suíte:");
    for (int i = 0; i < suites.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {suites[i].TipoSuite} - {suites[i].ValorDiaria:C} - Capacidade: {suites[i].Capacidade}");
    }

    if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= suites.Count)
    {
        return suites[index - 1];
    }
    else
    {
        throw new ArgumentException("Suíte inválida.");
    }
}

// Adicionar hóspedes e suítes se necessário
bool adicionarMaisHospedes = true;
while (adicionarMaisHospedes)
{
    AdicionarHospede(hospedes);
    Console.Write("Deseja adicionar mais hóspedes? (s/n): ");
    adicionarMaisHospedes = Console.ReadLine()?.Trim().ToLower() == "s";
    Console.Clear();
}

// Criar reserva
try
{
    // Selecionar hóspede e suíte
    List<Pessoa> hospedeEscolhido = SelecionarHospede(hospedes);
    Suite suiteEscolhida = SelecionarSuite(listaSuites);


    List<Reserva> reservasExistentes = new List<Reserva>();

    // Salvar a reserva em um arquivo JSON
    string arquivoReservas = "Arquivos/Reservas.json";
    if (File.Exists(arquivoReservas))
    {
        string conteudoArquivo = File.ReadAllText(arquivoReservas);
        reservasExistentes = JsonConvert.DeserializeObject<List<Reserva>>(conteudoArquivo) ?? new List<Reserva>();
    }
    else
    {
        reservasExistentes = new List<Reserva>();
    };
    // Criar a reserva
    Console.WriteLine("Digite o tempo que hospede permanecerá (em dias):");
    int diasHospedado = Convert.ToInt32(Console.ReadLine());
    Reserva novaReserva = new Reserva(diasReservados: diasHospedado);
    novaReserva.CadastrarSuite(suiteEscolhida);
    novaReserva.CadastrarHospedes(hospedeEscolhido);
    reservasExistentes.Add(novaReserva);
    string reservasJson = JsonConvert.SerializeObject(reservasExistentes, Formatting.Indented);
    File.WriteAllText(arquivoReservas, reservasJson);

    Console.WriteLine("Reserva salva com sucesso!");
    Console.WriteLine("Hóspede(s):");
    for (int i = 0; i < hospedeEscolhido.Count; i++)
    {
        Console.WriteLine($"{hospedeEscolhido[i].Nome}");
    }
    Console.WriteLine($"Suíte: {suiteEscolhida.TipoSuite}");
    Console.WriteLine($"Valor diária: {novaReserva.CalcularValorDiaria().ToString("C")}");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro: {ex.Message}");
}