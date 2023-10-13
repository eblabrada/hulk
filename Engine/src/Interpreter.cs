using static TokenType;

public class Interpreter : Expr.IVisitor<object>
{
  private Atmosphere environment;
  private readonly ILogger logger;

  public Interpreter(ILogger logger)
  {
    this.logger = logger;
    this.environment = new Atmosphere(logger);
  }

  public string Interpret(Expr expr)
  {
    try
    {
      return Stringify(Evaluate(expr));
    }
    catch
    {
      if (!logger.hadRuntimeError)
        logger.RuntimeError(new RuntimeError("", "This line can't be interpreted."));
      return null;
    }
  }

  private object Evaluate(Expr expr)
  {
    if (!logger.hadError)
      return expr.Accept(this);
    return null;
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
        if (left is double) 
          left = left.ToString();
        if (right is double) 
          right = right.ToString();
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
        return Math.Pow((double)left, (double)right);
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

    logger.RuntimeError(new RuntimeError(condition.ToString(), "Condition must be a boolean expression."));
    return null;
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

      logger.RuntimeError(new RuntimeError(new AST().VisitLetInExpr(expr), "Unexpected error in `let-in` expression."));
      return null;
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
      logger.RuntimeError(new RuntimeError(expr.name.lexeme, "Expected function."));
      return null;
    }

    if (!environment.IsFunction(expr.name, expr.Arity))
    {
      logger.RuntimeError(new RuntimeError(expr.name.lexeme, "incorrect arity for this function."));
      return null;
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
      logger.RuntimeError(new RuntimeError(value.ToString(), "Must be number."));
    }
  }

  private void CheckBoolean(object value, int side = 0)
  {
    if (!(value is bool))
    {
      logger.RuntimeError(new RuntimeError(value.ToString(), "Must be boolean."));
    }
  }

  private void CheckString(object value, int side = 0)
  {
    if (!(value is string))
    {
      logger.RuntimeError(new RuntimeError(value.ToString(), "Must be string."));
    }
  }
}