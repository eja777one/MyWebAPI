namespace MyWebApiTests
{
    [CollectionDefinition("Videos")]
    public class VideosTestCollection : ICollectionFixture<CustomWebApplicationFactory>;

    [CollectionDefinition("Blogs")]
    public class BlogsTestCollection : ICollectionFixture<CustomWebApplicationFactory>;

    [CollectionDefinition("Posts")]
    public class PostsTestCollection : ICollectionFixture<CustomWebApplicationFactory>;

    [CollectionDefinition("Users")]
    public class UsersTestCollection : ICollectionFixture<CustomWebApplicationFactory>;

    [CollectionDefinition("Auth")]
    public class AuthTestCollection : ICollectionFixture<CustomWebApplicationFactory>;
}
