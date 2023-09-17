using System.Diagnostics.Contracts;
using System.Net.Mail;
using System.Text;

public class AST : Expr.IVisitor<string>, Stmt.IVisitor<string>
{
  public string Print(Expr expr)
  {
    return expr.Accept(this);
  }

  public string Print(Stmt stmt)
  {
    return stmt.Accept(this);
  }

  public string VisitAssignExpr(Expr.Assign expr)
  {
    return Parenthesize2("=", expr.name.lexeme, expr.value);
  }

  public string VisitBinaryExpr(Expr.Binary expr)
  {
    return Parenthesize(expr.oper.lexeme, expr.left, expr.right);
  }

  public string VisitBlockStmt(Stmt.Block stmt)
  {
    var builder = new StringBuilder();
    builder.Append("(block ");

    foreach (var statement in stmt.statements)
    {
      builder.Append(statement.Accept(this));
    }

    builder.Append(")");
    return builder.ToString();
  }

  public string VisitCallExpr(Expr.Call expr)
  {
    return Parenthesize2("call", expr.calle, expr.arguments);
  }

  public string VisitExpressionStmt(Stmt.Expression stmt)
  {
    return Parenthesize(";", stmt.expression);
  }

  public string VisitFunctionStmt(Stmt.Function stmt)
  {
    var builder = new StringBuilder();
    builder.Append("(fun" + stmt.name.lexeme + "(");

    foreach (var param in stmt.parameters)
    {
      if (param != stmt.parameters[0]) builder.Append(" ");
      builder.Append(param.lexeme);
    }

    builder.Append(") ");

    foreach (var body in stmt.body)
    {
      builder.Append(body.Accept(this));
    }

    builder.Append(")");
    return builder.ToString();
  }

  public string VisitGetExpr(Expr.Get expr)
  {
    return Parenthesize2(".", expr.obj, expr.name.lexeme);
  }

  public string VisitGroupingExpr(Expr.Grouping expr)
  {
    return Parenthesize("group", expr.expression);
  }

  public string VisitIfStmt(Stmt.If stmt)
  {
    if (stmt.elseBranch == null)
    {
      return Parenthesize2("if", stmt.condition, stmt.thenBranch);
    }

    return Parenthesize2("if-else", stmt.condition, stmt.thenBranch, stmt.elseBranch);
  }

  public string VisitLiteralExpr(Expr.Literal expr)
  {
    if (expr.value == null) return "NULL";
    return expr.value.ToString();
  }

  public string VisitLogicalExpr(Expr.Logical expr)
  {
    return Parenthesize(expr.oper.lexeme, expr.left, expr.right);
  }

  public string VisitPrintStmt(Stmt.Print stmt)
  {
    return Parenthesize("print", stmt.expression);
  }

  public string VisitReturnStmt(Stmt.Return stmt)
  {
    if (stmt.value == null) return "(return)";
    return Parenthesize("return", stmt.value);
  }

  public string VisitSetExpr(Expr.Set expr)
  {
    return Parenthesize2("=", expr.obj, expr.name.lexeme, expr.value);
  }

  public string VisitUnaryExpr(Expr.Unary expr)
  {
    return Parenthesize(expr.oper.lexeme, expr.right);
  }

  public string VisitVariableExpr(Expr.Variable expr)
  {
    return expr.name.lexeme;
  }

  public string VisitVarStmt(Stmt.Var stmt)
  {
    if (stmt.initializer == null)
    {
      return Parenthesize2("var", stmt.name);
    }

    return Parenthesize2("var", stmt.name, "=", stmt.initializer);
  }

  private string Parenthesize(string name, params Expr[] exprs)
  {
    var builder = new StringBuilder();

    builder.Append("(").Append(name);

    foreach (var expr in exprs)
    {
      builder.Append(" ");
      builder.Append(expr.Accept(this));
    }

    builder.Append(")");
    return builder.ToString();
  }

  private string Parenthesize2(string name, params object[] parts)
  {
    var builder = new StringBuilder();

    builder.Append("(").Append(name);

    foreach (var part in parts)
    {
      builder.Append(" ");

      switch (part)
      {
        case Expr expr:
          builder.Append(expr.Accept(this));
          break;
        case Stmt stmt:
          builder.Append(stmt.Accept(this));
          break;
        case Token token:
          builder.Append(token.lexeme);
          break;
        case IEnumerable<Expr> expressions:
          if (expressions.Any())
          {
            builder.Append("[");
            foreach (var expr in expressions)
            {
              if (expr != expressions.First())
              {
                builder.Append(", ");
              }
              builder.Append(expr.Accept(this));
            }
            builder.Append("]");
          }
          break;
        default:
          builder.Append(part.ToString());
          break;
      }
    }

    builder.Append(")");
    return builder.ToString();
  }
}