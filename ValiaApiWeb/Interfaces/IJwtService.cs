
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.Security;

namespace ValiaApiWeb.Security
{
    public interface IJwtService
    {
        GeneratedToken GenerateToken(User user);
    }
}


