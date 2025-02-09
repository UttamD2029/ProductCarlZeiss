using Microsoft.AspNetCore.Identity;

namespace ProductZeissApi.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
