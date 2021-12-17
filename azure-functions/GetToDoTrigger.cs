
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using azure_functions.Infrastructure;
using azure_functions.Domain;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            ClaimsPrincipal identities = req.HttpContext.User;

            var userName = identities.Identity?.Name ?? req.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].ToString();
            log.LogInformation($"{req.Headers["X-MS-CLIENT-PRINCIPAL-NAME"]}");
            log.LogInformation($"{JsonSerializer.Serialize(identities.Identity)}");
            if (string.IsNullOrEmpty(userName))
            {
                log.LogInformation("Empty username");
                return new StatusCodeResult(403);
            }
            log.LogInformation($"Get all notes. User: {userName}");
            var noteList = _dbContext.Set<Note>().Where(x => x.CreatedBy == userName).ToList();
            log.LogInformation($"Get notes: {noteList.Count}");
            return new OkObjectResult(noteList);
        }
    }
}
