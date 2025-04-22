using PageHelpers.Jester.StartupProvider.Api;
using UnityEngine.SceneManagement;

namespace PageHelpers.Jester.StartupProvider.App {
	public class JesterStartupProvider : IJesterStartupProvider {
		public void StartupApp () {
			SceneManager.LoadScene("JesterMetaScene");
		}
 
		public void StartGameplay () {
			SceneManager.LoadScene("Menu");
		}
	}
}
