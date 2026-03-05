using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TemplateName.Shared.Abstractions.Modules;
using TemplateName.Shared.Infrastructure.Auth.JWT;

namespace TemplateName.Shared.Infrastructure.Auth;

public static class Extensions
{
    private const string SectionName = "auth";
    private const string AccessTokenCookieName = "__access_token";
    private const string AuthorizationHeader = "authorization";

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration,
        IEnumerable<IModule> modules = null, Action<JwtBearerOptions> optionsFactory = null)
    {
        var section = configuration.GetSection(SectionName);
        var options = section.BindOptions<AuthOptions>();
        services.Configure<AuthOptions>(section);

        if (!section.Exists())
        {
            return services;
        }

        if (options.Jwt is null)
        {
            throw new InvalidOperationException("JWT options cannot be null.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = options.Jwt.RequireAudience,
            ValidIssuer = options.Jwt.ValidIssuer,
            ValidIssuers = options.Jwt.ValidIssuers,
            ValidateActor = options.Jwt.ValidateActor,
            ValidAudience = options.Jwt.ValidAudience,
            ValidAudiences = options.Jwt.ValidAudiences,
            ValidateAudience = options.Jwt.ValidateAudience,
            ValidateIssuer = options.Jwt.ValidateIssuer,
            ValidateLifetime = options.Jwt.ValidateLifetime,
            ValidateTokenReplay = options.Jwt.ValidateTokenReplay,
            ValidateIssuerSigningKey = options.Jwt.ValidateIssuerSigningKey,
            SaveSigninToken = options.Jwt.SaveSigninToken,
            RequireExpirationTime = options.Jwt.RequireExpirationTime,
            RequireSignedTokens = options.Jwt.RequireSignedTokens,
            ClockSkew = TimeSpan.Zero
        };

        var hasCertificate = false;
        var algorithm = options.Algorithm;
        SecurityKey securityKey = null;
        if (options.Certificate is not null)
        {
            X509Certificate2 certificate = null;
            var password = options.Certificate.Password;
            var hasPassword = !string.IsNullOrWhiteSpace(password);

            if (!string.IsNullOrWhiteSpace(options.Certificate.Location))
            {
                certificate = X509CertificateLoader.LoadPkcs12FromFile(
                    options.Certificate.Location,
                    hasPassword ? password : null);

                var keyType = certificate.HasPrivateKey
                    ? "with private key"
                    : "with public key only";
                Console.WriteLine(
                    $"Loaded X.509 certificate from location: '{options.Certificate.Location}' {keyType}.");
            }

            if (!string.IsNullOrWhiteSpace(options.Certificate.RawData))
            {
                var rawData = Convert.FromBase64String(options.Certificate.RawData);
                certificate = X509CertificateLoader.LoadPkcs12(rawData, 
                    hasPassword ? password : null);

                var keyType = certificate.HasPrivateKey
                    ? "with private key"
                    : "with public key only";
                Console.WriteLine(
                    $"Loaded X.509 certificate from raw data {keyType}.");
            }

            if (certificate is not null)
            {
                if (string.IsNullOrWhiteSpace(options.Algorithm))
                {
                    algorithm = SecurityAlgorithms.RsaSha256;
                }

                hasCertificate = true;
                securityKey = new X509SecurityKey(certificate);
                tokenValidationParameters.IssuerSigningKey = securityKey;
                var actionType = certificate.HasPrivateKey ? "issuing" : "validating";
                Console.WriteLine($"Using X.509 certificate for {actionType} tokens.");
            }
        }

        if (!hasCertificate)
        {
            if (string.IsNullOrWhiteSpace(options.Jwt.IssuerSigningKey))
            {
                throw new InvalidOperationException("Missing issuer signing key.");
            }

            if (string.IsNullOrWhiteSpace(options.Algorithm))
            {
                algorithm = SecurityAlgorithms.HmacSha256;
            }

            var rawKey = Encoding.UTF8.GetBytes(options.Jwt.IssuerSigningKey);
            securityKey = new SymmetricSecurityKey(rawKey);
            tokenValidationParameters.IssuerSigningKey = securityKey;
            Console.WriteLine("Using symmetric encryption for issuing tokens.");
        }

        if (!string.IsNullOrWhiteSpace(options.Jwt.AuthenticationType))
        {
            tokenValidationParameters.AuthenticationType = options.Jwt.AuthenticationType;
        }

        if (!string.IsNullOrWhiteSpace(options.Jwt.NameClaimType))
        {
            tokenValidationParameters.NameClaimType = options.Jwt.NameClaimType;
        }

        if (!string.IsNullOrWhiteSpace(options.Jwt.RoleClaimType))
        {
            tokenValidationParameters.RoleClaimType = options.Jwt.RoleClaimType;
        }

        services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = options.Jwt.Authority;
                o.Audience = options.Jwt.Audience;
                o.MetadataAddress = options.Jwt.MetadataAddress ?? string.Empty;
                o.SaveToken = options.Jwt.SaveToken;
                o.RefreshOnIssuerKeyNotFound = options.Jwt.RefreshOnIssuerKeyNotFound;
                o.RequireHttpsMetadata = options.Jwt.RequireHttpsMetadata;
                o.IncludeErrorDetails = options.Jwt.IncludeErrorDetails;
                o.TokenValidationParameters = tokenValidationParameters;
                if (!string.IsNullOrWhiteSpace(options.Jwt.Challenge))
                {
                    o.Challenge = options.Jwt.Challenge;
                }
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue(AccessTokenCookieName, out var token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    },
                };

                optionsFactory?.Invoke(o);
            });

        if (securityKey is not null)
        {
            services.AddSingleton(new SecurityKeyDetails(securityKey, algorithm));
            services.AddSingleton<IJsonWebTokenManager, JsonWebTokenManager>();
        }

        services.AddSingleton(tokenValidationParameters);
        
        var policies = modules?.SelectMany(x => x.Policies ?? Enumerable.Empty<string>()) ??
                       Enumerable.Empty<string>();
        services.AddAuthorization(authorization =>
        {
            foreach (var policy in policies)
            {
                authorization.AddPolicy(policy, x => x.RequireClaim("permissions", policy));
            }
        });

        return services;
    }

    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.Headers.Remove(AuthorizationHeader);

            if (ctx.Request.Cookies.ContainsKey(AccessTokenCookieName))
            {
                var authenticateResult = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                if (authenticateResult.Succeeded && authenticateResult.Principal is not null)
                {
                    ctx.User = authenticateResult.Principal;
                }
            }

            await next();
        });

        return app;
    }
}



