using PageHelpers.Jester.TrackingTransparency.UI.Api;
using PageHelpers.Jester.TrackingTransparency.UI.ViewModels;
using PageHelpers.Jester.UI.Api;
using System;
using UnityEngine;

namespace PageHelpers.Jester.TrackingTransparency.UI.App {
	public class TrackingTransparencyUIService : ITrackingTransparencyUIService {
		private readonly IJesterUIService _quickUIService;
		private readonly Preferences _preferences;
		private readonly Canvas _canvas;

		public TrackingTransparencyUIService (IJesterUIService quickUIService, Preferences preferences) {
			_quickUIService = quickUIService;
			_preferences = preferences;
			_canvas = UnityEngine.Object.Instantiate(_preferences.canvas);
		}

		public void ShowPermissionScreen (TrackingPermissionNotifyViewModel viewModel) {
			_quickUIService.SetupJesterForm(_preferences.trackingTransparencyScreen, _canvas, viewModel);
		}

		public void HidePermissionScreen () {
			_quickUIService.RemoveJesterForm(_preferences.trackingTransparencyScreen, _canvas);
		}

		[Serializable]
		public class Preferences {
			[SerializeField]
			private Canvas _canvas;

			[SerializeField]
			private GameObject _trackingTransparencyScreen;

			public Canvas canvas => _canvas;
			public GameObject trackingTransparencyScreen => _trackingTransparencyScreen;
		}
	}
}
