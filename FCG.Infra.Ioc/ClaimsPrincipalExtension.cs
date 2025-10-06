using System.Security.Claims;
namespace FCG.Infra.Ioc
{
    public static class ClaimsPrincipalExtension
    {
        public static int GetId(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst("id");
            if (idClaim != null)
                return int.Parse(idClaim.Value);
            else
                return 0;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            var emailClaim = user.FindFirst("email");
            if (emailClaim != null)
                return emailClaim.Value;
            else
                return string.Empty;
        }
    }
}
