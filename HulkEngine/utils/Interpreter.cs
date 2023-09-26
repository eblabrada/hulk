using static TokenType;

public class Interpreter : Expr.IVisitor<object>
{
  private Env environment = new Env();

  public Interpreter() { }

  public string Interpret(Expr expr)
  {
    try
    {
      return Stringify(Evaluate(expr));
    }
    catch
    {
      throw new Exception("Interpreter error.");
    }
  }

  private object Evaluate(Expr expr)
  {
    return expr.Accept(this);
  }

  public object VisitLiteralExpr(Expr.Literal expr)
  {
    return expr.value;
  }

  public object VisitUnaryExpr(Expr.Unary expr)
  {
    object value = Evaluate(expr.right);
    switch (expr.oper.type)
    {
      case MINUS:
        CheckNumber(value);
        return -(double)value;
      case NOT:
        CheckBoolean(value);
        return !IsTrue(value);
    }
    return null;
  }

  public object VisitBinaryExpr(Expr.Binary expr)
  {
    object left = Evaluate(expr.left);
    object right = Evaluate(expr.right);

    switch (expr.oper.type)
    {
      case PLUS:
        CheckNumber(left, -1);
        CheckNumber(right, +1);
        return (double)left + (double)right;
      case MINUS:
        CheckNumber(left, -1);
        CheckNumber(right, +1);
        return (double)left - (double)right;
      case CONCAT:
        CheckString(left, -1);
        CheckString(right, +1);
        return String.Concat((string)left, (string)right);
      case MUL:
        CheckNumber(left, -1);
        CheckNumber(right, +1);
        return (double)left * (double)right;
      case MOD:
        CheckNumber(left, -1);
        CheckNumber(right, +1);
        return (double)left % (double)right;
      case DIV:
        CheckNumber(left, -1);
        CheckNumber(right, +1);
        return (double)left / (double)right;
      case POWER:
        CheckNumber(left, -1);
        CheckNumber(right, +1);
        return (double)Math.Pow((double)left, (double)right);
      case LESS:
        CheckNumber(left, -1);
        CheckNumber(right, +1);
        return (double)left < (double)right;
      case LESS_EQUAL:
        CheckNumber(left, -1);
        CheckNumber(right, +1);
        return (double)left <= (double)right;
      case GREATER:
        CheckNumber(left, -1);
        CheckNumber(right, +1);
        return (double)left > (double)right;
      case GREATER_EQUAL:
        CheckNumber(left, -1);
        CheckNumber(right, +1);
        return (double)left >= (double)right;
      case EQUAL_EQUAL:
        return IsEqual(left, right);
      case NOT_EQUAL:
        return !IsEqual(left, right);
      case AND:
        CheckBoolean(left, -1);
        CheckBoolean(right, +1);
        return (bool)left & (bool)right;
      case OR:
        CheckBoolean(left, -1);
        CheckBoolean(right, +1);
        return (bool)left | (bool)right;
    }

    return null;
  }

  public object VisitConditionalExpr(Expr.Conditional expr)
  {
    object condition = Evaluate(expr.condition);
    if (condition is bool)
    {
      if (IsTrue(condition))
      {
        return Evaluate(expr.thenBranch);
      }
      else
      {
        return Evaluate(expr.elseBranch);
      }
    }
    throw new Exception("Condition must be boolean expression.");
  }

  public object VisitLetInExpr(Expr.LetIn expr)
  {
    List<Token> values = new List<Token>();
    try
    {
      foreach (var assignment in expr.assignments)
      {
        environment.Set(assignment.name, Evaluate(assignment.value));
        values.Add(assignment.name);
      }

      object value = Evaluate(expr.into);

      foreach (var assigment in expr.assignments)
      {
        environment.Remove(assigment.name);
      }

      return value;
    }
    catch
    {
      foreach (var name in values)
      {
        environment.Remove(name);
      }

      throw new Exception("Error in let-in.");
    }
  }

  public object VisitAssignExpr(Expr.Assign expr)
  {
    throw new NotImplementedException();
  }

  public object VisitVariableExpr(Expr.Variable expr)
  {
    return environment.Get(expr.name);
  }

  public object VisitFunctionExpr(Expr.Function expr)
  {
    environment.FunDeclare(expr);
    return null;
  }

  public object VisitCallExpr(Expr.Call expr)
  {
    if (environment.IsBuiltin(expr.name.lexeme, expr.Arity))
    {
      List<object> parameters = new List<object>();
      foreach (var param in expr.parameters)
      {
        parameters.Add(Evaluate(param));
      }

      switch (expr.name.lexeme)
      {
        case "rand":
          Random result = new Random();
          return result.NextDouble();
        case "cos":
          return (double)Math.Cos((double)parameters[0]);
        case "sin":
          return (double)Math.Sin((double)parameters[0]);
        case "exp":
          return (double)Math.Exp((double)parameters[0]);
        case "print":
          Console.WriteLine(parameters[0]);
          return parameters[0];
        case "sqrt":
          return (double)Math.Sqrt((double)parameters[0]);
        case "log":
          return (double)Math.Log((double)parameters[1], (double)parameters[0]);
        default:
          throw new NotImplementedException();
      }
    }

    if (!environment.IsFunction(expr.name))
    {
      throw new Exception("Error, expected function.");
    }

    if (!environment.IsFunction(expr.name, expr.Arity))
    {
      throw new Exception("incorrect arity for this function.");
    }

    var assignments = new List<Expr.Assign>();
    var args = environment.GetParameters(expr.name.lexeme, expr.Arity);
    var _params = expr.parameters;
    for (int i = 0; i < expr.Arity; i++)
    {
      assignments.Add(new Expr.Assign(args[i], _params[i]));
    }

    var funCall = new Expr.LetIn(assignments, environment.GetBody(expr.name.lexeme, expr.Arity));
    return Evaluate(funCall);
  }

  private string Stringify(object value)
  {
    if (value == null) return null;
    return value.ToString();
  }

  private bool IsEqual(object left, object right)
  {
    return Equals(left, right);
  }

  private bool IsTrue(object value)
  {
    if (value == null) return false;
    if (value is bool) return (bool)value;
    return true;
  }

  private void CheckNumber(object value, int side = 0)
  {
    if (!(value is double))
    {
      throw new Exception("Must be number.");
    }
  }

  private void CheckBoolean(object value, int side = 0)
  {
    if (!(value is bool))
    {
      throw new Exception("Must be boolean.");
    }
  }

  private void CheckString(object value, int side = 0)
  {
    if (!(value is string))
    {
      throw new Exception("Must be string.");
    }
  }

  private string GetType(object value)
  {
    if (value is double) return "Number";
    return value.GetType().Name;
  }
}