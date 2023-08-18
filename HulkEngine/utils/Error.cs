public static class Error
{
  public static void showError(ErrorType type)
  {
    Console.WriteLine($"{type} ERROR:");
    throw new Exception("");
  }
}