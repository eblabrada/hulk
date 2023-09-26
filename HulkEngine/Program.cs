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

    // foreach (var x in tokens)
    // {
    //   Console.WriteLine(x);
    // }

    var parser = new Parser(logger, tokens);
    var interpreter = new Interpreter();

    var result = interpreter.Interpret(parser.Parse());
  
    if (logger.hadError) return Task.CompletedTask;

    return Task.CompletedTask;
  }
}