using MyWebAPI.Models.Posts;
using Xunit.Abstractions;

namespace MyWebAPI.Dto
{
    public class PaginatorDto<T>
    {
        public int PagesCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; } = new();

        public PaginatorDto() { }
        public PaginatorDto(List<T> items, SearchQueryDto dto)
        {
            var skip = (dto.PageNumber - 1) * dto.PageSize;

            PagesCount = PaginatorDto<T>.GetPagesTotalCount(items.Count, dto.PageSize);
            Page = dto.PageNumber > PagesCount ? 0 : dto.PageNumber;
            PageSize = dto.PageSize;
            TotalCount = items.Count;
            Items = items.Skip(skip).Take(dto.PageSize).ToList();
        }

        public static int GetPagesTotalCount(int count, int pageSize)
        {
            var totalCount = Math.Ceiling((float)count / (float)pageSize);
            return Convert.ToInt32(totalCount);
        }
    }
}
