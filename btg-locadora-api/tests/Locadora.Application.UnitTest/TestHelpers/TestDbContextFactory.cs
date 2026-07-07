using Locadora.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Application.UnitTest.TestHelpers;

internal static class TestDbContextFactory
{
    public static Mock<LocadoraDbContext> Create()
    {
        var options = new DbContextOptions<LocadoraDbContext>();
        var mockContext = new Mock<LocadoraDbContext>(options);

        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

        return mockContext;
    }
}
