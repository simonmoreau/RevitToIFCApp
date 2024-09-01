using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
