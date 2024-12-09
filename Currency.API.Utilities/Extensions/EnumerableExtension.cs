
namespace Currency.API.Utilities.Extensions
{
	public static class EnumerableExtension
	{
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			foreach (T item in source)
			{
				action(item);
			}

			return source;
		}
	}
}
