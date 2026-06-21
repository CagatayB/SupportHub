using Microsoft.AspNetCore.Components;
using System.Net;

namespace SupportHub.Client.Services
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly BrowserStorageService _storage;
        private readonly NavigationManager _nav;

        public JwtAuthorizationHandler(BrowserStorageService storage, NavigationManager nav)
        {
            _storage = storage;
            _nav = nav;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _storage.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _storage.RemoveTokenAsync();
            _nav.NavigateTo("/login?expired=true");
        }

            return response;
        }
    }
}
