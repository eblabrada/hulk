public class Env
{
  public readonly Env enclosing;
  private readonly Dictionary<string, object> values = new Dictionary<string, object>();

  public Env()
  {
    this.enclosing = null;
  }

  public Env(Env enclosing)
  {
    this.enclosing = enclosing;
  }

  public object get(Token name)
  {
    if (values.ContainsKey(name.lexeme))
    {
      return values[name.lexeme];
    }

    if (enclosing != null)
      return enclosing.get(name);

    throw new Exception();
  }

  public void assign(Token name, object value)
  {
    if (values.ContainsKey(name.lexeme))
    {
      values[name.lexeme] = value;
      return;
    }

    if (enclosing != null)
    {
      enclosing.assign(name, value);
      return;
    }

    throw new Exception();
  }

  public void declare(string name, object value)
  {
    values.Add(name, value);
  }

  public Env ancestor(int distance)
  {
    Env env = this;
    for (int i = 0; i < distance; i++)
    {
      env = env.enclosing;
    }
    return env;
  }

  public object getAt(int distance, string name)
  {
    return ancestor(distance).values[name];
  }

  public void assignAt(int distance, Token name, object value)
  {
    ancestor(distance).values[name.lexeme] = value;
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