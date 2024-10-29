using CodeBRDG_TT.Data.UnitOfWork;
using CodeBRDG_TT.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CodeBRDG_TT.Queries.Handlers;

public class DogsQueryHandler : IRequestHandler<DogsQuery, List<Dog>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DogsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Dog>> Handle(DogsQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(request.PageNumber), "Page number must be greater than or equal to 1.");
        }

        if (request.PageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.PageSize), "Page size must be greater than 0.");
        }

        IQueryable<Dog> query = _unitOfWork.Dogs.GetAll();

        var propertyInfo = typeof(Dog).GetProperty(request.Attribute,
            System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (propertyInfo != null)
        {
            var parameter = Expression.Parameter(typeof(Dog), "d");
            var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);

            if (!propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
            {
                var isNotNull = Expression.NotEqual(propertyAccess, Expression.Constant(null));
                var lambda = Expression.Lambda<Func<Dog, bool>>(isNotNull, parameter);
                query = query.Where(lambda);
            }
        }

        if (request.Order?.ToLower() == "desc")
        {
            query = query.OrderByDescending(d => EF.Property<object>(d, request.Attribute));
        }
        else
        {
            query = query.OrderBy(d => EF.Property<object>(d, request.Attribute));
        }

        var dogs = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return dogs;
    }
}
