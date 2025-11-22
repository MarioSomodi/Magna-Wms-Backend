using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Users.Repository;
using MagnaWms.Contracts;
using MagnaWms.Contracts.Errors;
using MagnaWms.Domain.UserAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserDto>.Failure(
                new Error(ErrorCode.NotFound, $"User with ID {request.UserId} was not found."));
        }

        IReadOnlyList<string> roles = await _userRepository.GetUsersRoles(request.UserId, cancellationToken);
        IReadOnlyList<long> warehouses = await _userRepository.GetUsersWarehousesAsync(request.UserId, cancellationToken);
        IReadOnlyList<string> permissions = await _userRepository.GetUsersPermissions(user.Id, cancellationToken);

        UserDto dto = _mapper.Map<UserDto>((user, roles, warehouses, permissions));

        return Result<UserDto>.Success(dto);
    }
}
