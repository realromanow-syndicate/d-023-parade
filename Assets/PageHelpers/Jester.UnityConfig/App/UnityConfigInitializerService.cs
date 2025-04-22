using Cysharp.Threading.Tasks;
using PageHelpers.Jester.UnityConfig.Api;
using System;
using System.Collections.Generic;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;

namespace PageHelpers.Jester.UnityConfig.App {
	public class UnityConfigInitializerService : IUnityConfigInitializerService, IDisposable {
		public IReadOnlyReactiveProperty<IDictionary<string, object>> cachedRawData => _cachedRawData;
		public IReadOnlyReactiveProperty<bool> isCacheInit => _isCacheInit;
		
		private readonly ReactiveProperty<IDictionary<string, object>> _cachedRawData = new();
		private readonly ReactiveProperty<bool> _isCacheInit = new(false);
		
		private struct UserAttributes {}
		private struct AppAttributes {}

		private static async UniTask InitializeRemoteConfigAsync () {
			await UnityServices.InitializeAsync().AsUniTask();
			
			if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync().AsUniTask();
		}

		public async UniTask Launch () {
			if (Utilities.CheckForInternetConnection()) await InitializeRemoteConfigAsync();

			RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
			RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
		}

		private void ApplyRemoteSettings (ConfigResponse configResponse) {
			if (configResponse.requestOrigin == ConfigOrigin.Cached || configResponse.requestOrigin == ConfigOrigin.Default) {
				return;
			}

			_cachedRawData.Value = RemoteConfigService.Instance.appConfig.config.ToObject<Dictionary<string, object>>();
			_isCacheInit.Value = true;
		}

		private void ApplyCachedSettings () {
			_cachedRawData.Value = new Dictionary<string, object>();
			_isCacheInit.Value = true;
		}
		
		public void Dispose() {
			RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteSettings;
		}
	}
}
