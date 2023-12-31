﻿using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TestTask.Application.Contracts;
using TestTask.Application.Contracts.Common;
using TestTask.Application.Implementations.Extensions;
using TestTask.Application.Implementations.Extensions.Extensions;
using TestTask.Application.Services;
using TestTask.DAL;
using TestTask.Domain.Constants;
using TestTask.Domain.Entities;

namespace TestTask.Application.Implementations.Services;

internal class UserService(
	TestTaskDbContext dbContext,
	IValidator<PagingOptions> pagingOptionsValidator,
	IValidator<UsersFilteringOptions> filteringOptionsValidator,
	IFilteringOptionsChecker<UsersFilteringOptions> filteringOptionsChecker,
	IValidator<UserRegisterDTO> registerDtoValidator,
	IValidator<UserUpdateDTO> updateDtoValidator) : IUserService
{
	private readonly TestTaskDbContext _dbContext = dbContext;
	private readonly IValidator<PagingOptions> _pagingOptionsValidator = pagingOptionsValidator;
	private readonly IValidator<UsersFilteringOptions> _filteringOptionsValidator = filteringOptionsValidator;
	private readonly IFilteringOptionsChecker<UsersFilteringOptions> _filteringOptionsChecker = filteringOptionsChecker;
	private readonly IValidator<UserRegisterDTO> _registerDtoValidator = registerDtoValidator;
	private readonly IValidator<UserUpdateDTO> _updateDtoValidator = updateDtoValidator;

	public async Task<Result<UsersPage>> GetAsync(
		UserId requesterId,
		UsersSortingOptions sortingOptions,
		PagingOptions? pagingOptions = null,
		UsersFilteringOptions? filteringOptions = null,
		CancellationToken cancellationToken = default)
	{
		var user = await _dbContext
			.Users
			.Include(e => e.Roles)
			.ThenInclude(e => e.Role)
			.SingleOrDefaultAsync(e => e.Id == requesterId, cancellationToken);

		if (user is null)
		{
			return Result.Failure<UsersPage>(Errors.EntityWithPassedIdIsNotExists(nameof(User)));
		}

		var isActionPermitted = user.IsInRole(Roles.SuperAdmin) || user.IsInRole(Roles.Admin) || user.IsInRole(Roles.Support);
		if (!isActionPermitted)
		{
			return Result.Failure<UsersPage>(Errors.Auth.AccessDenided);
		}

		var pagingOptionsValidationResult = pagingOptions is null ? null : _pagingOptionsValidator.Validate(pagingOptions);
		if (pagingOptionsValidationResult is { IsValid: false })
		{
			return Result.Failure<UsersPage>(pagingOptionsValidationResult.ToString());
		}

		var filteringOptionsValidationResult = filteringOptions is null ? null : _filteringOptionsValidator.Validate(filteringOptions);
		if (filteringOptionsValidationResult is { IsValid: false })
		{
			return Result.Failure<UsersPage>(filteringOptionsValidationResult.ToString());
		}

		var applicabilityResult = filteringOptions is null ? null : _filteringOptionsChecker.IsAppliсable(filteringOptions);
		if (applicabilityResult is { IsFailure: true })
		{
			return Result.Failure<UsersPage>(applicabilityResult.ErrorMessage);
		}

		int totalUsersCount = _dbContext.Users.ApplyFiltering(filteringOptions).Count();
		var users = await _dbContext
			.Users
			.Include(e => e.Roles)
			.ThenInclude(e => e.Role)
			.AsNoTracking()
			.ApplyFiltering(filteringOptions)
			.ApplySorting(sortingOptions)
			.ApplyPaging(pagingOptions)
			.Select(e => e.ToDTO())
			.ToListAsync(cancellationToken);

		return new UsersPage(
			users,
			totalUsersCount,
			sortingOptions,
			pagingOptions,
			filteringOptions);
	}

	public async Task<Result<UserDTO>> GetByIdAsync(UserId requesterId, UserId searchId, CancellationToken cancellationToken = default)
	{
		var requester = await _dbContext
			.Users
			.Include(e => e.Roles)
			.ThenInclude(e => e.Role)
			.SingleOrDefaultAsync(e => e.Id == requesterId, cancellationToken);

		if (requester is null)
		{
			return Result.Failure<UserDTO>("Requester not found.");
		}

		if (requester.Id == searchId)
		{
			return requester.ToDTO();
		}

		var actionPermitted = requester.IsInRole(Roles.Support) || requester.IsInRole(Roles.Admin) || requester.IsInRole(Roles.SuperAdmin);
		if (!actionPermitted)
		{
			return Result.Failure<UserDTO>(Errors.Auth.AccessDenided);
		}

		var searchUser = await _dbContext
			.Users
			.Include(e => e.Roles)
			.ThenInclude(e => e.Role)
			.SingleOrDefaultAsync(e => e.Id == searchId, cancellationToken);

		if (searchUser is null)
		{
			return Result.Failure<UserDTO>(Errors.EntityWithPassedIdIsNotExists(nameof(User)));
		}

		return searchUser.ToDTO();
	}

	public async Task<Result<UserId>> RegisterAsync(UserRegisterDTO userRegisterDTO, CancellationToken cancellationToken = default)
	{
		var validationResult = _registerDtoValidator.Validate(userRegisterDTO);
		if (!validationResult.IsValid)
		{
			return Result.Failure<UserId>(validationResult.ToString());
		}

		if (await _dbContext.Users.IsEmailTakenAsync(userRegisterDTO.Credentials.Email))
		{
			return Result.Failure<UserId>(Errors.User.EmailIsAlreadyTaken);
		}

		string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterDTO.Credentials.Password);
		var user = new User
		{
			Age = userRegisterDTO.Age,
			FullName = userRegisterDTO.FullName,
			Email = userRegisterDTO.Credentials.Email,
			PasswordHash = passwordHash
		};

		var defaultRole = await _dbContext.Roles.GetRoleByTitleAsync(Roles.User);
		user.Roles.Add(new UserRole { RoleId = defaultRole.Id, UserId = user.Id });

		_dbContext.Users.Add(user);
		await _dbContext.SaveChangesAsync(cancellationToken);
		return user.Id;
	}

	public async Task<Result> UpdateAsync(UserId requesterId, UserUpdateDTO userUpdateDTO, CancellationToken cancellationToken = default)
	{
		var validationResult = _updateDtoValidator.Validate(userUpdateDTO);
		if (!validationResult.IsValid)
		{
			return Result.Failure(validationResult.ToString());
		}

		var requester = await _dbContext
			.Users
			.Include(e => e.Roles)
			.ThenInclude(e => e.Role)
			.SingleOrDefaultAsync(e => e.Id == requesterId, cancellationToken);

		if (requester is null)
		{
			return Result.Failure<UserDTO>("Requester not found.");
		}

		var actionPermitted = requester.IsInRole(Roles.Admin) || requester.IsInRole(Roles.SuperAdmin) || requester.Id == userUpdateDTO.Id;
		if (!actionPermitted)
		{
			return Result.Failure(Errors.Auth.AccessDenided);
		}

		if (requester.Id == userUpdateDTO.Id)
		{
			requester.Age = userUpdateDTO.Age;
			requester.FullName = userUpdateDTO.FullName;
			await _dbContext.SaveChangesAsync(cancellationToken);
			return Result.Success();
		}

		var user = await _dbContext
		.Users
		.Include(e => e.Roles)
		.ThenInclude(e => e.Role)
		.SingleOrDefaultAsync(e => e.Id == userUpdateDTO.Id, cancellationToken);

		if (user is null)
		{
			return Result.Failure(Errors.EntityWithPassedIdIsNotExists(nameof(User)));
		}

		user.Age = userUpdateDTO.Age;
		user.FullName = userUpdateDTO.FullName;
		await _dbContext.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}
}
