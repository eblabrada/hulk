public static class Error
{
  public static void showError(int line, string message)
  {
    report(line, "", message);
  }

  private static void report(int line, string where, string message)
  {
    throw new Exception($"[line {line}] Error{where}: {message}");
  }

  public static void showError(Token token, string message)
  {
    if (token.type == TokenType.EOF)
    {
      report(token.line, " at end", message);
    }
    else
    {
      report(token.line, $" at '{token.lexeme}'", message);
    }
  }

}