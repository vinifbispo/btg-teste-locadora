using System.Linq.Expressions;
using Locadora.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Locadora.Application.UnitTest.TestHelpers;

internal sealed class TestAsyncEnumerator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
{
    public T Current => inner.Current;

    public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(inner.MoveNext());

    public ValueTask DisposeAsync()
    {
        inner.Dispose();
        return ValueTask.CompletedTask;
    }
}

internal sealed class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
{
    public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(StripEfOnlyCalls(expression));

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(StripEfOnlyCalls(expression));

    public object? Execute(Expression expression) => inner.Execute(StripEfOnlyCalls(expression));

    public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(StripEfOnlyCalls(expression));

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var expectedResultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethods()
            .First(m => m.Name == nameof(IQueryProvider.Execute) && m.IsGenericMethod)
            .MakeGenericMethod(expectedResultType)
            .Invoke(this, [StripEfOnlyCalls(expression)]);

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(expectedResultType)
            .Invoke(null, [executionResult])!;
    }

    private static Expression StripEfOnlyCalls(Expression expression) => new EfMethodStripperVisitor().Visit(expression);

    private sealed class EfMethodStripperVisitor : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
            => node.Method.DeclaringType == typeof(EntityFrameworkQueryableExtensions)
                ? Visit(node.Arguments[0])
                : base.VisitMethodCall(node);
    }
}

internal sealed class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
    {
    }

    public TestAsyncEnumerable(Expression expression) : base(expression)
    {
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
}

internal static class MockDbSetFactory
{
    public static Mock<DbSet<T>> Create<T>(List<T> data) where T : Entity
    {
        var queryable = new TestAsyncEnumerable<T>(data);
        var queryableInterface = (IQueryable)queryable;
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => new TestAsyncEnumerator<T>(data.GetEnumerator()));

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableInterface.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableInterface.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableInterface.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

        mockSet.Setup(m => m.Add(It.IsAny<T>()))
            .Callback<T>(item => data.Add(item));

        mockSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<T>>()))
            .Callback<IEnumerable<T>>(items => data.AddRange(items));

        mockSet.Setup(m => m.Remove(It.IsAny<T>()))
            .Callback<T>(item => data.Remove(item));

        mockSet.Setup(m => m.Update(It.IsAny<T>()));

        mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
            .Returns<object[]>(ids => new ValueTask<T?>(data.FirstOrDefault(e => e.Id.Equals(ids[0]))));

        return mockSet;
    }
}
