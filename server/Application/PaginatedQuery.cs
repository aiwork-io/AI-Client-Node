using System;
using MediatR;

namespace Application
{
    public abstract class PaginatedQuery<T> : IRequest<T>
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        // convert from zero-based to 1-based page
        // Math.Max is to make sure it'll never be negative
        public int Skip => Math.Max(Page - 1, 0) * PageSize;

        public int Take => PageSize;
    }
}