using Cysharp.Threading.Tasks;
using PageHelpers.Jester.LaunchProvider.Api;
using PageHelpers.Jester.LaunchProvider.UI.Api;
using PageHelpers.Jester.StartupProvider.Api;
using PageHelpers.Jester.TrackingTransparency.Api;
using PageHelpers.Jester.UnityConfig.Api;
using System;
using System.Threading;
using UniRx;
using UnityEngine;

namespace PageHelpers.Jester.LaunchProvider.App {
	public class JesterLaunchProvider : IJesterLaunchProvider, IDisposable {
		private readonly IJesterStartupProvider _jesterStartupProvider;
		private readonly IUnityConfigListenerService _configListener;
		private readonly IJesterStartupProviderUIService _startupProviderUIService;
		private readonly IJesterTrackingTransparencyService _quickTrackingTransparencyService;

		private readonly CompositeDisposable _compositeDisposable = new();

		public JesterLaunchProvider (
			IJesterStartupProvider jesterStartupProvider,
			IJesterTrackingTransparencyService quickTrackingTransparencyService, IUnityConfigListenerService configListener, IJesterStartupProviderUIService startupProviderUIService) {
			_jesterStartupProvider = jesterStartupProvider;
			_quickTrackingTransparencyService = quickTrackingTransparencyService;
			_configListener = configListener;
			_startupProviderUIService = startupProviderUIService;
		}

		public async UniTask LaunchJesterMetaLayer (CancellationToken cancellationToken) {
			Debug.developerConsoleEnabled = false;
			
			await RequestTransparency(cancellationToken);

			Application.targetFrameRate = 60;
			
			_configListener.isRestrictionCompleted
				.Take(1)
				.Subscribe(LoadPreferences)
				.AddTo(_compositeDisposable);
			
			_configListener.StartParseConfig();
			
			await UniTask.Delay(20000, cancellationToken: cancellationToken);

			if (!cancellationToken.IsCancellationRequested) {
				_startupProviderUIService.ShowLowInternetScreen();
			}
		}

		private void LoadPreferences (bool isCacheEnable) {
			if (isCacheEnable) _jesterStartupProvider.StartupApp();
			else StartupGameplayLayer();
		}

		private void StartupGameplayLayer () {
			_jesterStartupProvider.StartGameplay();
		}

		private async UniTask RequestTransparency (CancellationToken cancellationToken) {
			if (!_quickTrackingTransparencyService.hasTrackingTransparencyPermission) await _quickTrackingTransparencyService.RequestTrackingTransparencyPermission(cancellationToken);
		}

		public void Dispose () {
			_compositeDisposable.Dispose();
		}
	}
}
