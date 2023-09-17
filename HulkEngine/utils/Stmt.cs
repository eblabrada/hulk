public abstract class Stmt
{
  public interface IVisitor<R>
  {
    R VisitBlockStmt(Block stmt);
    R VisitExpressionStmt(Expression stmt);
    R VisitFunctionStmt(Function stmt);
    R VisitIfStmt(If stmt);
    R VisitPrintStmt(Print stmt);
    R VisitReturnStmt(Return stmt);
    R VisitVarStmt(Var stmt);
  }

  public abstract R Accept<R>(IVisitor<R> visitor);

  public class Block : Stmt
  {
    public readonly List<Stmt> statements;

    public Block(List<Stmt> statements)
    {
      this.statements = statements;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitBlockStmt(this);
    }
  }

  public class Expression : Stmt
  {
    public readonly Expr expression;

    public Expression(Expr expression)
    {
      this.expression = expression;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitExpressionStmt(this);
    }
  }

  public class Function : Stmt
  {
    public readonly Token name;
    public readonly List<Token> parameters;
    public readonly List<Stmt> body;

    public Function(Token name, List<Token> parameters, List<Stmt> body)
    {
      this.name = name;
      this.parameters = parameters;
      this.body = body;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitFunctionStmt(this);
    }
  }

  public class If : Stmt
  {
    public readonly Expr condition;
    public readonly Stmt thenBranch;
    public readonly Stmt elseBranch;

    public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
    {
      this.condition = condition;
      this.thenBranch = thenBranch;
      this.elseBranch = elseBranch;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitIfStmt(this);
    }
  }

  public class Print : Stmt
  {
    public readonly Expr expression;

    public Print(Expr expression)
    {
      this.expression = expression;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitPrintStmt(this);
    }
  }

  public class Return : Stmt
  {
    public readonly Token keyword;
    public readonly Expr value;

    public Return(Token keyword, Expr value)
    {
      this.keyword = keyword;
      this.value = value;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitReturnStmt(this);
    }
  }

  public class Var : Stmt
  {
    public readonly Token name;
    public readonly Expr initializer;

    public Var(Token name, Expr initializer)
    {
      this.name = name;
      this.initializer = initializer;
    }

    public override R Accept<R>(IVisitor<R> visitor)
    {
      return visitor.VisitVarStmt(this);
    }
  }
}