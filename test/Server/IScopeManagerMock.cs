using DevInstance.LogScope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCrast.ServerTests
{
    public class IScopeManagerMock : IScopeManager
    {
        public ILogProvider Provider => throw new NotImplementedException();

        public LogLevel Level => throw new NotImplementedException();

        public IScopeLog CreateLogger(string scope)
        {
            return new IScopeLogMock();
        }
    }

    public class IScopeLogMock : IScopeLog
    {
        public string Name { get { return ""; } }

        public void Dispose()
        {
        }

        public void Line(LogLevel level, string message)
        {
        }

        public void Line(string message)
        {
        }

        public IScopeLog Scope(LogLevel level, string scope)
        {
            return new IScopeLogMock();
        }

        public IScopeLog Scope(string scope)
        {
            return new IScopeLogMock();
        }
    }
}
