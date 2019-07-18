namespace ReflectionTests.Domain
{
	public class BookFactory : IBookFactory
	{
		public IBook CreateBook(string title, int pages) => new Book
		{
			Title = title,
			Pages = pages
		};
    }
}
