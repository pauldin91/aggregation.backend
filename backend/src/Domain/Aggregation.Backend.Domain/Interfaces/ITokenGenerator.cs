using Aggregation.Backend.Domain.Dtos.Auth;

namespace Aggregation.Backend.Domain.Interfaces
{
    public interface ITokenGenerator
    {
        TokenResponse GenerateToken(UserInfoResponse userInfo);
    }
}