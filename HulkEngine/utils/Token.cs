public class Token
{
  public TokenType type { get; private set; }
  public string lexeme { get; private set; }
  public object literal { get; private set; }
  public int line { get; private set; }
  public int id { get; private set; }

  public Token(TokenType type, string lexeme, object literal, int line, int id)
  {
    this.type = type;
    this.lexeme = lexeme;
    this.literal = literal;
    this.line = line;
    this.id = id;
  }

  public override string ToString()
  {
    return $"Token({type}, {lexeme}, {literal})";
  }
}