using System.Collections.Generic;
using TemplateName.Shared.Abstractions.Auth;

namespace TemplateName.Shared.Infrastructure.Auth.JWT;

public interface IJsonWebTokenManager
{
    JsonWebToken CreateToken(string userId, string email = null, string role = null,
        IDictionary<string, IEnumerable<string>> claims = null);
}

