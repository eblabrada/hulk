public enum TokenType
{
  // Single-character Tokens
  LEFT_PARENTESIS, RIGHT_PARENTESIS, COMMA, SEMICOLON,

  // Operators
  MUL, CONCAT, MINUS, PLUS, POWER, DIV, MOD,

  // Comparison Tokens
  NOT, NOT_EQUAL, EQUAL, EQUAL_EQUAL, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,

  // Literals
  BOOLEAN, STRING, NUMBER, IDENTIFIER,

  // Keywords
  AND, ELSE, FALSE, FUNCTION, IF, LET, IN, OR, TRUE,

  // Constants
  EULER, PI,

  // Others
  IMPLIES, EOF
}