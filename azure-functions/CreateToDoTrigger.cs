using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Claims;
using azure_functions.Domain;
using azure_functions.Infrastructure;

namespace azure_functions
{
    public class CreateToDoTrigger
    {
        private readonly NoteDbContext _dbContext;
        public CreateToDoTrigger(NoteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [FunctionName("CreateToDoTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            ClaimsPrincipal identities = req.HttpContext.User;
            var userName = identities.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return new UnauthorizedResult();
            }
            string requestBody = string.Empty;
            using (StreamReader streamReader = new(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var title = data?.title;
            var message = data?.message;
            var newNote = new Note
            {
                Title = title,
                Message = message,
                CreatedBy = userName,
            };
            log.LogInformation($"Insert new note. User: ${userName}");
            await _dbContext.AddAsync(newNote);
            log.LogInformation($"Created Note: ${newNote.Id}");
            return new OkObjectResult(newNote);
        }
    }
}
