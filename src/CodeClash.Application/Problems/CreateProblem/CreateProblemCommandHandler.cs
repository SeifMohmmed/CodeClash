using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Problems.CreateProblem;
internal sealed class CreateProblemCommandHandler
    : IRequestHandler<CreateProblemCommand, Response>
{
    public Task<Response> Handle(CreateProblemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
