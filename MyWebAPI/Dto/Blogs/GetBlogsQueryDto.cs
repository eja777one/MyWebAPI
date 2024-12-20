namespace MyWebAPI.Dto.Blogs
{
    public class GetBlogsQueryDto : SearchQueryDto
    {
        public string SearchNameTerm { get; set; } = "null";

        private static readonly List<string> blogPropersties =
            ["Id", "Name", "Description", "WebsiteUrl", "CreatedAt", "IsMembership"];

        public GetBlogsQueryDto(string searchNameTerm, string sortBy, string sortDirection,
            int pageNumber, int pageSize) : base(sortDirection, pageNumber, pageSize)
        {
            SearchNameTerm = searchNameTerm != "null" ? searchNameTerm : "";
            if (blogPropersties.Contains(sortBy)) SortBy = sortBy;
        }
    }
}
