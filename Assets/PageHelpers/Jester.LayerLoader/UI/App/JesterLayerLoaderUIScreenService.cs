using PageHelpers.Jester.LayerLoader.UI.Api;
using PageHelpers.Jester.UI.Api;
using System;
using UnityEngine;

namespace PageHelpers.Jester.LayerLoader.UI.App {
	public class JesterLayerLoaderUIScreenService : IJesterLayerLoaderUIScreenService {
		private readonly Preferences _preferences;
		private readonly IJesterUIService _uiScreenService;
		private readonly Canvas _unitCanvas;

		public JesterLayerLoaderUIScreenService (Preferences preferences, IJesterUIService uiScreenService) {
			_preferences = preferences;
			_uiScreenService = uiScreenService;

			_unitCanvas = UnityEngine.Object.Instantiate(_preferences.unitCanvasReference);
		}

		public void ShowBlackoutScreen () {
			_uiScreenService.SetupJesterForm<object>(_preferences.blackoutScreenReference, _unitCanvas);
		}
		
		public void ShowLoadingScreen () {
			_uiScreenService.SetupJesterForm<object>(_preferences.loadingScreenReference, _unitCanvas);
		}

		public void HideLoadingScreen () {
			_uiScreenService.RemoveJesterForm(_preferences.loadingScreenReference, _unitCanvas);
		}

		[Serializable]
		public class Preferences {
			[SerializeField]
			private Canvas _unitCanvasReference;

			[SerializeField]
			private GameObject _loadingScreenReference;
			
			[SerializeField]
			private GameObject _blackoutScreenReference;

			public Canvas unitCanvasReference => _unitCanvasReference;
			public GameObject loadingScreenReference => _loadingScreenReference;
			public GameObject blackoutScreenReference => _blackoutScreenReference;
		}
	}
}
