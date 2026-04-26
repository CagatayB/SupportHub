using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using SupportHub.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace SupportHub.Client.Auth
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
		private readonly BrowserStorageService _storage;
		private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

		public CustomAuthStateProvider(BrowserStorageService storage) => _storage = storage;

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await _storage.GetTokenAsync();
			if (string.IsNullOrWhiteSpace(token))
				return new AuthenticationState(_anonymous);

			return BuildState(token);
		}

		public void NotifyUserLogin(string token)
		{
			var authState = Task.FromResult(BuildState(token));
			NotifyAuthenticationStateChanged(authState);
		}

		public void NotifyUserLogout()
		{
			var authState = Task.FromResult(new AuthenticationState(_anonymous));
			NotifyAuthenticationStateChanged(authState);
		}

		private AuthenticationState BuildState(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(token);
			var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
			return new AuthenticationState(new ClaimsPrincipal(identity));
		}
	}
}
