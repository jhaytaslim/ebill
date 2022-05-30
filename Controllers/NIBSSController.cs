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

    [HttpPost("validation")]
    public ActionResult<ValidationResponse> Validation()
    {
        try
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "bad model", data = ModelState, });
            }
            Console.WriteLine("2...");
            Data.Models.Settings settings = _unitOfWork.Settings.GetSingleOrDefault(item => item.id > 0);
            if (settings == null)
            {
                return BadRequest(new { message = "settings not found" });
            }

            // get the encrypted header from the request header
            var hmac = Request.Headers["HASH"].ToString();
            Console.WriteLine("hmac..." + "\n" + AESThenHMAC.SimpleDecrypt(
                                hmac,
                               Convert.FromBase64String(settings.secret),
                                Convert.FromBase64String(settings.secret)));

            // desrialize hmac string into typed request object
            var model = JsonConvert.DeserializeObject<ValidationRequest>(AESThenHMAC.SimpleDecrypt(
                    hmac,
                   Convert.FromBase64String(settings.secret),
                    Convert.FromBase64String(settings.secret)));

            Console.WriteLine("4...");
            //perform ruleset validation here
            var response = _unitOfWork.Oracle.Validation(model);

            //decide on the response
            return Ok(response);
        }
        catch (Exception ex)
        {
            _log.LogInformation(ex.Message + "\n" + ex.StackTrace);

        }

        return new ValidationResponse();
    }

    [HttpPost("notification")]
    public ActionResult<NotificationResponse> Notification()
    {
        try
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "bad model", data = ModelState, });
            }
            Console.WriteLine("2...");
            Data.Models.Settings settings = _unitOfWork.Settings.GetSingleOrDefault(item => item.id > 0);
            if (settings == null)
            {
                return BadRequest(new { message = "settings not found" });
            }

            // get the encrypted header from the request header
            var hmac = Request.Headers["HASH"].ToString();
            Console.WriteLine("hmac..." + "\n" + AESThenHMAC.SimpleDecrypt(
                                hmac,
                               Convert.FromBase64String(settings.secret),
                                Convert.FromBase64String(settings.secret)));

            // desrialize hmac string into typed request object
            var model = JsonConvert.DeserializeObject<NotificationRequest>(AESThenHMAC.SimpleDecrypt(
                    hmac,
                   Convert.FromBase64String(settings.secret),
                    Convert.FromBase64String(settings.secret)));

            Console.WriteLine("4...");
            //perform ruleset validation here
            var response = _unitOfWork.Oracle.Notification(model);

            //decide on the response
            return Ok(response);


        }
        catch (Exception ex)
        {

        }
        return new NotificationResponse();
    }

    [HttpGet("reset")]
    [AllowAnonymous]
    public async Task<ActionResult> Rest()
    {
        try
        {

            if (!ModelState.IsValid)
            {

            }


            Data.Models.Settings settings = _unitOfWork.Settings.GetSingleOrDefault(item => item.id > 0);

            if (settings == null)
            {
                settings = await _unitOfWork.Settings.Add(new Data.Models.Settings
                {
                    billerName = "akindele04",
                    iv = Convert.ToBase64String(AESThenHMAC.GetIV()),
                    secret = Convert.ToBase64String(AESThenHMAC.NewKey()),
                });
            }
            else
            {
                settings.iv = Convert.ToBase64String(AESThenHMAC.GetIV());
                settings.secret = Convert.ToBase64String(AESThenHMAC.NewKey());
                settings = await _unitOfWork.Settings.Update(settings);
            }

            return Ok(settings);
        }
        catch (Exception ex)
        {
            return Ok();
        }

    }

    [HttpGet("hmac")]
    [AllowAnonymous]
    public ActionResult GenerateHmac(HmacObject val)
    {
        try
        {
            if (!ModelState.IsValid)
            {

            }

            Data.Models.Settings settings = _unitOfWork.Settings.GetSingleOrDefault(item => item.id > 0);

            if (settings == null)
            {
                return Ok(new { message = "settings not found" });
            }

            Console.WriteLine("val.request..." + val.request.GetType().FullName + "\n" + val.request.ToString());
            return Ok(new
            {
                resp = AESThenHMAC.SimpleEncrypt(
                    val.request.ToString(),
                   Convert.FromBase64String(settings.secret),
                    Convert.FromBase64String(settings.secret),
                     Convert.FromBase64String(settings.iv))
            });


        }
        catch (Exception ex)
        {
            return Ok();
        }

    }

}
