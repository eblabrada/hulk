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

  public string VisitConditionalExpr(Expr.Conditional expr) {
    return $"if-else ({Print(expr.condition)}, {Print(expr.thenBranch)}, {Print(expr.elseBranch)})";
  }

  public string VisitBinaryExpr(Expr.Binary expr) {
    return $"{expr.oper.lexeme} ({Print(expr.left)}, {Print(expr.right)})"; 
  }

  public string VisitUnaryExpr(Expr.Unary expr) {
    return $"{expr.oper.lexeme} ({Print(expr.right)})";
  }

  public string VisitLiteralExpr(Expr.Literal expr) {
    return expr.value.ToString();
  }

  public string VisitAssignExpr(Expr.Assign expr) {
    return $"= ({Print(expr)}, {Print(expr.value)})";
  }

  public string VisitVariableExpr(Expr.Variable expr) {
    return expr.name.lexeme;
  }

  public string VisitLetInExpr(Expr.LetIn expr) {
    string result = "let-in {";
    foreach (var assignment in expr.assignments) {
      result += assignment;
    }
    result += "}";
    result += "(" + Print(expr.into) + ")";
    return result;
  }

  public string VisitFunctionExpr(Expr.Function expr) {
    return expr.name.lexeme;
  }

  public string VisitCallExpr(Expr.Call expr) {
    return expr.name.lexeme;
  }
}