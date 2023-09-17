
public class ConsoleLogger : ILogger
{
  public bool hadError { get; private set; }

  public bool hadRuntimeError { get; private set; }

  public void Error(int line, string where, string message)
  {
    Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
    hadError = true;
  }

  public void RuntimeError(RuntimeError error)
  {
    Console.WriteLine(error.Message + "\n[line " + error.token.line + "]");
    hadRuntimeError = true;
  }

  public void ResetError()
  {
    hadError = false;
  }

  public void ResetRuntimeError()
  {
    hadRuntimeError = false;
  }
}

