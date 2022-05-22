
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

    public async Task<string> InvokeAsync(HttpRequest request)
    {
        request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var requestContent = Encoding.UTF8.GetString(buffer);

        request.Body.Position = 0;  //rewinding the stream to 0

        Console.WriteLine($"Request Body: {requestContent}");

        return requestContent;
    }

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

        var iv = Convert.FromBase64String("FvKOOxf90nHLE+m16ydE9g==");
        var cryptKey = Convert.FromBase64String("JzsCuaGAg6TsmyWXiMc4UvV4kUkRKykuhFBSNfs6luA=");

        Console.WriteLine("str..." + str.GetType().FullName + "\n" + str);

        //encrypt
        var gMac = AESThenHMAC.SimpleEncrypt(str,
        cryptKey,
        cryptKey,
         iv);

         var pass = AESThenHMAC.SimpleDecrypt(hmac,
        cryptKey,
        cryptKey);
        Console.WriteLine("pass: " + pass);

        // var asaf = AESThenHMAC.SimpleDecrypt(asd,
        // cryptKey,
        //  cryptKey
        //  );
        // Console.WriteLine("asaf: " + asaf);

        if (dateSha256.ToLower() != signature.ToLower())
        {
            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"localhost\"");
            Response.Headers.Add("date-x", dateSha256);
            Response.Headers.Add("signature-x", signature);
            Response.Headers.Add("username", username);
            // Response.Body = new {msg = "Invalid SIGNATURE header" };
            return Task.FromResult(AuthenticateResult.Fail("Invalid SIGNATURE Header"));
        }

        // Console.WriteLine("gMac.ToLower(): " + gMac.ToLower());
        // Console.WriteLine("hmac.ToLower(): " + hmac.ToLower());
        // if (gMac.ToLower() != hmac.ToLower())
        // {
        //     Response.StatusCode = 403;
        //     Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"localhost\"");
        //     Response.Headers.Add("gMac-x", gMac);
        //     // Response.Body = new {msg = "Invalid SIGNATURE header" };
        //     return Task.FromResult(AuthenticateResult.Fail("Invalid hmac Header"));
        // }

        var claims = new[] { new Claim("name", "apiUser"), new Claim(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims, "Basic");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        Console.WriteLine("chgvgh: ");
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