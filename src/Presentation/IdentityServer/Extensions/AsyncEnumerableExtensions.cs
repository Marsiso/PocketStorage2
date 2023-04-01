namespace IdentityServer.Extensions;

public static class AsyncEnumerableExtensions
{
    #region Public Methods

    public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        if (source == null)
        {
            const string errorMessage = $"LINQ extension method {nameof(ToListAsync)} cannot take first argument as null reference object.";
            throw new ArgumentNullException(nameof(source), errorMessage);
        }

        return ExecuteAsync();

        async Task<List<T>> ExecuteAsync()
        {
            var list = new List<T>();

            await foreach (var element in source)
            {
                list.Add(element);
            }

            return list;
        }
    }

    #endregion Public Methods
}