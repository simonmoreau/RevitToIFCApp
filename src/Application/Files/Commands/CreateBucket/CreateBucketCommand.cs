using MediatR;

namespace Application.Files.Commands.CreateBucket
{
    public class CreateBucketCommand : IRequest<string>
    {
        public string? Name { get; set; }
    }
}
