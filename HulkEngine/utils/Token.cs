public class Token
{
  public TokenType type { get; private set; }
  public string lexeme { get; private set; }
  public Object literal { get; private set; }
  public int line { get; private set; }

  public Token(TokenType type, string lexeme, Object literal, int line)
  {
    this.type = type;
    this.lexeme = lexeme;
    this.literal = literal;
    this.line = line;
  }

  public override string toString()
  {
    return type + " " + lexeme + " " + literal;
  }
}