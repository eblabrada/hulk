public interface ILogger
{
  bool hadError { get; }
  bool hadRuntimeError { get; }

  void Error(string type, int line, int column, string where, string message);
  void RuntimeError(RuntimeError error);
  void ResetError();
  void ResetRuntimeError();
}

public static class LoggerFunctions
{
  public static void Error(this ILogger logger, string type, int line, int column, string message)
  {
    logger.Error(type, line, column, "", message);
  }

  public static void Error(this ILogger logger, string type, Token token, string message)
  {
    if (token.type == TokenType.EOF)
    {
      logger.Error(type, token.line, token.column, " at end", message);
    }
    else
    {
      logger.Error(type, token.line, token.column, $" at `{token.lexeme}`", message);
    }
  }
}