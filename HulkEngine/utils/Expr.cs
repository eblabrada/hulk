public abstract class Expr
{
  public interface IVisitor<R>
  {
    R visitBinaryExpr(Binary expr);
    R visitLiteralExpr(Literal expr);
    R visitLogicalExpr(Logical expr);
    R visitUnaryExpr(Unary expr);
    R visitVariableExpr(Variable expr);
  }

  public class Binary : Expr
  {
    public readonly Expr left, right;
    public readonly Token op;

    Binary(Expr left, Token op, Expr right)
    {
      this.left = left;
      this.op = op;
      this.right = right;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.visitBinaryExpr(this);
    }
  }

  public class Literal : Expr
  {
    public readonly Object value;
    Literal(Object value)
    {
      this.value = value;
    }
    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.visitLiteralExpr(this);
    }
  }
  public class Logical : Expr
  {
    public readonly Expr left, right;
    public readonly Token op;

    Logical(Expr left, Token op, Expr right)
    {
      this.left = left;
      this.op = op;
      this.right = right;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.visitLogicalExpr(this);
    }
  }
  public class Unary : Expr
  {
    public readonly Token op;
    public readonly Expr right;

    Unary(Token op, Expr right)
    {
      this.op = op;
      this.right = right;
    }
    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.visitUnaryExpr(this);
    }
  }
  public class Variable : Expr
  {
    public readonly Token name;
    Variable(Token name)
    {
      this.name = name;
    }
    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.visitVariableExpr(this);
    }
  }
  public class Assign : Expr
  {
    public readonly Token name;
    public readonly Expr value;
    Assign(Token name, Expr value)
    {
      this.name = name;
      this.value = value;
    }
    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.visitAssignExpr(this);
    }
  }
  public abstract R Accept<R>(IVisitor<R> visitor);
}

