using static TokenType;

public class Scanner
{
  private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
  {
    {"and", AND},
    {"else", ELSE},
    {"false", FALSE},
    {"function", FUNCTION},
    {"if", IF},
    {"or", OR},
    {"print", PRINT},
    {"return", RETURN},
    {"true", TRUE},
    {"let", LET},
    {"in", IN},
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
    tokens.Add(new Token(EOF, "", null, line));
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
      case '.':
        AddToken(DOT);
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
      case '!':
        AddToken(IsNext('=') ? NOT_EQUAL : NOT);
        break;
      case '=':
        AddToken(IsNext('=') ? EQUAL_EQUAL : IsNext('>') ? IMPLIES : EQUAL);
        break;
      case '<':
        AddToken(IsNext('=') ? LESS_EQUAL : LESS);
        break;
      case '>':
        AddToken(IsNext('=') ? GREATER_EQUAL : GREATER);
        break;
      case '/':
        // maybe be comment?
        AddToken(DIV);
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
          logger.Error(this.line, "Unexpected character.");
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
    { // float ScanNumber
      Advance();
      while (IsDigit(Peek())) Advance();
    }

    AddToken(NUMBER, double.Parse(source.Substring(start, current - start)));
  }

  private void ScanString()
  {
    while (Peek() != '"' && !IsAtEnd())
    {
      if (Peek() == '\n') line++;
      Advance();
    }

    // unterminated string
    if (IsAtEnd())
    {
      logger.Error(this.line, "Unterminated string.");
      return;
    }

    // closing "
    Advance();

    // trim the surrounding quotes
    string value = source.Substring(start + 1, (current - 1) - (start + 1));
    AddToken(STRING, value);
  }

  private bool IsNext(char expected)
  {
    if (IsAtEnd()) return false;
    if (source[current] == expected)
    {
      current++;
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
    tokens.Add(new Token(type, text, literal, line));
  }
}
