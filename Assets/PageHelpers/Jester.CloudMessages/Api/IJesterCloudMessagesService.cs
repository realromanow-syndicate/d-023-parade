using Cysharp.Threading.Tasks;
using UniRx;

namespace PageHelpers.Jester.CloudMessages.Api {
	public interface IJesterCloudMessagesService {
		ReactiveCommand<bool> onPermissionStatusUpdated { get; }
		IReadOnlyReactiveProperty<bool> hasMessagesPermissionStatus { get; }
		IReadOnlyReactiveProperty<bool> timeMessagesAskPermissionExpiredStatus { get; }

		UniTask RequestQuickMessagesPermission ();
	}
}
