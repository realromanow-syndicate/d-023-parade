using PageHelpers.Jester.CloudMessages.UI.Api;
using PageHelpers.Jester.CloudMessages.UI.ViewModels;
using PageHelpers.Jester.UI.Api;
using System;
using UnityEngine;

namespace PageHelpers.Jester.CloudMessages.UI.App {
	public class JesterCloudMessagesUIScreenService : IJesterCloudMessagesUIScreenService {
		private readonly Preferences _preferences;
		private readonly IJesterUIService _uiScreenService;

		private readonly Canvas _notificationsCanvas;

		public JesterCloudMessagesUIScreenService (Preferences preferences, IJesterUIService uiScreenService) {
			_preferences = preferences;
			_uiScreenService = uiScreenService;

			_notificationsCanvas = UnityEngine.Object.Instantiate(preferences.unitCanvasReference);
		}

		public void RegisterJesterPermissionScreen (JesterMessagesPermissionScreenViewModel viewModel) {
			_uiScreenService.SetupJesterForm(_preferences.permissionScreenReference, _notificationsCanvas, viewModel);
		}

		public void RemoveJesterPermissionScreen () {
			_uiScreenService.RemoveJesterForm(_preferences.permissionScreenReference, _notificationsCanvas);
		}

		[Serializable]
		public class Preferences {
			[SerializeField]
			private Canvas _unitCanvasReference;

			[SerializeField]
			private GameObject _permissionScreenReference;

			public Canvas unitCanvasReference => _unitCanvasReference;
			public GameObject permissionScreenReference => _permissionScreenReference;
		}
	}
}
