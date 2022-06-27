
namespace ebill.Handlers;

using ebill.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Claims;
using Newtonsoft.Json;
using ebill.Extensions;
using ebill.Data;
// using Microsoft.AspNet.WebApi.Client;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    // private async Task<string> GetBody(HttpRequest Request)
    // {
    //     string content = "";
    //     HttpContext.Current.Request.InputStream.Position = 0;
    //     using (var reader = new StreamReader(
    //              Request.InputStream, System.Text.Encoding.UTF8, true, 4096, true))
    //     {
    //         content = reader.ReadToEnd();
    //     }
    //     //Reset the stream
    //     HttpContext.Current.Request.InputStream.Position = 0;

    //     return content;
    // }
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUnitOfWork unitOfWork
        ) : base(options, logger, encoder, clock)
    {
        //  _logger = (ILogger) logger;
        // _crypto = crypto;
        _unitOfWork = unitOfWork;

    }

    private readonly ILogger _logger;
    private readonly ICryptography _crypto;
    private readonly IUnitOfWork _unitOfWork;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // skip authentication if endpoint has [AllowAnonymous] attribute
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            return Task.FromResult(AuthenticateResult.NoResult());


        // if (!Request.Headers.ContainsKey("Authorization"))
        //     return AuthenticateResult.Fail("Missing Authorization Header");

        // check for secret setup
        Data.Models.Settings settings = _unitOfWork.Settings.GetSingleOrDefault(item => item.id > 0);
        Console.WriteLine("settings: " + settings.secret);
        if (settings == null || string.IsNullOrEmpty(settings?.secret))
        {
            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"localhost\"");
            return Task.FromResult(AuthenticateResult.Fail("System Not setup"));
        }

        var authHeader = Request.Headers["Authorization"].ToString();
        var signature = Request.Headers["SIGNATURE"].ToString();
        var hmac = Request.Headers["HASH"].ToString();
        var str = Request.GetRawBodyStringAsync().GetAwaiter().GetResult();

        //check if signature is equal to sha256 of todays date
        var today = DateTime.Now.ToString("yyyyMMdd");
        var secret = settings.secret;
        Cryptography crypt = new Cryptography(_unitOfWork);
        var dateSha256 = crypt.EncryptSHA256($"{today}{secret}");
        var username = crypt.Base64Decode(authHeader);

        if (dateSha256.ToLower() != signature.ToLower())
        {
            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"localhost\"");
            // Response.Headers.Add("date-x", dateSha256);
            // Response.Headers.Add("signature-x", signature);
            // Response.Headers.Add("username", username);
            return Task.FromResult(AuthenticateResult.Fail("Invalid SIGNATURE Header"));
        }

        var claims = new[] { new Claim("name", "apiUser"), new Claim(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims, "Basic");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));


    }
}
