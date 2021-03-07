using DevInstance.LogScope;
using NoCrast.Server.Data;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Services
{
    public class BaseService
    {
        protected IScopeLog log;

        public ITimeProvider TimeProvider { get; }
        public IQueryRepository Repository { get; }

        public BaseService(IScopeManager logManager, ITimeProvider timeProvider, IQueryRepository query)
        {
            log = logManager.CreateLogger(this);

            TimeProvider = timeProvider;
            Repository = query;
        }
    }
}
