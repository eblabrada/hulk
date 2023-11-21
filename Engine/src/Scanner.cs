using static TokenType;

public class Scanner
{
  private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
  {
    {"E", EULER},
    {"else", ELSE},
    {"false", FALSE},
    {"function", FUNCTION},
    {"if", IF},
    {"in", IN},
    {"let", LET},
    {"PI", PI},
    {"true", TRUE},
  };

  public string source { get; }
  private readonly List<Token> tokens = new List<Token>();
  private int start = 0;
  private int current = 0;
  private int line = 1;

  private readonly ILogger logger;

  public Scanner(ILogger logger, string source)
  {
    this.logger = logger;
    this.source = source;
  }

  public List<Token> ScanTokens()
  {
    while (!IsAtEnd())
    {
      start = current;
      ScanToken();
    }
    tokens.Add(new Token(EOF, "", null, line, current));
    return tokens;
  }

  private void ScanToken()
  {
    char c = Advance();
    switch (c)
    {
      case '(':
        AddToken(LEFT_PARENTESIS);
        break;
      case ')':
        AddToken(RIGHT_PARENTESIS);
        break;
      case ',':
        AddToken(COMMA);
        break;
      case '-':
        AddToken(MINUS);
        break;
      case '+':
        AddToken(PLUS);
        break;
      case ';':
        AddToken(SEMICOLON);
        break;
      case '*':
        AddToken(MUL);
        break;
      case '^':
        AddToken(POWER);
        break;
      case '@':
        AddToken(CONCAT);
        break;
      case '/':
        AddToken(DIV);
        break;
      case '%':
        AddToken(MOD);
        break;
      case '&':
        AddToken(AND);
        break;
      case '|':
        AddToken(OR);
        break;
      case '!':
        AddToken(Eat('=') ? NOT_EQUAL : NOT);
        break;
      case '=':
        AddToken(Eat('=') ? EQUAL_EQUAL : Eat('>') ? IMPLIES : EQUAL);
        break;
      case '<':
        AddToken(Eat('=') ? LESS_EQUAL : LESS);
        break;
      case '>':
        AddToken(Eat('=') ? GREATER_EQUAL : GREATER);
        break;
      case ' ':
      case '\r':
      case '\t':
        // ignore whitespaces
        break;
      case '\n':
        line++;
        break;
      case '"':
        ScanString();
        break;
      default:
        if (IsDigit(c))
        {
          ScanNumber();
        }
        else if (IsAlpha(c))
        {
          ScanIdentifier();
        }
        else
        {
          // unexpected character
          Token invalid = new Token(STRING, c.ToString(), null, line, current);
          logger.Error("LEXICAL", invalid, "Unexpected character.");
        }
        break;
    }
  }

  private void ScanIdentifier()
  {
    while (IsAlphaNum(Peek())) Advance();

    string text = source.Substring(start, current - start);
    TokenType type;
    if (keywords.ContainsKey(text))
    {
      type = keywords[text];
    }
    else
    {
      type = IDENTIFIER;
    }
    AddToken(type);
  }

  private void ScanNumber()
  {
    while (IsDigit(Peek())) Advance();

    if (Peek() == '.' && IsDigit(Next()))
    {
      Advance();
      while (IsDigit(Peek())) Advance();
    }

    if (IsAlpha(Peek()))
    {
      Advance();
      while (IsAlphaNum(Peek())) Advance();
      string text = source.Substring(start, current - start);
      Token invalid = new Token(IDENTIFIER, text, null, line, current);
      logger.Error("LEXICAL", invalid, "Is not a valid token");
      return;
    }

    AddToken(NUMBER, double.Parse(source.Substring(start, current - start)));
  }

  private void ScanString()
  {
    while (Peek() != '"' && !IsAtEnd())
    {
      if (Peek() == '\\' && Next() == '"') {
        Advance();
      }
      if (Peek() == '\n') line++;
      Advance();
    }
    
    // unterminated string
    if (IsAtEnd())
    {
      string text = source.Substring(start, current - start - 1);
      Token invalid = new Token(STRING, text, null, line, current);
      logger.Error("LEXICAL", invalid, "Unterminated string.");
      return;
    }

    // closing "
    Advance();

    // trim the surrounding quotes
    string value = source.Substring(start + 1, current - start - 2);

    value = value.Replace("\\t", "\t").Replace("\\n", "\n").Replace("\\\"", "\"");

    AddToken(STRING, value);
  }

  private bool Eat(char expected)
  {
    if (IsAtEnd()) return false;
    if (source[current] == expected)
    {
      Advance();
      return true;
    }
    return false;
  }

  private char Peek()
  {
    if (IsAtEnd()) return '\0';
    return source[current];
  }

  private char Next()
  {
    if (current + 1 >= source.Count()) return '\0';
    return source[current + 1];
  }

  private bool IsAlpha(char c)
  {
    return ('a' <= c && c <= 'z') ||
           ('A' <= c && c <= 'Z') ||
           (c == '_');
  }

  private bool IsDigit(char c)
  {
    return '0' <= c && c <= '9';
  }

  private bool IsAlphaNum(char c)
  {
    return IsAlpha(c) || IsDigit(c);
  }

  private bool IsAtEnd()
  {
    return current >= source.Count();
  }

  private char Advance()
  {
    return source[current++];
  }

  private void AddToken(TokenType type)
  {
    AddToken(type, null);
  }

  private void AddToken(TokenType type, object literal)
  {
    string text = source.Substring(start, current - start);
    tokens.Add(new Token(type, text, literal, line, current));
  }
}
