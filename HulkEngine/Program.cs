using System;

public class Hulk
{
  private readonly ConsoleLogger logger = new ConsoleLogger();

  public Hulk() { }

  public static Task Main(string[] args)
  {
    return new Hulk().RunPromptAsync();
  }

  private Task RunPromptAsync()
  {
    while (true)
    {
      Console.Write("> ");
      string line = Console.ReadLine();
      if (line == null || line == "exit")
      {
        break;
      }
      RunAsync(line);
      logger.ResetError();
    }
    return Task.CompletedTask;
  }

  private Task RunAsync(string source)
  {
    var scanner = new Scanner(logger, source);
    var tokens = scanner.ScanTokens();

    var parser = new Parser(logger, tokens);
    var statements = parser.Parse();

    if (logger.hadError) return Task.CompletedTask;

    var astPrinter = new AST();
    foreach (var statement in statements)
    {
      Console.WriteLine(astPrinter.Print(statement));
    }

    return Task.CompletedTask;
  }
}