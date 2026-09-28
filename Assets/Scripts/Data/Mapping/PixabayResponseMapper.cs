using System;
using System.Collections.Generic;
using System.Linq;
using ImageSearch.Data.Dtos;
using ImageSearch.Domain.Models;

namespace ImageSearch.Data.Mapping
{
    public static class PixabayResponseMapper
    {
        public static ImageItem ToModel(PixabayImageDto dto)
        {
            return new ImageItem(dto.Id, dto.WebformatUrl, ParseTags(dto.Tags), dto.User);
        }

        public static IReadOnlyList<ImageItem> ToModels(IReadOnlyList<PixabayImageDto> dtos)
        {
            return dtos.Select(ToModel).ToList();
        }

        private static IReadOnlyList<string> ParseTags(string rawTags)
        {
            if (string.IsNullOrWhiteSpace(rawTags))
            {
                return Array.Empty<string>();
            }

            return rawTags
                .Split(',')
                .Select(tag => tag.Trim())
                .Where(tag => tag.Length > 0)
                .ToList();
        }
    }
}
