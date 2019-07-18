using NUnit.Framework;
using ReflectionTests.Domain;

namespace ReflectionTests
{
	[TestFixture]
    public class MapTests : BaseMapTests<BookMap, BookDto, IBook, IBookFactory>
    {
	    protected override BookDto Source { get; } = new BookDto
	    {
		    Title = "Book title",
		    Pages = 987
	    };

	    protected override object[] FactoryParameters =>
	    new object[] {
			Source.Title,
			Source.Pages
	    };

	    protected override string FactoryMethodName { get; } = nameof(IBookFactory.CreateBook);
    }
}