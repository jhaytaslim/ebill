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
    private IEmailService _emailService;
    private IEmailSettings _emailSettings;


    public NIBSSController(IUnitOfWork unitOfWork, ILogger<dynamic> log, IEmailService emailService, IEmailSettings emailSettings)
    {
        _unitOfWork = unitOfWork;
        _log = log;
        _emailService = emailService;
        _emailSettings = emailSettings;
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
            var test = System.Text.Encoding.UTF8.GetBytes(settings.secret);
            Console.WriteLine("test..." + settings.secret + "\n" + test.Length);
            Console.WriteLine("hmac..." + "\n" + AESThenHMAC.Decrypt(
                    hmac,
                    settings.secret,
                    settings.iv)
                                );

            // desrialize hmac string into typed request object
            var model = JsonConvert.DeserializeObject<ValidationRequest>(AESThenHMAC.Decrypt(
                    hmac,
                    settings.secret,
                    settings.iv));

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
            Console.WriteLine("hmac..." + "\n" + AESThenHMAC.Decrypt(
                    hmac,
                    settings.secret,
                    settings.iv)
                                );

            // desrialize hmac string into typed request object
            var model = JsonConvert.DeserializeObject<NotificationRequest>(AESThenHMAC.Decrypt(
                    hmac,
                    settings.secret,
                    settings.iv));

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
            // await _emailService.SendEmail(new MailVM
            // {
            //     Title = "New Secret/IV Pair",
            //     Recipient = _emailSettings.Recipient,
            //     Message = $"Please find below the new details\n \n"

            // });
            Data.Models.Settings settings = _unitOfWork.Settings.GetSingleOrDefault(item => item.id > 0);

            if (settings == null)
            {
                settings = await _unitOfWork.Settings.Add(new Data.Models.Settings
                {
                    billerName = "akindele04",
                    iv = AESThenHMAC.IV(),
                    secret = AESThenHMAC.Key(),
                });
            }
            else
            {
                settings.iv = AESThenHMAC.IV();
                settings.secret = AESThenHMAC.Key();
                settings = await _unitOfWork.Settings.Update(settings);
            }

            // await _emailService.SendEmail(new MailVM
            // {
            //     Title = "New Secret/IV Pair",
            //     Recipient = _emailSettings.Recipient,
            //     Message = $"Please find below the new details\n Secret/Key: {settings.secret}\n IV: {settings.iv}.\n"

            // });
            return Ok(settings);
        }
        catch (Exception ex)
        {
            return Ok(new { msg = "error sending values" });
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

            var resp = AESThenHMAC.Encrypt(
                    val.request.ToString(),
                    val.key,
                    val.iv);

            return Ok(new
            {
                resp = resp
            });


        }
        catch (Exception ex)
        {
            Console.WriteLine("err: " + ex.Message + " : \n" + ex.StackTrace);
            return Ok();
        }

    }

}
