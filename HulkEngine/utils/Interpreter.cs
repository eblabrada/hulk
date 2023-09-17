class Interpreter : Expr.IVisitor<object>
{
  private readonly Parser parser;
  private readonly Env globals = new Env();
  private Env environment;
  private readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

  public Interpreter(Parser parser)
  {
    this.parser = parser;
    environment = globals;
  }

  public object interpret()
  {
    Expr tree = parser.parse();
    return evaluate(tree);
  }

  private object evaluate(Expr expr)
  {
    return expr.accept(this);
  }

  public object visitBinaryExpr(Expr.Binary expr)
  {
    dynamic left = evaluate(expr.left);
    dynamic right = evaluate(expr.right);

    switch (expr.oper.type)
    {
      case TokenType.PLUS:
        return left + right;
      case TokenType.MINUS:
        return left - right;
      case TokenType.MUL:
        return left * right;
      case TokenType.DIV:
        return left / right;
      case TokenType.POWER:
        return Math.Pow(left, right);
    }

    throw new Exception();
  }

  public object visitNumberExpr(Expr.Number expr)
  {
    return expr.value;
  }

  public object visitUnaryExpr(Expr.Unary expr)
  {
    dynamic value = evaluate(expr.right);

    switch (expr.oper.type)
    {
      case TokenType.PLUS:
        return +value;
      case TokenType.MINUS:
        return -value;
    }

    throw new Exception();
  }

  public object visitVariableExpr(Expr.Variable expr)
  {
    return lookUpVariable(expr.name, expr);
  }

  private object lookUpVariable(Token name, Expr expr)
  {
    if (locals.ContainsKey(expr))
    {
      int distance = locals[expr];
      return environment.getAt(distance, name.lexeme);
    }
    return globals.get(name);
  }

  public object visitAssignExpr(Expr.Assign expr)
  {
    dynamic value = evaluate(expr.value);
    if (locals.ContainsKey(expr))
    {
      int distance = locals[expr];
      environment.assignAt(distance, expr.name, value);
    }
    else
    {
      globals.assign(expr.name, value);
    }
    return value;
  }

  public object visitCompoundExpr(Expr.Compound expr)
  {
    return evaluate(expr.child);
  }
}