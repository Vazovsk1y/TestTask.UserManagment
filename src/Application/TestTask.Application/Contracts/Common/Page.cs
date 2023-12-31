﻿namespace TestTask.Application.Contracts.Common;

public abstract record Page<TItem>
{
	private const int StartCountingFrom = 1;

	public IReadOnlyCollection<TItem> Items { get; }

	public int PageIndex { get; }

	public int TotalCount { get; }

	public int TotalPages { get; }

	public bool HasNextPage => PageIndex < TotalPages;

	public bool HasPreviousPage => PageIndex > StartCountingFrom;

	protected Page(
		IReadOnlyCollection<TItem> items,
		int totalItemsCount,
		PagingOptions? pagingOptions = null)
	{
		Validate(items, totalItemsCount, pagingOptions);

		Items = items;
		TotalCount = totalItemsCount;
		PageIndex = pagingOptions is null ? StartCountingFrom : pagingOptions.PageIndex;
		TotalPages = CalculateTotalPages(pagingOptions, totalItemsCount);
	}

	private static int CalculateTotalPages(PagingOptions? pagingOptions, int totalItemsCount)
	{
		if (pagingOptions is null)
		{
			return StartCountingFrom;
		}

		return totalItemsCount < pagingOptions.PageSize ? StartCountingFrom : (int)Math.Ceiling(totalItemsCount / (double)pagingOptions.PageSize);
	}

	private static void Validate(
		IReadOnlyCollection<TItem> items,
		int totalItemsCount,
		PagingOptions? pagingOptions)
	{
		if (pagingOptions is { PageIndex: <= 0 })
		{
			throw new ArgumentException("Page index must be greater than zero.");
		}

		if (pagingOptions is { PageSize: <= 0 })
		{
			throw new ArgumentException("Page size must be greater than zero.");
		}

		if (items.Count > totalItemsCount)
		{
			throw new ArgumentException("Total items count can't be lower than current items count.");
		}

		if (pagingOptions is null && totalItemsCount != items.Count)
		{
			throw new ArgumentException("If paging options is not passed, total items count must be equal to current items count.");
		}
	}
}

public abstract record Page<TItem, TSorting> : Page<TItem>
	where TSorting : class, ISortingOptions
{
	public TSorting AppliedSorting { get; }

	protected Page(
		IReadOnlyCollection<TItem> items,
		int totalItemsCount,
		TSorting sortingOptions,
		PagingOptions? pagingOptions = null)
		: base(items, totalItemsCount, pagingOptions)
	{
		ArgumentNullException.ThrowIfNull(sortingOptions);
		AppliedSorting = sortingOptions;
	}
}

public abstract record Page<TItem, TSorting, TFiltering> : Page<TItem, TSorting>
	where TSorting : class, ISortingOptions
	where TFiltering : class, IFilteringOptions
{
	public TFiltering? AppliedFiltering { get; }

	protected Page(
		IReadOnlyCollection<TItem> items,
		int totalItemsCount,
		TSorting sortingOptions,
		PagingOptions? pagingOptions = null,
		TFiltering? filteringOptions = null)
		: base(items, totalItemsCount, sortingOptions, pagingOptions)
	{
		AppliedFiltering = filteringOptions;
	}
}
