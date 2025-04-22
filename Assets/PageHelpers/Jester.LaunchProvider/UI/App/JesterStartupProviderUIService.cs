using PageHelpers.Jester.LaunchProvider.UI.Api;
using PageHelpers.Jester.UI.Api;
using System;
using UnityEngine;

namespace PageHelpers.Jester.LaunchProvider.UI.App {
	public class JesterStartupProviderUIService : IJesterStartupProviderUIService {
		private readonly IJesterUIService _uiService;
		private readonly Preferences _preferences;
		private readonly Canvas _canvas;

		public JesterStartupProviderUIService (IJesterUIService uiService, Preferences preferences) {
			_uiService = uiService;
			_preferences = preferences;

			_canvas = UnityEngine.Object.Instantiate(_preferences.internetScreenCanvas);
		}

		public void ShowLowInternetScreen () {
			_uiService.SetupJesterForm<object>(_preferences.internetScreen, _canvas, null);
		}

		[Serializable]
		public class Preferences {
			[SerializeField]
			private GameObject _internetScreen;

			[SerializeField]
			private Canvas _internetScreenCanvas;

			public GameObject internetScreen => _internetScreen;
			public Canvas internetScreenCanvas => _internetScreenCanvas;
		}
	}
}
