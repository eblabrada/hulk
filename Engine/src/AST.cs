using System.Diagnostics.Contracts;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Text;

public class AST : Expr.IVisitor<string>
{
  public string Print(Expr expr)
  {
    return expr.Accept(this);
  }

  public string VisitConditionalExpr(Expr.Conditional expr)
  {
    return $"if-else ({Print(expr.condition)}, {Print(expr.thenBranch)}, {Print(expr.elseBranch)})";
  }

  public string VisitBinaryExpr(Expr.Binary expr)
  {
    return $"{expr.oper.lexeme} ({Print(expr.left)}, {Print(expr.right)})";
  }

  public string VisitUnaryExpr(Expr.Unary expr)
  {
    return $"{expr.oper.lexeme} ({Print(expr.right)})";
  }

  public string VisitLiteralExpr(Expr.Literal expr)
  {
    if (expr.value == null) return "nil";
    return expr.value.ToString();
  }

  public string VisitAssignExpr(Expr.Assign expr)
  {
    return $"= ({Print(expr)}, {Print(expr.value)})";
  }

  public string VisitVariableExpr(Expr.Variable expr)
  {
    return expr.name.lexeme;
  }

  public string VisitLetInExpr(Expr.LetIn expr)
  {
    string result = "let-in {"; bool comma = false;
    foreach (var assignment in expr.assignments)
    {
      if (comma) result += ", ";
      result += $"= ({assignment.name.lexeme} {Print(assignment.value)})";
      comma = true;
    }
    result += "}";
    result += "(" + Print(expr.into) + ")";
    return result;
  }

  public string VisitFunctionExpr(Expr.Function expr)
  {
    return expr.name.lexeme;
  }

  public string VisitCallExpr(Expr.Call expr)
  {
    return Parenthesize("call", expr.name, expr.parameters);
  }

  private string Parenthesize(string name, params object[] parts)
  {
    var result = new StringBuilder();
    result.Append("(").Append(name);

    foreach (var part in parts)
    {
      result.Append(" ");

      switch (part)
      {
        case Expr expr:
          result.Append(expr.Accept(this));
          break;
        case Token token:
          result.Append(token.lexeme);
          break;
        case IEnumerable<Expr> expressions:
          if (expressions.Any())
          {
            result.Append("[");
            foreach (var expr in expressions)
            {
              if (expr != expressions.First())
              {
                result.Append(", ");
              }
              result.Append(expr.Accept(this));
            }
            result.Append("]");
          }
          break;
        default:
          result.Append(part.ToString());
          break;
      }
    }

    result.Append(")");
    return result.ToString();
  }
}