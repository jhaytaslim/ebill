
namespace ebill.Handlers;

using ebill.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Claims;

public class BasicAuthenticationHandler: AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
        ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var signature = Request.Headers["SIGNATURE"].ToString();

        //check if signature is equal to sha256 of todays date
        Cryptography crypt = new Cryptography();
        var dateSha256 = crypt.EncryptSHA256(DateTime.Now.ToString("yyyyMMdd"));

        if (dateSha256 != signature)
        {
            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"localhost\"");
            return Task.FromResult(AuthenticateResult.Fail("Invalid SIGNATURE Header"));
        }

        var claims = new[] { new Claim("name", "apiUser"), new Claim(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims, "Basic");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));

        // if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
        // {
        //     var token = authHeader.Substring("Basic ".Length).Trim();
        //     System.Console.WriteLine(token);
        //     var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        //     var credentials = credentialstring.Split(':');
        //     if (credentials[0] == "admin" && credentials[1] == "admin")
        //     {
        //         var claims = new[] { new Claim("name", credentials[0]), new Claim(ClaimTypes.Role, "Admin") };
        //         var identity = new ClaimsIdentity(claims, "Basic");
        //         var claimsPrincipal = new ClaimsPrincipal(identity);
        //         return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        //     }

        //     Response.StatusCode = 401;
        //     Response.Headers.Add("WWW-Authenticate", "Basic realm=\"dotnetthoughts.net\"");
        //     return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        // }
        // else
        // {
        //     Response.StatusCode = 401;
        //     Response.Headers.Add("WWW-Authenticate", "Basic realm=\"dotnetthoughts.net\"");
        //     return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        // }

    }
}



// public class BasicAuthMiddleware
// {
//     private readonly RequestDelegate _next;

//     public BasicAuthMiddleware(RequestDelegate next)
//     {
//         _next = next;
//     }

//     public async Task Invoke(HttpContext context, IUserService userService)
//     {
//         try
//         {
//             var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
//             var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
//             var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
//             var username = credentials[0];
//             var password = credentials[1];

//             // authenticate credentials with user service and attach user to http context
//             context.Items["User"] = "await userService.Authenticate(username, password)";
//         }
//         catch
//         {
//             // do nothing if invalid auth header
//             // user is not attached to context so request won't have access to secure routes
//         }

//         await _next(context);
//     }
// }