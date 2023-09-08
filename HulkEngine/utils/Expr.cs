public abstract class Expr
{

  public interface IVisitor<R>
  {
    R visitBinaryExpr(Binary expr);
    R visitNumberExpr(Number expr);
    R visitUnaryExpr(Unary expr);
  }

  public abstract R accept<R>(IVisitor<R> visitor);

  public class Binary : Expr
  {
    public readonly Expr left;
    public readonly Token oper;
    public readonly Expr right;

    public Binary(Expr left, Token oper, Expr right)
    {
      this.left = left;
      this.oper = oper;
      this.right = right;
    }

    public override R accept<R>(IVisitor<R> visitor)
    {
      return visitor.visitBinaryExpr(this);
    }
  }

  public class Number : Expr
  {
    public readonly Token token;
    public readonly double value;

    public Number(Token token)
    {
      this.token = token;
      this.value = double.Parse(token.lexeme);
    }

    public override R accept<R>(IVisitor<R> visitor)
    {
      return visitor.visitNumberExpr(this);
    }
  }

  public class Unary : Expr
  {
    public readonly Token oper;
    public readonly Expr right;

    public Unary(Token oper, Expr right)
    {
      this.oper = oper;
      this.right = right;
    }

    public override R accept<R>(IVisitor<R> visitor)
    {
      return visitor.visitUnaryExpr(this);
    }
  }
}