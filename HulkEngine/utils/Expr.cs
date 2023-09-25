public abstract class Expr
{

  public interface IVisitor<R>
  {
    R VisitAssignExpr(Assign expr);
    R VisitBinaryExpr(Binary expr);
    R VisitCallExpr(Call expr);
    R VisitConditionalExpr(Conditional expr);
    R VisitFunctionExpr(Function expr);
    R VisitLetInExpr(LetIn expr);
    R VisitLiteralExpr(Literal expr);
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

  public class Call : Expr
  {
    public readonly Token name;
    public readonly List<Expr> parameters;

    public Call(Token name, List<Expr> parameters)
    {
      this.name = name;
      this.parameters = parameters;
    }

    public int Arity => parameters.Count;

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitCallExpr(this);
    }
  }

  public class Conditional : Expr
  {
    public readonly Expr condition;
    public readonly Expr thenBranch;
    public readonly Expr elseBranch;

    public Conditional(Expr condition, Expr thenBranch, Expr elseBranch)
    {
      this.condition = condition;
      this.thenBranch = thenBranch;
      this.elseBranch = elseBranch;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitConditionalExpr(this);
    }
  }

  public class Function : Expr
  {
    public readonly Token name;
    public readonly Expr body;
    public readonly List<Token> parameters;
    public readonly bool overwritable;

    public Function(Token name, List<Token> parameters, Expr body, bool overwritable = false)
    {
      this.name = name;
      this.parameters = parameters;
      this.body = body;
      this.overwritable = overwritable;
    }

    public int Arity => parameters.Count;

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitFunctionExpr(this);
    }
  }

  public class LetIn : Expr
  {
    public readonly List<Assign> assignments;
    public readonly Expr into;

    public LetIn(List<Assign> assignments, Expr into)
    {
      this.assignments = assignments;
      this.into = into;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitLetInExpr(this);
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