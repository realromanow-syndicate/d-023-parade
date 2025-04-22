using PageHelpers.Jester.TrackingTransparency.UI.ViewModels;

namespace PageHelpers.Jester.TrackingTransparency.UI.Api {
	public interface ITrackingTransparencyUIService {
		void ShowPermissionScreen (TrackingPermissionNotifyViewModel viewModel);

		void HidePermissionScreen ();
	}
}
