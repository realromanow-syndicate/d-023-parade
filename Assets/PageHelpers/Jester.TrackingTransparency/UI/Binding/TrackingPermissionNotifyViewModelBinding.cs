using PageHelpers.Jester.DI.Binding;
using PageHelpers.Jester.TrackingTransparency.UI.ViewModels;

namespace PageHelpers.Jester.TrackingTransparency.UI.Binding {
	public class TrackingPermissionNotifyViewModelBinding : JesterItemBinding<TrackingPermissionNotifyViewModel> {
		public void Submit () {
			item.Submit();
		}
	}
}
