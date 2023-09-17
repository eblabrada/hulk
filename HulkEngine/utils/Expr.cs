public abstract class Expr
{

  public interface IVisitor<R>
  {
    R VisitAssignExpr(Assign expr);
    R VisitBinaryExpr(Binary expr);
    R VisitCallExpr(Call expr);
    R VisitGetExpr(Get expr);
    R VisitGroupingExpr(Grouping expr);
    R VisitLiteralExpr(Literal expr);
    R VisitLogicalExpr(Logical expr);
    R VisitSetExpr(Set expr); 
    R VisitUnaryExpr(Unary expr);
    R VisitVariableExpr(Variable expr);
  }

  public abstract R Accept<R>(IVisitor<R> visitor);

  public class Assign : Expr
  {
    public readonly Token name;
    public readonly Expr value;

    public Assign(Token name, Expr value)
    {
      this.name = name;
      this.value = value;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitAssignExpr(this);
    }
  }

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

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitBinaryExpr(this);
    }
  }

  public class Call : Expr {
    public readonly Expr calle;
    public readonly Token paren;
    public readonly List<Expr> arguments;

    public Call(Expr calle, Token paren, List<Expr> arguments) {
      this.calle = calle;
      this.paren = paren;
      this.arguments = arguments;
    }

    public override R Accept<R>(IVisitor<R> visitor) {
      return visitor.VisitCallExpr(this);
    }
  }

  public class Get : Expr
  {
    public readonly Expr obj;
    public readonly Token name;

    public Get(Expr obj, Token name)
    {
      this.obj = obj;
      this.name = name;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitGetExpr(this);
    }
  }

  public class Grouping : Expr
  {
    public readonly Expr expression;

    public Grouping(Expr expression)
    {
      this.expression = expression;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitGroupingExpr(this);
    }
  }

  public class Literal : Expr
  {
    public readonly object value;
    public Literal(object value)
    {
      this.value = value;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitLiteralExpr(this);
    }
  }

  public class Logical : Expr
  {
    public readonly Expr left;
    public readonly Token oper;
    public readonly Expr right;

    public Logical(Expr left, Token oper, Expr right)
    {
      this.left = left;
      this.oper = oper;
      this.right = right;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitLogicalExpr(this);
    }
  }

  public class Set : Expr
  {
    public readonly Expr obj;
    public readonly Token name;
    public readonly Expr value;

    public Set(Expr obj, Token name, Expr value)
    {
      this.obj = obj;
      this.name = name;
      this.value = value;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitSetExpr(this);
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

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitUnaryExpr(this);
    }
  }

  public class Variable : Expr
  {
    public readonly Token name;

    public Variable(Token name)
    {
      this.name = name;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitVariableExpr(this);
    }
  }
}