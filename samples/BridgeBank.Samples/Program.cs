using BridgeBank.Samples;

while (true)
{
    Console.Clear();
    Console.WriteLine("BridgeBank - Exemplos de Utilização");
    Console.WriteLine(new string('=', 50));
    Console.WriteLine();
    Console.WriteLine("  1. Leitura de extractos bancários (Parsers)");
    Console.WriteLine("  2. Reconciliação bancária (Core)");
    Console.WriteLine("  3. Geração de ficheiros de pagamento (Generators)");
    Console.WriteLine();
    Console.WriteLine("  0. Sair");
    Console.WriteLine();
    Console.Write("Escolha uma opção: ");

    string? opcao = Console.ReadLine()?.Trim();

    Console.WriteLine();

    switch (opcao)
    {
        case "1":
            ExemploParsers.Executar();
            break;
        case "2":
            ExemploReconciliacao.Executar();
            break;
        case "3":
            ExemploGeradores.Executar();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }

    Console.WriteLine();
    Console.Write("Prima qualquer tecla para voltar ao menu...");
    Console.ReadKey(true);
}