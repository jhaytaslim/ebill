using Microsoft.AspNetCore.Mvc;
using ebill.Contracts;

namespace ebill.Controllers;

[ApiController]
[Route("[controller]")]
public class NIBSSController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<NIBSSController> _logger;

    public NIBSSController(ILogger<NIBSSController> logger)
    {
        _logger = logger;
    }

    [HttpPost("validation")]
    // [HttpPost(Name = "Validation")]
    public IEnumerable<ValidationResponse> Validation(ValidationRequest model)
    {
        try
        {
            if (!ModelState.IsValid)
            {

            }


        }
        catch (Exception ex)
        {

        }

        return new List<ValidationResponse>();
    }

    [HttpPost("notification")]
    // [HttpPost(Name = "Notification")]
    public IEnumerable<NotificationResponse> Notification(NotificationRequest model)
    {
        try
        {
            if (!ModelState.IsValid)
            {

            }


        }
        catch (Exception ex)
        {

        }
        return new List<NotificationResponse>();
    }

}
