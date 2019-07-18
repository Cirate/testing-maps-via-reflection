namespace ReflectionTests.Domain
{
	public interface IBookFactory
	{
		IBook CreateBook(string title, int pages);
	}
}