using Microsoft.AspNetCore.Mvc;
using ebill.Contracts;
using ebill.Data.Models;
using Microsoft.AspNetCore.Authorization;
using ebill.Utils;
using Newtonsoft.Json;
using ebill.Data;

namespace ebill.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "BasicAuthentication")]
public class NIBSSController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private IUnitOfWork _unitOfWork;
    private ILogger<dynamic> _log;


    public NIBSSController(IUnitOfWork unitOfWork, ILogger<dynamic> log)
    {
        _unitOfWork = unitOfWork;
        _log = log;
    }

    // private readonly ILogger<NIBSSController> _logger;

    // public NIBSSController(ILogger<NIBSSController> logger)
    // {
    //     _logger = logger;
    // }

    [HttpPost("validation")]
    // [HttpPost(Name = "Validation")]
    public ActionResult<IEnumerable<ValidationResponse>> Validation(ValidationRequest model)
    {
        try
        {
            Console.WriteLine("here...");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("error: " + ModelState);
                return BadRequest(new { message = "bad model", data = ModelState, });

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

    [HttpGet("reset")]
    [AllowAnonymous()]
    public ActionResult Rest()
    {
        try
        {
            if (!ModelState.IsValid)
            {

            }


            var settings = _unitOfWork.Settings.Add(new Data.Models.Settings
            {
                iv = Convert.ToBase64String(AESThenHMAC.GetIV()),
                secret = Convert.ToBase64String(AESThenHMAC.NewKey()),
            });

            Console.WriteLine("db settings...." + JsonConvert.SerializeObject(settings));
            return Ok(settings);

            //  return Ok(new
            // {
            //     iv = settings.iv,
            //     key = settings.secret,
            // });
        }
        catch (Exception ex)
        {
            return Ok();
        }

    }


    [HttpGet("hmac")]
    [AllowAnonymous()]
    public ActionResult GenerateHmac(HmacObject val)
    {
        try
        {
            if (!ModelState.IsValid)
            {

            }

            return Ok(new
            {
                resp = AESThenHMAC.SimpleEncrypt(
                    JsonConvert.SerializeObject(val.request),
                   Convert.FromBase64String(val.key),
                    Convert.FromBase64String(val.key),
                     Convert.FromBase64String(val.iv))
            });


        }
        catch (Exception ex)
        {
            return Ok();
        }

    }

}
