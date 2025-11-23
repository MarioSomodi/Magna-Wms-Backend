using MagnaWms.Application.Core.Results;
using MagnaWms.Application.Users.Repository;
using MagnaWms.Contracts.Users;
using MagnaWms.Domain.UserAggregate;
using MapsterMapper;
using MediatR;

namespace MagnaWms.Application.Users.Queries.GetAllUsers;

public sealed class GetAllUsersQueryHandler
    : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<User> users = await _userRepository.GetAllAsync(cancellationToken);

        var list = new List<UserDto>(users.Count);

        foreach (User user in users)
        {
            IReadOnlyList<string> roles = await _userRepository.GetUsersRoles(user.Id, cancellationToken);
            IReadOnlyList<long> warehouses = await _userRepository.GetUsersWarehousesAsync(user.Id, cancellationToken);
            IReadOnlyList<string> permissions = await _userRepository.GetUsersPermissions(user.Id, cancellationToken);

            UserDto dto = _mapper.Map<UserDto>((user, roles, warehouses, permissions));
            list.Add(dto);
        }

        return Result<IReadOnlyList<UserDto>>.Success(list);
    }
}
