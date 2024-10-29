using CodeBRDG_TT.Data;
using CodeBRDG_TT.Data.UnitOfWork;
using CodeBRDG_TT.Models;
using CodeBRDG_TT.Queries;
using CodeBRDG_TT.Queries.Handlers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CodeBRDG_TT.Tests.Queries;

public class DogsQueryHandlerTests
{
    private readonly DogsQueryHandler _handler;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly AppDbContext _context;

    public DogsQueryHandlerTests()
    {
        // Set up in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "DogDatabase")
            .Options;

        _context = new AppDbContext(options);
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUnitOfWork.Setup(u => u.Dogs.GetAll()).Returns(_context.Set<Dog>().AsQueryable());
        _handler = new DogsQueryHandler(_mockUnitOfWork.Object);

        // Seed the database with default dogs
        DatabaseSeeder.SeedDefaultDogs(_context);
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldOrderDogs()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "asc",
            pageNumber = 1,
            pageSize = 30
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(5);
        result.First().name.Should().Be("Rex"); 
        result.Last().name.Should().Be("Buddy"); 
    }

    [Fact]
    public async Task Handle_ValidQueryDescending_ShouldOrderDogsDescending()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "desc",
            pageNumber = 1,
            pageSize = 5
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(5);
        result.First().name.Should().Be("Buddy"); 
        result.Last().name.Should().Be("Rex");
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldReturnDogsInColorOrder()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "color",
            order = "asc",
            pageNumber = 1,
            pageSize = 5
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(5);
        result.First().name.Should().Be("Daisy"); // Alphabetically first
        result.Last().name.Should().Be("Rex"); // Alphabetically last 
    }

    [Fact]
    public async Task Handle_EmptyQuery_ShouldReturnNoDogs()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "asc",
            pageNumber = 2, // pageNumber > total pages available
            pageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty(); 
    }

    [Fact]
    public async Task Handle_InvalidAttribute_ShouldThrowException()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "invalidAttribute", // Invalid attribute
            order = "asc",
            pageNumber = 1,
            pageSize = 3
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>(); 
    }

    [Fact]
    public async Task Handle_ValidQueryWithColorAndPagination_ShouldFilterDogsByColor()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "color",
            order = "asc",
            pageNumber = 1,
            pageSize = 1
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().name.Should().Be("Daisy");
    }

    [Fact]
    public async Task Handle_ValidQueryWithWithPaginationAndWeight_ShouldReturnNoDogs()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "asc",
            pageNumber = 1,
            pageSize = 2
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Last().name.Should().Be("Daisy"); 
    }

    [Fact]
    public async Task Handle_ValidQuery_WithEmptyPageSize_ShouldThrowException()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "asc",
            pageNumber = 1,
            pageSize = 0 // Invalid page size
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>(); // Expecting an exception
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldHandleMultipleColors()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "color",
            order = "asc",
            pageNumber = 1,
            pageSize = 5
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(5);
        result.First().name.Should().Be("Daisy"); // First in alphabetical order
        result.Last().name.Should().Be("Rex"); // Last in alphabetical order
    }

    [Fact]
    public async Task Handle_ValidQuery_WithNegativePageNumber_ShouldThrowArgumentException()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "asc",
            pageNumber = -1, // Invalid page number
            pageSize = 5
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>(); // Expecting an exception
    }

    [Fact]
    public async Task Handle_ValidQuery_WithPageSizeGreaterThanTotal_ShouldReturnAllDogs()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "asc",
            pageNumber = 1,
            pageSize = 100 // Larger than total dogs
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(5); // Expect all dogs returned
    }

    [Fact]
    public async Task Handle_ValidQuery_WithInvalidPageSize_ShouldThrowArgumentException()
    {
        // Arrange
        var query = new DogsQuery
        {
            attribute = "weight",
            order = "asc",
            pageNumber = 1,
            pageSize = -1 // Invalid page size
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>(); // Expecting an exception
    }

}
