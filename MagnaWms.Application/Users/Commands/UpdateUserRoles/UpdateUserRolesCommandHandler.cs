using MagnaWms.Application.Core.Abstractions;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Users.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.UserAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Users.Commands.UpdateUserRoles;

public sealed class UpdateUserRolesCommandHandler
    : IRequestHandler<UpdateUserRolesCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserRolesCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(
        UpdateUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserDto>.Failure(
                new Error(ErrorCode.NotFound, $"User with ID {request.UserId} was not found."));
        }

        await _userRepository.SetUserRolesAsync(user.Id, request.RoleIds, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        IReadOnlyList<string> roles = await _userRepository.GetUsersRoles(user.Id, cancellationToken);
        IReadOnlyList<long> warehouses = await _userRepository.GetUsersWarehousesAsync(user.Id, cancellationToken);
        IReadOnlyList<string> permissions = await _userRepository.GetUsersPermissions(user.Id, cancellationToken);

        UserDto dto = _mapper.Map<UserDto>((user, roles, warehouses, permissions));

        return Result<UserDto>.Success(dto);
    }
}
