using PageHelpers.Jester.CoreLauncher.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PageHelpers.Jester.CoreLauncher.App {
	public class JesterCoreLauncherService : IJesterCoreLauncherService {
		public void LoadJesterCore () {
			PrepareJesterScreenRotation();

			SceneManager.LoadScene("Menu");
		}

		private static void PrepareJesterScreenRotation () {
			Screen.autorotateToLandscapeRight = false;
			Screen.autorotateToPortraitUpsideDown = false;
			Screen.autorotateToPortrait = true;
			Screen.autorotateToLandscapeLeft = false;
			
			Screen.orientation = ScreenOrientation.Portrait;
		}
	}
}
