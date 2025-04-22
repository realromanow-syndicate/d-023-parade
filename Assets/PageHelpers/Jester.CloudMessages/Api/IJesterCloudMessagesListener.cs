using System;
using UniRx;

namespace PageHelpers.Jester.CloudMessages.Api {
	public interface IJesterCloudMessagesListener {
		ReactiveCommand<string> onTokenUpdate { get; }

		IReadOnlyReactiveProperty<string> lastValidToken { get; }

		void StartCloudMessaging ();

		void SubscribeOnReceivedCloudMessage (string key, Action<string> onMessageReceived);
	}
}
