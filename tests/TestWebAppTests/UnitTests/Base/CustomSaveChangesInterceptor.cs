using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TestWebAppTests.UnitTests.Base
{
    public class CustomSaveChangesInterceptor(int forceResult) : SaveChangesInterceptor
    {
        public override ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            return new(forceResult);
        }
    }
}
