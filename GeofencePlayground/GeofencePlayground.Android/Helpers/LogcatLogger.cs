using System;
using Android.Util;
using Splat;

namespace GeofencePlayground.Droid.Helpers
{
    public class LogcatLogger : ILogger
    {
        public LogLevel Level { get; set; }

        private const string Tag = "LogcatLogger";

        public void Write(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Log.Debug(Tag, message);
                    break;

                case LogLevel.Info:
                    Log.Info(Tag, message);
                    break;

                case LogLevel.Warn:
                    Log.Warn(Tag, message);
                    break;

                case LogLevel.Error:
                    Log.Error(Tag, message);
                    break;

                case LogLevel.Fatal:
                    Log.Wtf(Tag, message);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}