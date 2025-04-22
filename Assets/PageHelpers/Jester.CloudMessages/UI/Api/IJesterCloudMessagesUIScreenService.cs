using PageHelpers.Jester.CloudMessages.UI.ViewModels;

namespace PageHelpers.Jester.CloudMessages.UI.Api {
	public interface IJesterCloudMessagesUIScreenService {
		void RegisterJesterPermissionScreen (JesterMessagesPermissionScreenViewModel viewModel);
		
		void RemoveJesterPermissionScreen ();
	}
}
