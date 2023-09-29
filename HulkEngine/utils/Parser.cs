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

  public Expr Parse()
  {
    try
    {
      Expr result = HulkExpr();

      if (Match(SEMICOLON))
      {
        if (!IsAtEnd())
        {
          logger.Error(Peek(), "Expected end of line after ';'.");
          return null;
        }
        return result;
      }

      logger.Error(Peek(), "Expected ';' at end of line but not found.");
      return null;
    }
    catch
    {
      logger.Error(Peek(), "Unreachable code.");
      return null;
    }
  }

  private Expr HulkExpr(int MaxParameters = 10)
  {
    if (Match(FUNCTION))
    {
      var name = Eat(IDENTIFIER, "Expected function name.");
      Eat(LEFT_PARENTESIS, "Expected '(' after function name.");

      var parameters = new List<Token>();
      if (!Check(RIGHT_PARENTESIS))
      {
        do
        {
          if (parameters.Count > MaxParameters)
          {
            logger.Error(Peek(), $"Functions can't have more than {MaxParameters} parameters.");
            return null;
          }

          parameters.Add(Eat(IDENTIFIER, "Expected parameter name."));
        } while (Match(COMMA));
      }

      Eat(RIGHT_PARENTESIS, "Expected ')' after parameters.");
      Eat(IMPLIES, "Expected '=>' symbol before body of function.");

      var body = Expression();

      return new Expr.Function(name, parameters, body);
    }

    return Expression();
  }

  private Expr VarDeclaration()
  {
    if (Match(LET))
    {
      var assignments = Variables();

      Eat(IN, "Expected 'in' at end of 'let-in' statement.");

      Expr into = Expression();
      return new Expr.LetIn(assignments, into);
    }

    return IfStatement();
  }

  private List<Expr.Assign> Variables()
  {
    var assignments = new List<Expr.Assign>();

    do
    {
      if (!Match(IDENTIFIER))
      {
        logger.Error(Peek(), "Variable name expected but not found.");
        return null;
      }

      Token name = Previous();
      Eat(EQUAL, "Expected '=' after variable name.");
      Expr value = Expression();
      assignments.Add(new Expr.Assign(name, value));
    } while (Match(COMMA));

    return assignments;
  }

  private Expr IfStatement()
  {
    if (Match(IF))
    {
      Eat(LEFT_PARENTESIS, "Expected '(' after 'if'.");
      var condition = Expression();
      Eat(RIGHT_PARENTESIS, "Expected ')' after if condition.");

      var thenBranch = Expression();
      var elseBranch = default(Expr);

      if (Match(ELSE))
      {
        elseBranch = Expression();
      }

      return new Expr.Conditional(condition, thenBranch, elseBranch);
    }

    return Or();
  }

  private Expr Expression()
  {
    return VarDeclaration();
  }

  private Expr Or()
  {
    var expr = And();

    while (Match(OR))
    {
      var op = Previous();
      var right = And();
      expr = new Expr.Binary(expr, op, right);
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
      expr = new Expr.Binary(expr, op, right);
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
    return AssociativeBinOp(Power, DIV, MOD, MUL);
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

    return Grouping();
  }

  private Expr Grouping()
  {
    if (Match(LEFT_PARENTESIS))
    {
      Expr expr = Expression();
      Eat(RIGHT_PARENTESIS, "Expected ')' after expression.");
      return expr;
    }
    return Call();
  }

  private Expr Call()
  {
    if (Check(IDENTIFIER) && CheckNext(LEFT_PARENTESIS))
    {
      Token name = Advance();
      Eat(LEFT_PARENTESIS, "Expected '(' after identifier.");
      List<Expr> parameters = Parameters();
      Eat(RIGHT_PARENTESIS, "Expected ')' after parameters.");
      return new Expr.Call(name, parameters);
    }
    return Literal();
  }

  private List<Expr> Parameters()
  {
    var parameters = new List<Expr>();

    if (!Check(RIGHT_PARENTESIS))
    {
      do
      {
        parameters.Add(Expression());
      } while (Match(COMMA));
    }

    return parameters;
  }

  private Expr Literal()
  {
    switch (Peek().type)
    {
      case STRING:
      case NUMBER:
        return new Expr.Literal(Advance().literal);
      case TRUE:
        Advance();
        return new Expr.Literal(true);
      case FALSE:
        Advance();
        return new Expr.Literal(false);
      case PI:
        Advance();
        return new Expr.Literal(Math.PI);
      case TokenType.EULER:
        Advance();
        return new Expr.Literal(Math.E);
      case TokenType.IDENTIFIER:
        return new Expr.Variable(Advance());
      default:
        logger.Error(Peek(), "Expected expression 'literal' but not found.");
        return null;
    }
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
    logger.Error(Peek(), message);
    return null;
  }

  private bool Check(TokenType type)
  {
    if (IsAtEnd()) return false;
    return Peek().type == type;
  }

  private bool CheckNext(TokenType type)
  {
    if (IsAtEnd()) return false;
    return PeekNext().type == type;
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

  private Token PeekNext()
  {
    return tokens[current + 1];
  }

  private Token Previous()
  {
    return tokens[current - 1];
  }
}