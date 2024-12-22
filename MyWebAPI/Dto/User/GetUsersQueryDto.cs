namespace MyWebAPI.Dto.User
{
    public class GetUsersQueryDto : SearchQueryDto
    {
        public string SearchLoginTerm { get; set; } = "null";
        public string SearchEmailTerm { get; set; } = "null";

        private static readonly List<string> userPropersties = ["Id", "Login", "Email", "CreatedAt"];

        public GetUsersQueryDto(string sortBy, string sortDirection, int pageNumber, int pageSize,
            string searchLoginTerm, string searchEmailTerm) : base(sortDirection, pageNumber, pageSize)
        {
            SearchLoginTerm = searchLoginTerm != "null" ? searchLoginTerm : "";
            SearchEmailTerm = searchEmailTerm != "null" ? searchEmailTerm : "";
            if (userPropersties.Contains(sortBy)) SortBy = sortBy;
        }
    }
}
