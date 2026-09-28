using System.Collections.Generic;

namespace ImageSearch.Domain.Models
{
    public sealed record ImageItem(int Id, string ThumbnailUrl, IReadOnlyList<string> Tags, string AuthorName);
}
