public class Atmosphere
{
  private Dictionary<string, List<object>> varGlobal;
  private Dictionary<string, Dictionary<int, Expr.Function>> funGlobal;

  private HashSet<(string, int)> builtins = new HashSet<(string, int)>()
  {
    ("rand", 0),
    ("cos", 1),
    ("exp", 1),
    ("print", 1),
    ("sin", 1),
    ("sqrt", 1),
    ("log", 2)
  };

  public Atmosphere()
  {
    this.varGlobal = new Dictionary<string, List<object>>();
    this.funGlobal = new Dictionary<string, Dictionary<int, Expr.Function>>();
  }

  public object Get(Token name)
  {
    try
    {
      if (IsFunction(name))
      {
        throw new Exception("Missing '(' after function's call.");
      }
      return varGlobal[name.lexeme].Last();
    }
    catch (KeyNotFoundException)
    {
      throw new Exception($"Variable {name} not be declared.");
    }
  }

  public void Set(Token name, object value)
  {
    if (IsFunction(name))
    {
      throw new Exception("A function name can't be used as name of a variable.");
    }

    if (!varGlobal.ContainsKey(name.lexeme))
    {
      varGlobal.Add(name.lexeme, new List<object>());
    }

    varGlobal[name.lexeme].Add(value);
  }

  public void Remove(Token name)
  {
    varGlobal[name.lexeme].RemoveAt(varGlobal[name.lexeme].Count - 1);
    if (varGlobal[name.lexeme].Count == 0)
    {
      varGlobal.Remove(name.lexeme);
    }
  }

  public void FunDeclare(Expr.Function fun)
  {
    string name = fun.name.lexeme;
    int arity = fun.Arity;

    if (IsBuiltin(name, arity))
    {
      throw new Exception($"{name} is a built-in function.");
    }

    if (funGlobal.ContainsKey(name))
    {
      Dictionary<int, Expr.Function> table = funGlobal[name];
      if (table.ContainsKey(arity))
      {
        throw new Exception("Function already exists.");
      }
      else
      {
        funGlobal[name].Add(arity, fun);
      }
    }
    else
    {
      Dictionary<int, Expr.Function> table = new Dictionary<int, Expr.Function>();
      table.Add(arity, fun);
      funGlobal.Add(name, table);
    }
  }

  private bool IsOverwritable(Expr.Function fun)
  {
    return fun.overwritable;
  }

  public bool IsFunction(Token name)
  {
    if (funGlobal.ContainsKey(name.lexeme))
    {
      return true;
    }
    return false;
  }

  public bool IsFunction(Token name, int arity)
  {
    if (IsFunction(name))
    {
      return funGlobal[name.lexeme].ContainsKey(arity);
    }
    return false;
  }

  public List<Token> GetParameters(string name, int arity)
  {
    return funGlobal[name][arity].parameters;
  }

  public Expr GetBody(string name, int arity)
  {
    return funGlobal[name][arity].body;
  }

  public List<int> GetAritys(string fun)
  {
    return funGlobal[fun].Keys.ToList();
  }

  public bool IsBuiltin(string name, int arity)
  {
    return builtins.Contains((name, arity));
  }

}