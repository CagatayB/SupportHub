using Microsoft.JSInterop;

namespace SupportHub.Client.Services
{
    public class BrowserStorageService
    {
        private readonly IJSRuntime _js;

        public BrowserStorageService(IJSRuntime js) => _js = js;

        public async Task SetTokenAsync(string token)
            => await _js.InvokeVoidAsync("localStorage.setItem", "authToken", token);

        public async Task<string?> GetTokenAsync()
            => await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");

        public async Task RemoveTokenAsync()
            => await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
    }
}
