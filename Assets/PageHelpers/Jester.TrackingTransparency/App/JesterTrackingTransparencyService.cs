using Cysharp.Threading.Tasks;
using PageHelpers.Jester.TrackingTransparency.Api;
using PageHelpers.Jester.TrackingTransparency.UI.Api;
using PageHelpers.Jester.TrackingTransparency.UI.ViewModels;
using System;
using System.Threading;
using UniRx;
using Unity.Advertisement.IosSupport;

namespace PageHelpers.Jester.TrackingTransparency.App {
	public class JesterTrackingTransparencyService : IJesterTrackingTransparencyService, IDisposable {
		private readonly ITrackingTransparencyUIService _uiService;
		private readonly CompositeDisposable _compositeDisposable = new();

		public JesterTrackingTransparencyService (ITrackingTransparencyUIService uiService) {
			_uiService = uiService;
		}

		public bool hasTrackingTransparencyPermission =>
			ATTrackingStatusBinding.GetAuthorizationTrackingStatus()
			== ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED;

		public async UniTask RequestTrackingTransparencyPermission (CancellationToken cancellationToken) {
			if (AuthorizationStatusIsNotDetermined()) return;

			var trackingViewModel = new TrackingPermissionNotifyViewModel().AddTo(_compositeDisposable);
			trackingViewModel.read
				.Subscribe(_ => RequestNativePermission())
				.AddTo(_compositeDisposable);
			
			_uiService.ShowPermissionScreen(trackingViewModel);

			await RequestPermissionStatus(cancellationToken);
		}

		private void RequestNativePermission () {
			_uiService.HidePermissionScreen();
			RequestAuthorizationTracking();
		}

		private static bool AuthorizationStatusIsNotDetermined () {
			return ATTrackingStatusBinding.GetAuthorizationTrackingStatus()
				!= ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED;
		}

		private static void RequestAuthorizationTracking () {
			ATTrackingStatusBinding.RequestAuthorizationTracking();
		}

		private static async UniTask RequestPermissionStatus (CancellationToken cancellationToken) {
			while (!cancellationToken.IsCancellationRequested) {
				if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus()
					!= ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
					return;

				await UniTask.Yield();
			}
		}

		public void Dispose() {
			_compositeDisposable?.Dispose();
		}
	}
}
