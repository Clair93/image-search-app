namespace ImageSearch.Domain.ValueObjects
{
    public sealed record ImageSearchQuery(string Keyword, int Page, int PerPage);
}
