using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public const string AdminRole = "LifeAdmin";

    public static bool IsUser(this ClaimsPrincipal principal)
    {
        return principal.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value == "LifeUser");
    }

    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        return principal.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value == AdminRole);
    }
}
