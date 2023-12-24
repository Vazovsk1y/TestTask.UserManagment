﻿using TestTask.Application.Contracts;
using TestTask.Application.Contracts.Common;
using TestTask.Domain.Entities;

namespace TestTask.Application.Services;

public interface IUserService
{
	Task<Result<UsersPage>> GetAsync(
		UsersSortingOptions sortingOptions, 
		PagingOptions? pagingOptions = null,
		UsersFilteringOptions? filteringOptions = null,
		CancellationToken cancellationToken = default);

	Task<Result<UserDTO>> GetByIdAsync(UserId userId, CancellationToken cancellationToken = default);

	Task<Result> RegisterAsync(UserRegisterDTO userRegisterDTO, CancellationToken cancellationToken = default);

	Task<Result> UpdateAsync(UserUpdateDTO userUpdateDTO, CancellationToken cancellationToken = default);
}
