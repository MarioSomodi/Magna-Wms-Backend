using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Users.Repository;
using MagnaWms.Contracts.Authentication;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.UserAggregate;
using MediatR;

namespace MagnaWms.Application.Authentication.Command.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _hasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository repo,
        IPasswordHasherService hasher,
        IUnitOfWork uow)
    {
        _userRepository = repo;
        _hasher = hasher;
        _unitOfWork = uow;
    }

    public async Task<Result<RegisterResponse>> Handle(
        RegisterCommand request, CancellationToken cancellationToken)
    {
        User? existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (existing is not null)
        {
            return Result<RegisterResponse>.Failure(
                new Error(ErrorCode.Conflict, "A user with this email already exists."));
        }

        string hash = _hasher.HashPassword(request.Password);

        var user = new User(request.Email, hash);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RegisterResponse>.Success(new RegisterResponse(user.Id, user.Email));
    }
}
