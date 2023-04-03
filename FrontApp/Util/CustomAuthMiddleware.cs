using System.Net;

namespace FrontApp.Util
{
    // add custom middleware that validate authorization bearer header
    public class CustomAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthMiddleware(RequestDelegate next) => _next = next;

        // create InvokeAsync method that validate authorization bearer header
        public async Task InvokeAsync(HttpContext context)
        {
            // get authorization header
            string authHeader = context.Request.Headers["Authorization"];
            // check if authorization header is not null
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer"))
            {
                // get authorization header value
                var authHeaderValue = authHeader.ToString();
                // check if authorization header value is not null
                if (!string.IsNullOrEmpty(authHeaderValue))
                {
                    // do nothing
                }
                else
                {
                    // return 401 response
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
            }
            // call next middleware
            await _next(context);
        }


    }
}
