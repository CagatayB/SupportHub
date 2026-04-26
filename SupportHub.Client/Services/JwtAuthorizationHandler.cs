namespace SupportHub.Client.Services
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly BrowserStorageService _storage;

        public JwtAuthorizationHandler(BrowserStorageService storage) => _storage = storage;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _storage.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
