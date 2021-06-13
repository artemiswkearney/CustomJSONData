namespace CustomJSONData
{
    using System.Runtime.CompilerServices;

    using IPALogger = IPA.Logging.Logger;

    internal static class Logger
    {
        internal static IPALogger logger { get; set; }

        internal static void Log(object obj, IPALogger.Level level = IPALogger.Level.Debug, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            Log(obj.ToString(), level, member, line);
        }

        internal static void Log(string message, IPALogger.Level level = IPALogger.Level.Debug, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            logger.Log(level, $"{member}({line}): {message}");
        }
    }
}
