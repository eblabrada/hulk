using System;
using static TokenType;

public class Hulk
{
  private static readonly ConsoleLogger logger = new ConsoleLogger();
  private static readonly Interpreter interpreter = new Interpreter(logger);
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
      logger.ResetRuntimeError();
    }
    return Task.CompletedTask;
  }

  private Task RunAsync(string source)
  {
    var scanner = new Scanner(logger, source);
    var tokens = scanner.ScanTokens();
    
    if (logger.hadError || logger.hadRuntimeError) 
      return Task.CompletedTask;

    var parser = new Parser(logger, tokens);
    var parseResult = parser.Parse();

    if (logger.hadError || logger.hadRuntimeError) 
      return Task.CompletedTask;

    var interpretResult = interpreter.Interpret(parseResult);

    if (logger.hadError || logger.hadRuntimeError) 
      return Task.CompletedTask;

    return Task.CompletedTask;
  }
}