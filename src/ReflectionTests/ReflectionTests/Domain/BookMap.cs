using Mapster;

namespace ReflectionTests.Domain
{
    public class BookMap : IMap
    {
	    private readonly IBookFactory _bookFactory;

	    public BookMap(IBookFactory bookFactory)
	    {
		    _bookFactory = bookFactory;
	    }

	    public void Register()
	    {
		    TypeAdapterConfig<BookDto, IBook>
			    .NewConfig()
			    .MapWith(src => _bookFactory.CreateBook(src.Title, src.Pages));
	    }
    }
}
