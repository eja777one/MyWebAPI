namespace MyWebAPI.Dto
{
    public class SearchQueryDto
    {
        public string SortBy { get; set; } = "createdAt";
        public string SortDirection { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public SearchQueryDto()
        {

        }

        public SearchQueryDto(string sortDirection, int pageNumber, int pageSize)
        {
            if (sortDirection == "asc" || sortDirection == "desc") SortDirection = sortDirection;
            if (pageNumber > 0) PageNumber = pageNumber;
            if (pageSize > 0) PageSize = pageSize;
        }
    }
}
