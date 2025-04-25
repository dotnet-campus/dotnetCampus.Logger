namespace dotnetCampus.Logging.Writers.ConsoleLoggerHelpers;

/// <summary>
/// 包含控制台输出颜色的字符串常量。
/// </summary>
internal static class ConsoleColors
{
    public const string Reset = "\e[0m";

    public static class Foreground
    {
        #region 4-bit colors

        public const string Black = "\e[30m";
        public const string Red = "\e[31m";
        public const string Green = "\e[32m";
        public const string Yellow = "\e[33m";
        public const string Blue = "\e[34m";
        public const string Magenta = "\e[35m";
        public const string Cyan = "\e[36m";
        public const string White = "\e[37m";
        public const string BrightBlack = "\e[90m";
        public const string BrightRed = "\e[91m";
        public const string BrightGreen = "\e[92m";
        public const string BrightYellow = "\e[93m";
        public const string BrightBlue = "\e[94m";
        public const string BrightMagenta = "\e[95m";
        public const string BrightCyan = "\e[96m";
        public const string BrightWhite = "\e[97m";

        #endregion
    }

    public static class Background
    {
        #region 4-bit colors

        public const string Black = "\e[40m";
        public const string Red = "\e[41m";
        public const string Green = "\e[42m";
        public const string Yellow = "\e[43m";
        public const string Blue = "\e[44m";
        public const string Magenta = "\e[45m";
        public const string Cyan = "\e[46m";
        public const string White = "\e[47m";
        public const string BrightBlack = "\e[100m";
        public const string BrightRed = "\e[101m";
        public const string BrightGreen = "\e[102m";
        public const string BrightYellow = "\e[103m";
        public const string BrightBlue = "\e[104m";
        public const string BrightMagenta = "\e[105m";
        public const string BrightCyan = "\e[106m";
        public const string BrightWhite = "\e[107m";

        #endregion
    }

    public static class Decoration
    {
        public const string Bold = "\e[1m";
        public const string Dim = "\e[2m";
        public const string Italic = "\e[3m";
        public const string Underline = "\e[4m";
        public const string Blink = "\e[5m";
        public const string Reverse = "\e[7m";
        public const string Hidden = "\e[8m";
        public const string Strikethrough = "\e[9m";
    }
}
