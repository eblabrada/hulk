public class Env
{
  public readonly Env enclosing;
  private readonly Dictionary<string, object> values = new Dictionary<string, object>();

  public Env(Env enclosing = null)
  {
    this.enclosing = enclosing;
  }

  public void Define(string name, object value)
  {
    values[name] = value;
  }

  public object Get(Token name)
  {
    if (values.TryGetValue(name.lexeme, out var value))
    {
      return value;
    }

    if (enclosing != null)
    {
      return enclosing.Get(name);
    }

    throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
  }

  public object GetAt(int distance, string name)
  {
    return Ancestor(distance).values[name];
  }

  public void Assign(Token name, object value)
  {
    if (values.ContainsKey(name.lexeme))
    {
      values[name.lexeme] = value;
      return;
    }

    if (enclosing != null)
    {
      enclosing.Assign(name, value);
      return;
    }

    throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
  }

  public void AssignAt(int distance, Token name, object value)
  {
    Ancestor(distance).values[name.lexeme] = value;
  }

  public Env Ancestor(int distance)
  {
    Env env = this;
    for (int i = 0; i < distance; i++)
    {
      env = env.enclosing;
    }
    return env;
  }

  public override string? ToString()
  {
    string result = values.ToString();
    if (enclosing != null)
    {
      result = result + " -> " + enclosing.ToString();
    }
    return result;
  }

}