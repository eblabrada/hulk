public class RuntimeError : Exception
{
  public readonly string tokenStr;

  public RuntimeError(string tokenStr, string message) : base(message)
  {
    this.tokenStr = tokenStr;
  }
}