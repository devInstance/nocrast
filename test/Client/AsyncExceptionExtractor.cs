using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoCrast.ClientTests
{
    public class AsyncExceptionExtractor
    {
        public static T Run<T>(Task<T> task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                {
                    throw ex;
                });
            }
            return task.Result;
        }
    }
}
