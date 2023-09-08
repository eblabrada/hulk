class Interpreter : Expr.IVisitor<object>
{
  private readonly Parser parser;

  public Interpreter(Parser parser)
  {
    this.parser = parser;
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

  public object interpret()
  {
    Expr tree = parser.parse();
    return evaluate(tree);
  }
}