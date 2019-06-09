using System;
using System.Collections.Generic;
using System.Text;

namespace CLogger
{
    public interface ILogger
    {
        void LogInformation(LogLevels eventLevel, string information, Exception ex = null, params object[] values);
    }
}
