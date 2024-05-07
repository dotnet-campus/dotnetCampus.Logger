namespace dotnetCampus.Logging.Writers.ConsoleLoggerHelpers;

/// <summary>
/// 包含控制台输出颜色的字符串常量。
/// </summary>
internal static class ConsoleColors
{
    public const string Reset = "\u001b[0m";

    public static class Foreground
    {
        #region 4-bit colors

        public const string Black = "\u001b[30m";
        public const string Red = "\u001b[31m";
        public const string Green = "\u001b[32m";
        public const string Yellow = "\u001b[33m";
        public const string Blue = "\u001b[34m";
        public const string Magenta = "\u001b[35m";
        public const string Cyan = "\u001b[36m";
        public const string White = "\u001b[37m";
        public const string BrightBlack = "\u001b[90m";
        public const string BrightRed = "\u001b[91m";
        public const string BrightGreen = "\u001b[92m";
        public const string BrightYellow = "\u001b[93m";
        public const string BrightBlue = "\u001b[94m";
        public const string BrightMagenta = "\u001b[95m";
        public const string BrightCyan = "\u001b[96m";
        public const string BrightWhite = "\u001b[97m";

        #endregion
    }

    public static class Background
    {
        #region 4-bit colors

        public const string Black = "\u001b[40m";
        public const string Red = "\u001b[41m";
        public const string Green = "\u001b[42m";
        public const string Yellow = "\u001b[43m";
        public const string Blue = "\u001b[44m";
        public const string Magenta = "\u001b[45m";
        public const string Cyan = "\u001b[46m";
        public const string White = "\u001b[47m";
        public const string BrightBlack = "\u001b[100m";
        public const string BrightRed = "\u001b[101m";
        public const string BrightGreen = "\u001b[102m";
        public const string BrightYellow = "\u001b[103m";
        public const string BrightBlue = "\u001b[104m";
        public const string BrightMagenta = "\u001b[105m";
        public const string BrightCyan = "\u001b[106m";
        public const string BrightWhite = "\u001b[107m";

        #endregion
    }

    public static class Decoration
    {
        public const string Bold = "\u001b[1m";
        public const string Dim = "\u001b[2m";
        public const string Italic = "\u001b[3m";
        public const string Underline = "\u001b[4m";
        public const string Blink = "\u001b[5m";
        public const string Reverse = "\u001b[7m";
        public const string Hidden = "\u001b[8m";
        public const string Strikethrough = "\u001b[9m";
    }
}
