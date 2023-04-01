namespace IdentityServer.Extensions;

public static class AsyncEnumerableExtensions
{
    #region Public Methods

    public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        if (source == null)
        {
            string errorMessage = $"[{nameof(AsyncEnumerableExtensions)}] Null reference exception. Parameter: '{nameof(source)}' Value: '{null}'";
            throw new ArgumentNullException(nameof(source), errorMessage);
        }

        return ExecuteAsync();

        async Task<List<T>> ExecuteAsync()
        {
            List<T> list = new List<T>();

            await foreach (var element in source)
            {
                list.Add(element);
            }

            return list;
        }
    }

    #endregion Public Methods
}