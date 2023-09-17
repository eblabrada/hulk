public interface ILogger
{
  bool hadError { get; }
  bool hadRuntimeError { get; }

  void Error(int line, string where, string message);
  void RuntimeError(RuntimeError error);
  void ResetError();
  void ResetRuntimeError();
}

public static class LoggerFunctions
{
  public static void Error(this ILogger logger, int line, string message)
  {
    logger.Error(line, "", message);
  }

  public static void Error(this ILogger logger, Token token, string message)
  {
    if (token.type == TokenType.EOF)
    {
      logger.Error(token.line, " at end", message);
    }
    else
    {
      logger.Error(token.line, $" at '{token.lexeme}'", message);
    }
  }
}