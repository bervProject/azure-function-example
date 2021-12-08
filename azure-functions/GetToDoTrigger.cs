
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using azure_functions.Infrastructure;
using azure_functions.Domain;
using System.Linq;
using System.Security.Claims;

namespace azure_functions
{
    public class GetToDoTrigger
    {
        private readonly NoteDbContext _dbContext;
        public GetToDoTrigger(NoteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [FunctionName("GetToDoTrigger")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            ClaimsPrincipal identities = req.HttpContext.User;
            var userName = identities.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return new UnauthorizedResult();
            }
            log.LogInformation($"Get all notes. User: ${userName}");
            var noteList = _dbContext.Set<Note>().Where(x => x.CreatedBy == userName).ToList();
            log.LogInformation($"Get notes: ${noteList.Count}");
            return new OkObjectResult(noteList);
        }
    }
}
