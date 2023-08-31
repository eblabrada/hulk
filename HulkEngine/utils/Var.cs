using System.Linq.Expressions;

public class Var
{
  public static readonly Var ZERO = new Var(0);
  public VarType type { get; set; }

  public double number { get; set; }
  public bool boolean { get; set; }
  public string str { get; set; }

  public Var(Token tk) {
    if (tk.type == TokenType.NUMBER) {
      this.type = VarType.NUMBER;
      this.number = double.Parse(tk.lexeme);
    } else if (tk.type == TokenType.STRING) {
      this.type = VarType.STRING;
      this.str = tk.lexeme;
    }
  }

  public Var(double number)
  {
    this.type = VarType.NUMBER;
    this.number = number;
  }

  public Var(string str)
  {
    this.type = VarType.STRING;
    this.str = str;
  }

  public Var(bool boolean)
  {
    this.type = VarType.BOOLEAN;
    this.boolean = boolean;
  }

  public Var ParseVar(VarType type)
  {
    if (this.type != type)
    {
      switch (type)
      {
        case VarType.NUMBER:
          if (this.type == VarType.BOOLEAN)
          {
            if (this.boolean) this.number = 1;
            else this.number = 0;
          }
          else
          {
            this.number = double.Parse(this.str);
          }
          this.type = VarType.NUMBER;
          break;
        case VarType.STRING:
          if (this.type == VarType.BOOLEAN)
          {
            if (this.boolean) this.str = "True";
            else this.str = "False";
          }
          else
          {
            this.str = this.number.ToString();
          }
          this.type = VarType.STRING;
          break;
        case VarType.BOOLEAN:
          if (this.type == VarType.STRING)
          {
            Error.showError(ErrorType.SYNTAX);
            // Cannot implicitly convert type 'string' to 'bool'
          }
          else
          {
            this.boolean = Math.Abs(this.number) > 0.0;
            this.type = VarType.BOOLEAN;
          }
          break;
      }
    }
    return this;
  }

  public Var Unary(Token token)
  {
    if (this.type == VarType.STRING)
    {
      Error.showError(ErrorType.SYNTAX);
    }

    switch (token.type)
    {
      case TokenType.PLUS:
        return this;
      case TokenType.MINUS:
        return this.type == VarType.NUMBER ? new Var(-number) : this;
      case TokenType.NOT:
        return this.type == VarType.NUMBER ? new Var(number == 0 ? 1 : 0) : new Var(!boolean);
    }

    Error.showError(ErrorType.SYNTAX);
    return ZERO; // unuseful
  }

  public Var Binary(Var b, Token token)
  {
    Var a = this;
    if (a.type != b.type)
    {
      if (a.type > b.type)
      {
        b = b.ParseVar(a.type);
      }
      else
      {
        a = a.ParseVar(b.type);
      }
    }

    if (token.type == TokenType.CONCAT)
    {
      if (a.type != VarType.STRING || b.type != VarType.STRING)
      {
        Error.showError(ErrorType.SYNTAX);
        return ZERO;
      }
      return new Var(string.Concat(a.str, b.str));
    }

    if (token.type == TokenType.EQUAL_EQUAL)
    {
      if (a.type == VarType.BOOLEAN) return new Var(a.boolean == b.boolean);
      if (a.type == VarType.NUMBER) return new Var(a.number == b.number);
      return new Var(string.Compare(a.str, b.str) == 0);
    }

    if (token.type == TokenType.NOT_EQUAL)
    {
      if (a.type == VarType.BOOLEAN) return new Var(a.boolean != b.boolean);
      if (a.type == VarType.NUMBER) return new Var(a.number != b.number);
      return new Var(string.Compare(a.str, b.str) != 0);
    }

    if (token.type == TokenType.GREATER)
    {
      if (a.type == VarType.BOOLEAN) return new Var(a.boolean && !b.boolean);
      if (a.type == VarType.NUMBER) return new Var(a.number > b.number);
      return new Var(string.Compare(a.str, b.str) == 1);
    }

    if (token.type == TokenType.GREATER_EQUAL)
    {
      if (a.type == VarType.BOOLEAN) return new Var(a.boolean || a.boolean == b.boolean);
      if (a.type == VarType.NUMBER) return new Var(a.number >= b.number);
      return new Var(string.Compare(a.str, b.str) >= 0);
    }

    if (token.type == TokenType.LESS)
    {
      if (a.type == VarType.BOOLEAN) return new Var(!a.boolean && b.boolean);
      if (a.type == VarType.NUMBER) return new Var(a.number < b.number);
      return new Var(string.Compare(a.str, b.str) == -1);
    }

    if (token.type == TokenType.LESS_EQUAL)
    {
      if (a.type == VarType.BOOLEAN) return new Var(!a.boolean || a.boolean == b.boolean);
      if (a.type == VarType.NUMBER) return new Var(a.number <= b.number);
      return new Var(string.Compare(a.str, b.str) <= 0);
    }

    if (token.type == TokenType.AND)
    {
      Var na = a.ParseVar(VarType.BOOLEAN), nb = b.ParseVar(VarType.BOOLEAN);
      return new Var(na.boolean && nb.boolean);
    }

    if (token.type == TokenType.OR)
    {
      Var na = a.ParseVar(VarType.BOOLEAN), nb = b.ParseVar(VarType.BOOLEAN);
      return new Var(na.boolean || nb.boolean);
    }

    if (a.type != VarType.NUMBER)
    {
      Error.showError(ErrorType.LEXICAL);
      return ZERO;
    }

    switch (token.type)
    {
      case TokenType.PLUS:
        return new Var(a.number + b.number);
      case TokenType.MINUS:
        return new Var(a.number - b.number);
      case TokenType.ASTERISK:
        return new Var(a.number * b.number);
      case TokenType.SLASH:
        if (b.number == 0) Error.showError(ErrorType.SEMANTIC);
        return new Var(a.number / b.number);
      case TokenType.POWER:
        return new Var(Math.Pow(a.number, b.number));
    }

    return ZERO;
  }

  public override string ToString()
  {
    if (this.type == VarType.NUMBER)
      return this.number.ToString();

    if (this.type == VarType.BOOLEAN)
      return this.boolean ? "True" : "False";

    return this.str;
  }
}