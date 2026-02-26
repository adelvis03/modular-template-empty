using Microsoft.IdentityModel.Tokens;

namespace TemplateName.Shared.Infrastructure.Auth.JWT;

internal sealed record SecurityKeyDetails(SecurityKey Key, string Algorithm);


