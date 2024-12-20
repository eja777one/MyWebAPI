namespace MyWebAPI.Dto.Posts
{
    public class GetPostsQueryDto : SearchQueryDto
    {
        private static readonly List<string> postPropersties =
            ["Id", "Title", "ShortDescription", "Content", "BlogId", "BlogName", "CreatedAt"];

        public GetPostsQueryDto(string sortBy, string sortDirection, int pageNumber, int pageSize)
            : base(sortDirection, pageNumber, pageSize)
        {
            if (postPropersties.Contains(sortBy)) SortBy = sortBy;
        }
    }
}
