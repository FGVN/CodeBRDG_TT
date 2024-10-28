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
        IQueryable<Dog> query = _unitOfWork.Dogs.GetAll();

        var propertyInfo = typeof(Dog).GetProperty(request.attribute,
            System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (propertyInfo != null)
        {
            var parameter = Expression.Parameter(typeof(Dog), "d");
            var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);

            // Check if the property type is nullable
            //if (propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(propertyInfo.PropertyType) == null)
            if (!propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
            {
                var isNotNull = Expression.NotEqual(propertyAccess, Expression.Constant(null));
                var lambda = Expression.Lambda<Func<Dog, bool>>(isNotNull, parameter);
                query = query.Where(lambda);
            }
        }

        // Apply ordering
        if (request.order?.ToLower() == "desc")
        {
            query = query.OrderByDescending(d => EF.Property<object>(d, request.attribute));
        }
        else
        {
            query = query.OrderBy(d => EF.Property<object>(d, request.attribute));
        }

        // Apply pagination
        var dogs = await query
            .Skip((request.pageNumber - 1) * request.pageSize)
            .Take(request.pageSize)
            .ToListAsync(cancellationToken);

        return dogs;
    }
}
