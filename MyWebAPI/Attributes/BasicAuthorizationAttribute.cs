using Microsoft.AspNetCore.Authorization;

namespace MyWebAPI.Attributes
{
    public class BasicAuthorizationAttribute : AuthorizeAttribute
    {
        public BasicAuthorizationAttribute()
        {
            AuthenticationSchemes = "Basic";
        }
    }
}
