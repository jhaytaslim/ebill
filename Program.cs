using ebill.Data;
using ebill.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ebill.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


// add services to DI container
{
    var services = builder.Services;
    // Add services to the container.
    builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

Console.WriteLine("entering scheme...");
    builder.Services.AddAuthentication("BasicAuthentication")
                    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
                    ("BasicAuthentication", null);
    builder.Services.AddAuthorization();
    

    // custom injections
    builder.Services.Configure<Connections>(builder.Configuration.GetSection("ConnectionStrings"));
    builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

    builder.Services.AddCors();
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();


    // configure DI for application services
    // builder.Services.AddScoped<IUserService, UserService>();

}





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// configure HTTP request pipeline
// {
// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

// custom basic auth middleware
// app.UseMiddleware<BasicAuthMiddleware>();

app.MapControllers();
// }



app.Run();
