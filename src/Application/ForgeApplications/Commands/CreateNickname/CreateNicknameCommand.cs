using MediatR;

namespace Application.ForgeApplications.Commands.CreateNickname
{
    public class CreateNicknameCommand : IRequest<string>
    {
        public string Nickname { get; internal set; }
    }
}
