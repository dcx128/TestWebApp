using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWebApp.Api.V0.Types;
using TestWebApp.Extensions;
using TestWebApp.Model;
using TestWebApp.Model.Types;
using TestWebApp.Services;

namespace TestWebApp.Api.V0
{
    [ApiController]
    [Route("api/v0/user")]
    public class UserService(
        ILogger<UserService> logger,
        IDbContextFactory<AppDbContext> contextFactory,
        IHashService hashService,
        ILdapService ldapService)
        : ControllerBase
    {
        [HttpPost]
        [Route("checkin")]
        public async Task<IActionResult> CheckinAsync(
            [FromHeader][RegularExpression("[a-zA-Z0-9]+")] string userName,
            [FromHeader] string password,
            CancellationToken cancellationToken)
        {
            var response = new Response { UserName = userName };
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(response.ToJson());
            }

            try
            {
                using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
                if (context.Users.Any(u => u.UserName == userName))
                {
                    return Conflict(response.ToJson());
                }

                var user = new User
                {
                    UserName = userName,
                    PassHash = GeneratePassHash(userName, password),
                };
                user.Session = GenerateSession(user.PassHash);
                context.Users.Add(user);

                if (await context.SaveChangesAsync(cancellationToken) > 0)
                {
                    response.Session = user.Session;
                    return Ok(response.ToJson());
                }
                else
                {
                    return BadRequest(response.ToJson());
                }
            }
            catch (OperationCanceledException)
            {
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(CheckinAsync)} error, {nameof(userName)}: '{userName}', ex: '{ex.Message}'");
                return Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(
            [FromHeader] string userName,
            [FromHeader] string password,
            CancellationToken cancellationToken)
        {
            var response = new Response { UserName = userName };
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(response.ToJson());
            }

            try
            {
                using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
                if (user == null)
                {
                    return NotFound(response.ToJson());
                }

                if (!string.Equals(user.PassHash, GeneratePassHash(userName, password), StringComparison.Ordinal))
                {
                    return Forbid();
                }

                user.Session = GenerateSession(user.PassHash);
                if (await context.SaveChangesAsync(cancellationToken) > 0)
                {
                    response.Session = user.Session;
                    return Ok(response.ToJson());
                }
                else
                {
                    return BadRequest(response.ToJson());
                }
            }
            catch (OperationCanceledException)
            {
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(LoginAsync)} error, {nameof(userName)}: '{userName}', ex: '{ex.Message}'");
                return Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> AuthAsync(
            [FromHeader] string userName,
            [FromHeader] string session,
            CancellationToken cancellationToken)
        {
            var response = new Response { UserName = userName };
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(session))
            {
                return Forbid(response.ToJson());
            }

            try
            {
                using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
                if (user == null)
                {
                    return Forbid();
                }

                if (!string.Equals(user.Session, session, StringComparison.Ordinal))
                {
                    return Forbid();
                }

                response.Session = session;
                return Ok(response.ToJson());
            }
            catch (OperationCanceledException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(AuthAsync)} error, {nameof(userName)}: '{userName}', ex: '{ex.Message}'");
                return Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogoutAsync(
            [FromHeader] string userName,
            [FromHeader] string session,
            CancellationToken cancellationToken)
        {
            var response = new Response { UserName = userName };
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest(response.ToJson());
            }

            try
            {
                using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
                if (user == null)
                {
                    return NotFound(response.ToJson());
                }

                if (!string.Equals(user.Session, session, StringComparison.Ordinal))
                {
                    return Forbid();
                }

                user.Session = string.Empty;
                if (await context.SaveChangesAsync(cancellationToken) > 0)
                {
                    response.Session = user.Session;
                    return Ok(response.ToJson());
                }
                else
                {
                    return BadRequest(response.ToJson());
                }
            }
            catch (OperationCanceledException)
            {
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(LoginAsync)} error, {nameof(userName)}: '{userName}', ex: '{ex.Message}'");
                return Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("login/ldap")]
        public async Task<IActionResult> LoginLdapAsync(
            [FromHeader] string userName,
            [FromHeader] string password,
            CancellationToken cancellationToken)
        {
            var response = new Response { UserName = userName };
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(response.ToJson());
            }

            try
            {
                return await ldapService.LoginAsync(userName, password, cancellationToken)
                    ? Ok(response.ToJson())
                    : Forbid();
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(LoginLdapAsync)} error, {nameof(userName)}: '{userName}', ex: '{ex.Message}'");
                return Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        private string GeneratePassHash(string userName, string password) => hashService.GetHash($"{userName}:{password}");
        private string GenerateSession(string passHash) => hashService.GetHash($"{passHash}+{DateTime.UtcNow}");
    }
}
