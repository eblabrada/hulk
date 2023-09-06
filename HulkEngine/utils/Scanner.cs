public class Scanner
{
  private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>();
  private string src;
  private List<Token> tokens = new List<Token>();
  private int start = 0;
  private int current = 0;
  private int line = 1;

  static void addKeywords()
  {
    keywords.Add("and", TokenType.AND);
    keywords.Add("else", TokenType.ELSE);
    keywords.Add("false", TokenType.FALSE);
    keywords.Add("for", TokenType.FOR);
    keywords.Add("function", TokenType.FUNCTION);
    keywords.Add("if", TokenType.IF);
    keywords.Add("or", TokenType.OR);
    keywords.Add("print", TokenType.PRINT);
    keywords.Add("return", TokenType.RETURN);
    keywords.Add("true", TokenType.TRUE);
    keywords.Add("let", TokenType.LET);
    keywords.Add("in", TokenType.IN);
    keywords.Add("while", TokenType.WHILE);
  }

  public Scanner(string src)
  {
    this.src = src;
  }

  public List<Token> scanTokens()
  {
    while (!isAtEnd())
    {
      start = current;
      scanToken();
    }
    tokens.Add(new Token(TokenType.EOF, "", null, line));
    return tokens;
  }

  private void scanToken()
  {
    char c = advance();
    switch (c)
    {
      case '(':
        addToken(TokenType.LEFT_PARENTESIS);
        break;
      case ')':
        addToken(TokenType.RIGHT_PARENTESIS);
        break;
      case ',':
        addToken(TokenType.COMMA);
        break;
      case '.':
        addToken(TokenType.DOT);
        break;
      case '-':
        addToken(TokenType.MINUS);
        break;
      case '+':
        addToken(TokenType.PLUS);
        break;
      case ';':
        addToken(TokenType.SEMICOLON);
        break;
      case '*':
        addToken(TokenType.MUL);
        break;
      case '@':
        addToken(TokenType.CONCAT);
        break;
      case '!':
        addToken(isNext('=') ? TokenType.NOT_EQUAL : TokenType.NOT);
        break;
      case '=':
        addToken(isNext('=') ? TokenType.EQUAL_EQUAL : isNext('>') ? TokenType.RETURN : TokenType.EQUAL);
        break;
      case '<':
        addToken(isNext('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
        break;
      case '>':
        addToken(isNext('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
        break;
      case '/':
        // maybe be comment?
        addToken(TokenType.DIV);
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
        str();
        break;
      default:
        if (IsDigit(c))
        {
          number();
        }
        else if (isAlpha(c))
        {
          identifier();
        }
        else
        {
          // unexpected character
          Error.showError(this.line, "Lexical Error");
        }
        break;
    }
  }

  private void identifier()
  {
    while (isAlphaNum(curValue())) advance();

    string text = src.Substring(start, current - start);
    TokenType type;
    if (keywords.ContainsKey(text))
    {
      type = keywords[text];
    }
    else
    {
      type = TokenType.IDENTIFIER;
    }
    addToken(type);
  }

  private void number()
  {
    while (IsDigit(curValue())) advance();

    if (curValue() == '.' && IsDigit(nextValue()))
    { // float number
      advance();
      while (IsDigit(curValue())) advance();
    }

    addToken(TokenType.NUMBER, double.Parse(src.Substring(start, current - start)));
  }

  private void str()
  {
    while (curValue() != '"' && !isAtEnd())
    {
      if (curValue() == '\n') line++;
      advance();
    }

    if (isAtEnd())
    {
      // unterminated string
      Error.showError(this.line, "Syntax Error");
      return;
    }

    advance();

    string value = src.Substring(start + 1, (current - 1) - (start + 1));
    addToken(TokenType.STRING, value);
  }

  private bool isNext(char expected)
  {
    if (isAtEnd()) return false;
    if (src[current] == expected)
    {
      current++;
      return true;
    }
    return false;
  }

  private char curValue()
  {
    if (isAtEnd()) return '\0';
    return src[current];
  }

  private char nextValue()
  {
    if (current + 1 >= src.Count()) return '\0';
    return src[current + 1];
  }

  private bool isAlpha(char c)
  {
    return ('a' <= c && c <= 'z') ||
           ('A' <= c && c <= 'Z') ||
           (c == '_');
  }

  private bool IsDigit(char c)
  {
    return '0' <= c && c <= '9';
  }

  private bool isAlphaNum(char c)
  {
    return isAlpha(c) || IsDigit(c);
  }

  private bool isAtEnd()
  {
    return current >= src.Count();
  }

  private char advance()
  {
    return src[current++];
  }

  private void addToken(TokenType type)
  {
    addToken(type, null);
  }

  private void addToken(TokenType type, object literal)
  {
    string text = src.Substring(start, current - start);
    tokens.Add(new Token(type, text, literal, line));
  }
}
