using System.Linq.Expressions;
using System.Reflection.Emit;
using static TokenType;

public class Parser
{

  private class ParserError : Exception { }

  private int current = 0;
  public readonly List<Token> tokens;

  private readonly ILogger logger;

  public Parser(ILogger logger, List<Token> tokens)
  {
    this.logger = logger;
    this.tokens = tokens;
  }

  public List<Stmt> Parse()
  {
    var statements = new List<Stmt>();
    while (!IsAtEnd())
    {
      statements.Add(Declaration());
    }
    return statements;
  }

  private Stmt Declaration()
  {
    try
    {
      if (Match(FUNCTION)) return Function("function");
      if (Match(LET)) return VarDeclaration();

      return Statement();
    }
    catch (ParserError)
    {
      Sync();
      return null;
    }
  }

  private Stmt.Function Function(string kind)
  {
    var name = Eat(IDENTIFIER, $"Expected {kind} name.");
    Eat(LEFT_PARENTESIS, $"Expected '(' after {kind} name.");

    var parameters = new List<Token>();
    if (!Check(RIGHT_PARENTESIS))
    {
      do
      {
        if (parameters.Count >= 10)
        {
          Error(Peek(), "Cannot have more than 10 parameters.");
        }

        parameters.Add(Eat(IDENTIFIER, "Expected parameter name."));
      } while (Match(COMMA));
    }

    Eat(RIGHT_PARENTESIS, "Expected ')' after parameters.");
    Eat(IMPLIES, $"Expected '=>' symbol before body of {kind}.");

    var body = InlineFunctionBody();

    return new Stmt.Function(name, parameters, body);
  }

  private List<Stmt> InlineFunctionBody()
  {
    var statements = new List<Stmt>();
    while (!Check(SEMICOLON) && !IsAtEnd())
    {
      statements.Add(Declaration());
    }

    Eat(SEMICOLON, "Expected ';' after inline function body.");
    return statements;
  }

  private Stmt VarDeclaration()
  {
    var name = Eat(IDENTIFIER, "Expected variable name.");

    Expr initializer = null;
    if (Match(EQUAL))
    {
      initializer = Expression();
    }

    Eat(IN, "Expected 'in' after variable declaration.");
    return new Stmt.Var(name, initializer);
  }

  private Stmt Statement()
  {
    if (Match(IF)) return IfStatement();
    if (Match(PRINT)) return PrintStatement();
    if (Match(RETURN)) return ReturnStatement();
    return ExpressionStatement();
  }

  private Stmt IfStatement()
  {
    Eat(LEFT_PARENTESIS, "Expected '(' after 'if'.");
    var condition = Expression();
    Eat(RIGHT_PARENTESIS, "Expected ')' after if condition.");

    var thenBranch = Statement();
    var elseBranch = default(Stmt);

    if (Match(ELSE))
    {
      elseBranch = Statement();
    }

    return new Stmt.If(condition, thenBranch, elseBranch);
  }

  private Stmt PrintStatement()
  {
    var value = Expression();
    if (Check(SEMICOLON))
    {
      Eat(SEMICOLON, "");
      if (!IsAtEnd())
      {
        Error(Peek(), "Expected EOF after ';'.");
      }
    }
    return new Stmt.Print(value);
  }

  private Stmt ReturnStatement()
  {
    var keyword = Previous();
    Expr value = null;
    if (!Check(SEMICOLON))
    {
      value = Expression();
    }

    Eat(SEMICOLON, "Expected ';' after return value.");
    return new Stmt.Return(keyword, value);
  }

  private Stmt ExpressionStatement()
  {
    var expr = Expression();
    // Eat(SEMICOLON, "Expected ';' after expression.");
    return new Stmt.Expression(expr);
  }

  private Expr Expression()
  {
    return Assignment();
  }

  private Expr Assignment()
  {
    var expr = Or();

    if (Match(EQUAL))
    {
      var equals = Previous();
      var value = Assignment();

      if (expr is Expr.Variable variableExpr)
      {
        var name = variableExpr.name;
        return new Expr.Assign(name, value);
      }
      else if (expr is Expr.Get getExpr)
      {
        return new Expr.Set(getExpr.obj, getExpr.name, value);
      }

      Error(equals, "Invalid assignment target.");
    }

    return expr;
  }

