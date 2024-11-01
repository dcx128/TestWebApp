using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWebApp.Api.V0.Types;
using TestWebApp.Extensions;
using TestWebApp.Model;
using TestWebApp.Model.Types;
using TestWebApp.Services;
using TestWebApp.Settings;

namespace TestWebApp.Api.V0
{
    [ApiController]
    [Route("api/v0/user")]
    public class UserService(
        ILogger<UserService> logger,
        IDbContextFactory<AppDbContext> contextFactory,
        IHashService hashService,
        IProductSettings productSettings)
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
                    return Forbid(response.ToJson());
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
                    return Forbid(response.ToJson());
                }

                if (!string.Equals(user.Session, session, StringComparison.Ordinal))
                {
                    return Forbid(response.ToJson());
                }

                response.Session = session;
                return Ok(response.ToJson());
            }
            catch (OperationCanceledException)
            {
                return Forbid(response.ToJson());
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
        public IActionResult LoginLdap(
            [FromHeader] string userName,
            [FromHeader] string password)
        {
            var response = new Response { UserName = userName };
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(response.ToJson());
            }

            try
            {
                var id = new LdapDirectoryIdentifier(productSettings.ADSettings.Host, productSettings.ADSettings.Port);
                using var connection = new LdapConnection(id);
                connection.AuthType = productSettings.ADSettings.AuthType;
                connection.SessionOptions.ProtocolVersion = productSettings.ADSettings.ProtocolVersion;
                var credentials = new NetworkCredential(userName, password);
                connection.Bind(credentials);
                return Ok(response.ToJson());
            }
            catch (LdapException)
            {
                return Forbid(response.ToJson());
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(LoginLdap)} error, {nameof(userName)}: '{userName}', ex: '{ex.Message}'");
                return Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        private string GeneratePassHash(string userName, string password) => hashService.GetHash($"{userName}:{password}");
        private string GenerateSession(string passHash) => hashService.GetHash($"{passHash}+{DateTime.UtcNow}");
    }
}
