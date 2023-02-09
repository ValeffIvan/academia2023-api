using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NovitSoftware.Academia.Persistence;
using NovitSoftware.Academia.Services.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace NovitSoftware.Academia.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly AplicacionDbContext context;

    public AccountController(IConfiguration configuration, AplicacionDbContext context)
    {
        this.configuration = configuration;
        this.context = context;
    }


    [HttpPost("Register")]
    [AllowAnonymous]
    public ActionResult<User> Register(UserRegisterDto request)
    {
        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Name = request.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Roles = new List<Role>()
        };

        var role = context.Roles.FirstOrDefault(x => x.Name == request.Role);

        if (role is null)
        {
            return BadRequest("Rol inexistente.");
        }

        user.Roles.Add(role);

        context.Users.Add(user);

        context.SaveChanges();

        var userCreated = context.Users.FirstOrDefault(x => x.Id == user.Id);

        return Ok(new { Id = userCreated.Id, Username = userCreated.Username, Role = userCreated.Roles.FirstOrDefault().Name });
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public ActionResult<string> Login(UserDto request)
    {
        var user = context.Users.Where(x => x.Username == request.Username).Include(x => x.Roles).First();

        if (user is null)
        {
            return BadRequest("Usuario inexistente.");
        }

        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return BadRequest("Contraseña incorrecta.");
        }

        string token = CreateToken(user);

        return Ok(token);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [HttpPost("CreateRole")]
    public ActionResult<string> Role(RoleDto request)
    {
        var role = new Role()
        {
            Name = request.Name
        };

        context.Roles.Add(role);

        context.SaveChanges();

        return Ok(role);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [HttpPut("/api/Account/User/{userId:Guid}/AddRole/{roleId:Guid}")]
    public ActionResult<string> AddRole(Guid userId, Guid roleId)
    {
        var user = context.Users.FirstOrDefault(x => x.Id == userId);

        if (user is null) 
        {
            return BadRequest("Usuario inexistente.");
        }

        var role = context.Roles.FirstOrDefault(x => x.Id == roleId);

        if (role is null)
        {
            return BadRequest("Rol inexistente.");
        }

        user.Roles.Add(role);

        context.SaveChanges();

        return Ok("Rol asociado");
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [HttpGet("GetUsers")]
    public ActionResult Users()
    {
        var users = context.Users.Include(x => x.Roles).ToList().Select(x => new { x.Id, x.Username, x.PasswordHash, roles = x.Roles.Select(y => new { y.Id, y.Name })});
        return Ok(users);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [HttpGet("GetRoles")]
    public ActionResult Roles()
    {
        var roles = context.Roles.Include(x => x.Users).ToList().Select(x => new { x.Id, x.Name, users = x.Users.Select(y => new {y.Id, y.Username})});
        return Ok(roles);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)            
        };

        string roles = string.Join(",", user.Roles.Select(x => x.Name));

        claims.Add(new Claim(ClaimTypes.Role, roles));

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("Token:Key").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}