  private Expr Or()
  {
    var expr = And();

    while (Match(OR))
    {
      var op = Previous();
      var right = And();
      expr = new Expr.Logical(expr, op, right);
    }

    return expr;
  }

  private Expr And()
  {
    var expr = Equality();

    while (Match(AND))
    {
      var op = Previous();
      var right = Equality();
      expr = new Expr.Logical(expr, op, right);
    }

    return expr;
  }

  private Expr Equality()
  {
    return AssociativeBinOp(Comparison, NOT_EQUAL, EQUAL_EQUAL);
  }

  private Expr Comparison()
  {
    return AssociativeBinOp(Addition, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL);
  }

  private Expr Addition()
  {
    return AssociativeBinOp(Multiplication, MINUS, PLUS, CONCAT);
  }

  private Expr Multiplication()
  {
    return AssociativeBinOp(Power, DIV, MUL);
  }

  private Expr Power()
  {
    return AssociativeBinOp(Unary, POWER);
  }

  private Expr AssociativeBinOp(Func<Expr> higherPrecedence, params TokenType[] tokenTypes)
  {
    var expr = higherPrecedence();

    while (Match(tokenTypes))
    {
      var op = Previous();
      var right = higherPrecedence();
      expr = new Expr.Binary(expr, op, right);
    }

    return expr;
  }

  private Expr Unary()
  {
    if (Match(NOT, MINUS))
    {
      var op = Previous();
      var right = Unary();
      return new Expr.Unary(op, right);
    }

    return Call();
  }

  private Expr Call()
  {
    var expr = Primary();

    while (true)
    {
      if (Match(LEFT_PARENTESIS))
      {
        expr = FinishCall(expr);
      }
      else
      {
        break;
      }
    }

    return expr;
  }

  private Expr FinishCall(Expr calle)
  {
    var arguments = new List<Expr>();
    if (!Check(RIGHT_PARENTESIS))
    {
      do
      {
        if (arguments.Count >= 10)
        {
          Error(Peek(), "Cannot have more than 10 arguments.");
        }
        arguments.Add(Expression());
      } while (Match(COMMA));
    }

    var paren = Eat(RIGHT_PARENTESIS, "Expected ')' after arguments.");

    return new Expr.Call(calle, paren, arguments);
  }

  private Expr Primary()
  {
    if (Match(FALSE)) return new Expr.Literal(false);
    if (Match(TRUE)) return new Expr.Literal(true);

    if (Match(NUMBER, STRING))
    {
      return new Expr.Literal(Previous().literal);
    }

    if (Match(IDENTIFIER))
    {
      return new Expr.Variable(Previous());
    }

    if (Match(LEFT_PARENTESIS))
    {
      var expr = Expression();
      Eat(RIGHT_PARENTESIS, "Expected ')' after expression.");
      return new Expr.Grouping(expr);
    }

    throw Error(Peek(), "Expected expression.");
  }

  private bool Match(params TokenType[] types)
  {
    foreach (TokenType type in types)
    {
      if (Check(type))
      {
        Advance();
        return true;
      }
    }
    return false;
  }

  private Token Eat(TokenType type, string message)
  {
    if (Check(type)) return Advance();
    throw Error(Peek(), message);
  }

  private bool Check(TokenType type)
  {
    if (IsAtEnd()) return false;
    return Peek().type == type;
  }

  private Token Advance()
  {
    if (!IsAtEnd()) current++;
    return Previous();
  }

  private bool IsAtEnd()
  {
    return Peek().type == TokenType.EOF;
  }

  private Token Peek()
  {
    return tokens[current];
  }

  private Token Previous()
  {
    return tokens[current - 1];
  }

  private ParserError Error(Token token, string message)
  {
    logger.Error(token, message);
    return new ParserError();
  }

  private void Sync()
  {
    Advance();

    while (!IsAtEnd())
    {
      if (Previous().type == SEMICOLON) return;

      switch (Peek().type)
      {
        case FUNCTION:
        case LET:
        case IF:
        case PRINT:
        case RETURN:
          return;
      }
    }

    Advance();
  }
}