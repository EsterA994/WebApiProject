using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MyJewelry.Services;

public static class JewelryTokenService
{
    private static SymmetricSecurityKey key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(
            "hngfctyfe4edf653t4u5v6w7x8y9y9z0a1b2c3d4e5f6g7h8j9k0l1m2n3o4p5q6r7s8t9u0v1w2x3y4z"
        )
    );
    private static string issuer = "https://MyJewelry.com";

    public static SecurityToken GetToken(List<Claim> claims) =>
        new JwtSecurityToken(
            issuer,
            issuer,
            claims,
            expires: DateTime.Now.AddDays(30.0),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

    public static TokenValidationParameters GetTokenValidationParameters() =>
        new TokenValidationParameters
        {
            ValidIssuer = issuer,
            ValidAudience = issuer,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero, // remove delay of token when expire
        };

    public static string WriteToken(SecurityToken token) =>
        new JwtSecurityTokenHandler().WriteToken(token);
}
