using System.Reflection.Metadata.Ecma335;

public class Interpreter
{

  private class InterpreterError : Exception { }

  public readonly List<Token> tokens;
  private int current = 0;

  public Interpreter(List<Token> tokens)
  {
    this.tokens = tokens;
  }

  public double eval()
  {
    return expr();
  }
  
  private double factor() {
    // factor: NUMBER
    Token token = peek();
    eat(TokenType.NUMBER, "Expected number");
    return double.Parse(token.lexeme);
  }

  private double term() {
    // term: factor((MUL | DIV) factor)*

    double result = factor();
    while (match(TokenType.MUL, TokenType.DIV)) {
      Token op = previous();
      double right = factor();
      
      if (op.type == TokenType.MUL) {
        result = result * right;
      } else {
        result = result / right;
      }
    }

    return result;
  }

  private double expr() {
    // expr: term((PLUS | MINUS) term)*
    // term: factor((MUL | DIV) factor)*
    // factor: NUMBER
    
    double result = term();
    while (match(TokenType.PLUS, TokenType.MINUS)) {
      Token op = previous();
      double right = term();
      if (op.type == TokenType.PLUS) {
        result = result + right;
      } else {
        result = result - right;
      }
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

  private InterpreterError showError(Token token, string message)
  {
    Error.showError(token, message);
    return new InterpreterError();
  }

}