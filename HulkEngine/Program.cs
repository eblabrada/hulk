using System;

public class Hulk
{
  public static void Main()
  {
    while (true)
    {
      Console.Write("> ");
      string line = Console.ReadLine();
      if (line == null || line == "exit") break;
      run(line);
    }
  }

  private static void run(string src)
  {
    Scanner scanner = new Scanner(src);
    List<Token> tokens = scanner.scanTokens();

    Interpreter h = new Interpreter(tokens);
    Console.WriteLine(h.eval());


  }
}