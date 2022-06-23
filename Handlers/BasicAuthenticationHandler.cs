
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
        ISystemClock clock
        ) : base(options, logger, encoder, clock)
    {
        //  _logger = (ILogger) logger;


    }

    private readonly ILogger _logger;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // skip authentication if endpoint has [AllowAnonymous] attribute
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            return Task.FromResult(AuthenticateResult.NoResult());


        // if (!Request.Headers.ContainsKey("Authorization"))
        //     return AuthenticateResult.Fail("Missing Authorization Header");

        var authHeader = Request.Headers["Authorization"].ToString();
        var signature = Request.Headers["SIGNATURE"].ToString();
        var hmac = Request.Headers["HASH"].ToString();
        var str = Request.GetRawBodyStringAsync().GetAwaiter().GetResult();

        //check if signature is equal to sha256 of todays date
        Cryptography crypt = new Cryptography();
        var dateSha256 = crypt.EncryptSHA256(DateTime.Now.ToString("yyyyMMdd"));
        var username = Cryptography.Base64Decode(authHeader);

Console.WriteLine("dateSha256: " + signature);
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
