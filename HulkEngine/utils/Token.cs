public class Token
{
  public TokenType type { get; private set; }
  public string lexeme { get; private set; }
  public object literal { get; private set; }
  public int line { get; private set; }
  public int column { get; private set; }

  public Token(TokenType type, string lexeme, object literal, int line, int column)
  {
    this.type = type;
    this.lexeme = lexeme;
    this.literal = literal;
    this.line = line;
    this.column = column;
  }

  public override string ToString()
  {
    return $"Token({type}, {lexeme}, {literal})";
  }
}