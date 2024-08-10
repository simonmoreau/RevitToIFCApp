using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ForgeApplications.Commands.CreateNickname
{
    public class CreateNicknameCommand : IRequest<string>
    {
        public string Nickname { get; internal set; }
    }
}
