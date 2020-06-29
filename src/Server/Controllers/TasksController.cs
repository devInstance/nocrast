using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NoCrast.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ILogger<TasksController> logger;

        public TasksController(ILogger<TasksController> logger)
        {
            this.logger = logger;
        }
    }
}
