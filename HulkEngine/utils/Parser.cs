class Parser
{

  private class ParserError : Exception { }

  public readonly List<Token> tokens;
  private int current = 0;

  public Parser(List<Token> tokens)
  {
    this.tokens = tokens;
  }

  public Expr parse()
  {
    return expr();
  }

  private Expr factor()
  {
    // factor: (PLUS | MINUS) factor | NUMBER | LPAREN expr RPAREN
    Token token = peek();

    switch (token.type)
    {
      case TokenType.PLUS:
        eat(TokenType.PLUS, "Expected +");
        return new Expr.Unary(token, factor());
      case TokenType.MINUS:
        eat(TokenType.MINUS, "Expected -");
        return new Expr.Unary(token, factor());
      case TokenType.NUMBER:
        eat(TokenType.NUMBER, "Expected number");
        return new Expr.Number(token);
      case TokenType.LEFT_PARENTESIS:
        eat(TokenType.LEFT_PARENTESIS, "Expected parentesis");
        Expr result = expr();
        eat(TokenType.RIGHT_PARENTESIS, "Expected parentesis");
        return result;
    }

    if (token.type == TokenType.NUMBER)
    {
      eat(TokenType.NUMBER, "Expected number");
      return new Expr.Number(token);
    }
    else if (token.type == TokenType.LEFT_PARENTESIS)
    {
      eat(TokenType.LEFT_PARENTESIS, "Expected parentesis");
      Expr result = expr();
      eat(TokenType.RIGHT_PARENTESIS, "Expected parentesis");
      return result;
    }
    throw new Exception();
  }

  private Expr term2()
  {
    // term2: factor((POWER) factor)*
    Expr result = factor();
    while (match(TokenType.POWER))
    {
      Token oper = previous();
      Expr right = factor();
      result = new Expr.Binary(result, oper, right);
    }

    return result;
  }

  private Expr term()
  {
    // term: term2((MUL | DIV) term2)*
    Expr result = term2();
    while (match(TokenType.MUL, TokenType.DIV))
    {
      Token oper = previous();
      Expr right = term2();
      result = new Expr.Binary(result, oper, right);
    }

    return result;
  }

  private Expr expr()
  {
    // expr: term((PLUS | MINUS) term)*
    // term: factor((MUL | DIV) factor)*
    // factor: NUMBER
    Expr result = term();
    while (match(TokenType.PLUS, TokenType.MINUS))
    {
      Token oper = previous();
      Expr right = term();
      result = new Expr.Binary(result, oper, right);
    }

    return result;
  }

  private bool match(params TokenType[] types)
  {
    foreach (TokenType type in types)
    {
      if (check(type))
      {
        advance();
        return true;
      }
    }
    return false;
  }

  private Token eat(TokenType type, string message)
  {
    if (check(type)) return advance();
    throw showError(peek(), message);
  }

  private bool check(TokenType type)
  {
    if (isAtEnd()) return false;
    return peek().type == type;
  }

  private Token advance()
  {
    if (!isAtEnd()) current++;
    return previous();
  }

  private bool isAtEnd()
  {
    return peek().type == TokenType.EOF;
  }

  private Token peek()
  {
    return tokens[current];
  }

  private Token previous()
  {
    return tokens[current - 1];
  }

  private ParserError showError(Token token, string message)
  {
    Error.showError(token, message);
    return new ParserError();
  }
}