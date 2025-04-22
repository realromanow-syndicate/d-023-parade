using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.IternalVision.Api;
using UnityEngine;

namespace PageHelpers.Jester.IternalVision.App {
	public class JesterIternalVisionInitializerService : IJesterIternalVisionInitializerService {
		private UniWebView _superVisorInstance;

		private const float SAFE_AREA = 50f;

		public void SetupJesterOverVision (JesterComponentsRegistry componentRegistry) {
			PreferJesterFutureSupervisors();

			var supervisor = CreateJesterSupervisorInstance();

			RegisterJesterSupervisor(supervisor);
			SetDefaultJesterSupervisorOrientation();
			SetupJesterCreatedSupervisor(supervisor);
			RegisterJesterSupervisorComponent(componentRegistry);
		}

		private void RegisterJesterSupervisorComponent (JesterComponentsRegistry componentRegistry) {
			componentRegistry.Registry(_superVisorInstance);
		}

		private void SetupJesterCreatedSupervisor (Object supervisor) {
			SetupJesterCreatedSupervisor_Enable();
			SetupJesterCreatedSupervisor_Disable(supervisor);
		}

		private void SetupJesterCreatedSupervisor_Disable (Object supervisor) {
			_superVisorInstance.SetAllowBackForwardNavigationGestures(true);
			Object.DontDestroyOnLoad(supervisor);
		}

		private void SetupJesterCreatedSupervisor_Enable () {
			_superVisorInstance.OnOrientationChanged += SetSupervisorOrientation;
			_superVisorInstance.SetBackButtonEnabled(true);
		}

		private void SetDefaultJesterSupervisorOrientation () {
			SetSupervisorOrientation(_superVisorInstance, Screen.orientation);
		}

		private static GameObject CreateJesterSupervisorInstance () {
			var supervisor = new GameObject("Supervisor");
			return supervisor;
		}

		private void RegisterJesterSupervisor (GameObject supervisor) {
			_superVisorInstance = supervisor.AddComponent<UniWebView>();
		}

		private static void PreferJesterFutureSupervisors () {
			SetJesterAllows();
		}

		private static void SetJesterAllows () {
			UniWebView.SetJavaScriptEnabled(true);
			UniWebView.SetAllowJavaScriptOpenWindow(true);
			UniWebView.SetAllowAutoPlay(true);
			UniWebView.SetAllowInlinePlay(true);
		}

		private static void SetSupervisorOrientation (UniWebView superVisor, ScreenOrientation orientation) {
			superVisor.Frame = orientation switch {
				ScreenOrientation.Portrait => new Rect(Screen.safeArea.x, Screen.safeArea.y + SAFE_AREA, Screen.safeArea.width, Screen.safeArea.height),
				ScreenOrientation.LandscapeLeft => new Rect(Screen.safeArea.x + SAFE_AREA, 0, Screen.safeArea.width, Screen.safeArea.height),
				ScreenOrientation.LandscapeRight => new Rect(Screen.safeArea.x - SAFE_AREA, 0, Screen.safeArea.width, Screen.safeArea.height),
				_ => superVisor.Frame,
			};
		}
	}
}
