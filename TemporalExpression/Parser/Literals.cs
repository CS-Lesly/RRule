namespace TemporalExpression.Parser;

public static class Literals
{
    public static LiteralComponent COLON     => new(":");
    public static LiteralComponent SEMICOLON => new(";");
    public static LiteralComponent COMMA     => new(",");
    public static LiteralComponent EQUALS    => new("=");
    public static LiteralComponent NEWLINE   => new("\r\n");
    public static LiteralComponent PLUS      => new("+");
    public static LiteralComponent MINUS     => new("-");
    public static LiteralComponent SLASH     => new("/");
}