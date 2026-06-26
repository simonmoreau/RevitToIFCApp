using MediatR;

namespace Application.Users.Queries.GetUser
{
    public class GetUserQuery : IRequest<UserDTO>
    {
        public GetUserQuery(string userId)
        {
            UserId = userId;
        }

        public readonly string UserId;
    }
}
