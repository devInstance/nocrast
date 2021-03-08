using Microsoft.AspNetCore.Mvc;
using NoCrast.Server.Database;
using System;
using System.Threading.Tasks;

namespace NoCrast.Server.Controllers
{
    public delegate ActionResult<T> WebHandler<T>();
    public delegate Task<ActionResult<T>> WebHandlerAsync<T>();

    public class BaseController : ControllerBase
    {
        protected readonly ApplicationDbContext db;

        protected BaseController(ApplicationDbContext d)
        {
            db = d;
        }

        public ApplicationDbContext DB
        {
            get
            {
                return db;
            }
        }
        private ActionResult<T> HandleException<T>(Exception ex)
        {
            //TODO: Log error
            return Problem(detail: ex.StackTrace, title: ex.Message);
        }

        protected async Task<ActionResult<T>> HandleWebRequestAsync<T>(WebHandlerAsync<T> handler)
        {
            try
            {
                return await handler();
            }
            catch (Exception ex)
            {
                return HandleException<T>(ex);
            }
        }

        protected ActionResult<T> HandleWebRequest<T>(WebHandler<T> handler)
        {
            try
            {
                return handler();
            }
            catch (Exception ex)
            {
                return HandleException<T>(ex);
            }
        }
    }
}
