using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using azure_functions.Infrastructure;
using System.Security.Claims;
using azure_functions.Domain;
using System.Linq;

namespace azure_functions
{
    public class DeleteToDoTrigger
    {
        private readonly NoteDbContext _dbContext;
        public DeleteToDoTrigger(NoteDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [FunctionName("DeleteToDoTrigger")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            ClaimsPrincipal identities = req.HttpContext.User;
            var userName = identities.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return new UnauthorizedResult();
            }

            string id = req.Query["id"];
            if (!int.TryParse(id, out int parsedId))
            {
                return new BadRequestResult();
            }

            var existingNote = _dbContext.Set<Note>().Where(x => x.Id == parsedId && x.CreatedBy == userName).FirstOrDefault();

            if (existingNote != null)
            {
                log.LogInformation($"Delete ${existingNote.Id} by ${userName}");
                _dbContext.Remove(existingNote);
                return new EmptyResult();
            }

            return new NotFoundResult();

        }
    }
}
