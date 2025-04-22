using Cysharp.Threading.Tasks;
using PageHelpers.Jester.Analytics.Api;
using PageHelpers.Jester.CloudMessages.Api;
using PageHelpers.Jester.CoreLauncher.Api;
using PageHelpers.Jester.IternalVision.Api;
using PageHelpers.Jester.LayerLauncher.Api;
using PageHelpers.Jester.LayerLoader.UI.Api;
using PageHelpers.Jester.Storage.App;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace PageHelpers.Jester.LayerLauncher.App {
	public class JesterLauncherService : IJesterLauncherService, IDisposable {
		private const string USER_CHARGE_CAN_START_APP_KEY = "user_can_start_app";
		private const string URL_KEY = "url";

		private readonly IJesterApiService _api;
		private readonly IJesterLayerLoaderUIScreenService _JesterLayerLoaderScreenService;
		private readonly IJesterCloudMessagesListener _quickCloudMessagesListener;
		private readonly IJesterCloudMessagesService _quickCloudMessagesService;
		private readonly IJesterCollectorService _JesterCollectorService;
		private readonly IJesterCoreLauncherService _JesterCoreLauncherService;
		private readonly IJesterAnalyticsListener _quickAnalyticsListener;
		private readonly IJesterIternalVisionService _JesterIternalVisionService;
		private readonly CompositeDisposable _compositeDisposable = new();
		private readonly JesterAppStorage _JesterAppStorage;

		private readonly ReactiveProperty<bool> _isNotificationsInit = new(false);
		private readonly ReactiveProperty<bool> _isWViewInitFromPush = new(false);
		private readonly ReactiveProperty<int> _conversionDataGeneration = new(0);

		private Dictionary<string, object> _cachedConversionData = new();

		public JesterLauncherService (
			IJesterApiService api,
			IJesterLayerLoaderUIScreenService JesterLayerLoaderScreenService,
			IJesterCloudMessagesListener quickCloudMessagesListener,
			IJesterCloudMessagesService quickCloudMessagesService,
			IJesterCollectorService JesterCollectorService,
			IJesterCoreLauncherService JesterCoreLauncherService,
			IJesterAnalyticsListener quickAnalyticsListener,
			IJesterIternalVisionService JesterIternalVisionService,
			JesterAppStorage JesterAppStorage) {
			_api = api;
			_JesterLayerLoaderScreenService = JesterLayerLoaderScreenService;
			_quickCloudMessagesListener = quickCloudMessagesListener;
			_quickCloudMessagesService = quickCloudMessagesService;
			_JesterCollectorService = JesterCollectorService;
			_JesterCoreLauncherService = JesterCoreLauncherService;
			_quickAnalyticsListener = quickAnalyticsListener;
			_JesterIternalVisionService = JesterIternalVisionService;
			_JesterAppStorage = JesterAppStorage;
		}

		public async UniTask LaunchJesterMetaAsync (CancellationToken cancellationToken) {
			SetupSupervisor();
			SetupLoadingScreen();

			if (GetStartGameStatus()) {
				StartGame();

				return;
			}

			if (_JesterAppStorage.TryExtractJesterStorageEntity(URL_KEY, out var url)) {
				await SetupCloudMessages();

				_JesterIternalVisionService.LoadJesterVision(url.ToString());

				return;
			}

			SetupTokenChanges(cancellationToken);
			SetupConversions(cancellationToken);
		}

		private void SetupConversions (CancellationToken cancellationToken) {
			_quickAnalyticsListener.dataSuccessReceived
				.Subscribe(conversionCache => {
					if (_conversionDataGeneration.Value++ > 0) return;

					OnConversionDataReceived(conversionCache, cancellationToken).Forget(Debug.LogException);
				})
				.AddTo(_compositeDisposable);
			
			_quickAnalyticsListener.dataFailReceived
				.Subscribe(_ => OnConversionDataFailed())
				.AddTo(_compositeDisposable);
		}

		private void SetupTokenChanges (CancellationToken cancellationToken) {
			_quickCloudMessagesListener.onTokenUpdate
				.Subscribe(token => {
					if (string.IsNullOrEmpty(token)) return;

					SendAppParams(_cachedConversionData, cancellationToken);
				})
				.AddTo(_compositeDisposable);
		}

		private static bool GetStartGameStatus () {
			return PlayerPrefs.GetInt(USER_CHARGE_CAN_START_APP_KEY, 0) == 1;
		}

		private void SetupLoadingScreen () {
			_JesterLayerLoaderScreenService.ShowLoadingScreen();
		}

		private void StartGame () {
			_JesterCoreLauncherService.LoadJesterCore();
			_JesterLayerLoaderScreenService.HideLoadingScreen();
		}

		private async UniTask SetupCloudMessages () {
			if (_isNotificationsInit.Value)
				return;

			_isNotificationsInit.Value = true;

			if (_quickCloudMessagesService.hasMessagesPermissionStatus.Value) {
				_quickCloudMessagesListener.SubscribeOnReceivedCloudMessage("url", url => {
					_JesterIternalVisionService.LoadJesterVision(url);
					_isWViewInitFromPush.Value = true;
				});
				
				_quickCloudMessagesListener.StartCloudMessaging();
			}
			
			_quickCloudMessagesService.onPermissionStatusUpdated
				.Subscribe(status => {
					if (!status) return;

					_quickCloudMessagesListener.SubscribeOnReceivedCloudMessage("url", url => {
						_JesterIternalVisionService.LoadJesterVision(url);
						_isWViewInitFromPush.Value = true;
					});
				
					_quickCloudMessagesListener.StartCloudMessaging();
				})
				.AddTo(_compositeDisposable);

			if (_quickCloudMessagesService.timeMessagesAskPermissionExpiredStatus.Value
				&& !_quickCloudMessagesService.hasMessagesPermissionStatus.Value)
				await _quickCloudMessagesService.RequestQuickMessagesPermission();
		}

		private async UniTask StartupSupervisor () {
			if (_JesterIternalVisionService.visionGeneration.Value > 1) return;

			Screen.autorotateToPortrait = true;
			Screen.autorotateToLandscapeLeft = true;
			Screen.autorotateToLandscapeRight = true;
			Screen.autorotateToPortraitUpsideDown = true;

			Screen.orientation = ScreenOrientation.AutoRotation;
			
			_JesterLayerLoaderScreenService.HideLoadingScreen();
			_JesterLayerLoaderScreenService.ShowBlackoutScreen();
				
			await UniTask.Delay(100);
				
			_JesterIternalVisionService.LaunchJesterVision();
		}
		
		private void SetupSupervisor () {
			_JesterIternalVisionService.jesterVisionLoaded
				.Subscribe(_ => {
					StartupSupervisor().Forget(Debug.LogException);
				})
				.AddTo(_compositeDisposable);
		}

		private async UniTask OnConversionDataReceived (Dictionary<string, object> conversionData, CancellationToken cancellationToken) {
			_cachedConversionData = conversionData;

			var viewParams = await SendAppParams(conversionData, cancellationToken);

			await LaunchIntermediateLayer(viewParams, cancellationToken);
		}

		private UniTask<Dictionary<string, object>> SendAppParams (Dictionary<string, object> conversionData, CancellationToken cancellationToken) {
			var paramsDataCollection = new Dictionary<string, object>();

			FillConv(conversionData, paramsDataCollection);

			var paramsData = _JesterCollectorService
				.GetJesterConfigurationParameters();

			FillParams(paramsData, paramsDataCollection);

			return _api.GetJesterConfigAsync(paramsDataCollection, cancellationToken);
		}

		private static void FillConv (Dictionary<string, object> conversionData, Dictionary<string, object> paramsDataCollection) {
			foreach (var convPair in conversionData) {
				paramsDataCollection.Add(convPair.Key, convPair.Value);
			}
		}

		private static void FillParams (Dictionary<string, object> paramsData, Dictionary<string, object> paramsDataCollection) {
			foreach (var param in paramsData) {
				paramsDataCollection.Add(param.Key, param.Value);
			}
		}

		private void OnConversionDataFailed () {
			_JesterCoreLauncherService.LoadJesterCore();
		}

		private async UniTask LaunchIntermediateLayer (IReadOnlyDictionary<string, object> appParams, CancellationToken cancellationToken) {
			var layerStatus = (bool)appParams["ok"];

			if (layerStatus) {
				await SetupCloudMessages();

				var url = (string)appParams["url"];
				var expirationStamp = (int)(long)appParams["expires"];

				InitInterLayer(url, expirationStamp);
			}
			else {
				InitGame();
			}
		}

		private void InitGame () {
			PlayerPrefs.SetInt(USER_CHARGE_CAN_START_APP_KEY, 1);
			StartGame();
		}

		private void InitInterLayer (string url, int expirationStamp) {
			_JesterAppStorage.AddJesterStorageEntity(URL_KEY, url, expirationStamp);

			if (!_isWViewInitFromPush.Value)
				_JesterIternalVisionService.LoadJesterVision(url);
		}

		public void Dispose() {
			_compositeDisposable.Dispose();
			_isNotificationsInit.Dispose();
			_isWViewInitFromPush.Dispose();
			_conversionDataGeneration.Dispose();
		}
	}
}
