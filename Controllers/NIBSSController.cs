using Microsoft.AspNetCore.Mvc;
using ebill.Contracts;
using ebill.Data.Models;


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
    // private readonly IConfiguration _config;

    // public AccountController(IConfiguration config)
    // {
    //     _config = config;
    // }

    public NIBSSController(ILogger<NIBSSController> logger)
    {
        _logger = logger;
    }

    [HttpPost("validation")]
    // [HttpPost(Name = "Validation")]
    public ActionResult<IEnumerable<ValidationResponse>> Validation(ValidationRequest model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("error: " + ModelState);
                return BadRequest(new { message = "bad model", data=ModelState,});

            }

            return Ok(new ValidationResponse());
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
