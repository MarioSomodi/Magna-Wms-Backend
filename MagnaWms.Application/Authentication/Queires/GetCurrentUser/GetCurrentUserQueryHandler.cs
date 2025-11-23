using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Users.Repository;
using MagnaWms.Contracts.Errors;
using MapsterMapper;
using MediatR;
using MagnaWms.Domain.UserAggregate;
using MagnaWms.Contracts.Users;

namespace MagnaWms.Application.Authentication.Queires.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(
        ICurrentUser currentUser,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            return Result<UserDto>.Failure(
                new Error(ErrorCode.Unauthorized, "Authentication required."));
        }

        User? user = await _userRepository.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<UserDto>.Failure(
                new Error(ErrorCode.NotFound, "User not found."));
        }

        IReadOnlyList<string> roles = await _userRepository.GetUsersRoles(user.Id, cancellationToken);
        IReadOnlyList<long> warehouses = await _userRepository.GetUsersWarehousesAsync(user.Id, cancellationToken);
        IReadOnlyList<string> permissions = await _userRepository.GetUsersPermissions(user.Id, cancellationToken);

        UserDto dto = _mapper.Map<UserDto>((user, roles, warehouses, permissions));

        return Result<UserDto>.Success(dto);
    }
}
