using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Users.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.UserAggregate;
using MediatR;

namespace MagnaWms.Application.Users.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand, Result<Success>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<Success>.Failure(
                new Error(ErrorCode.NotFound, $"User with ID {request.UserId} was not found."));
        }

        IReadOnlyList<string> userRoles = await _userRepository.GetUsersRoles(user.Id, cancellationToken);

        if (userRoles.ToList().Contains("SuperAdmin", StringComparer.OrdinalIgnoreCase))
        {
            return Result<Success>.Failure(
                new Error(ErrorCode.Forbidden, $"Not high enough permissions to delete this user."));
        }

        await _userRepository.RemoveUserRolesAsync(request.UserId, cancellationToken);

        await _userRepository.RemoveUserWarehousesAsync(request.UserId, cancellationToken);

        await _userRepository.RemoveRefreshTokensAsync(request.UserId, cancellationToken);

        _userRepository.Remove(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Success>.Success(new Success("Deleted"));
    }
}
