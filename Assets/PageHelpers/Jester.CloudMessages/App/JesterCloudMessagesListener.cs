using Firebase.Messaging;
using PageHelpers.Jester.CloudMessages.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace PageHelpers.Jester.CloudMessages.App {
	public class JesterCloudMessagesListener : IJesterCloudMessagesListener, IDisposable {
		public IReadOnlyReactiveProperty<string> lastValidToken => _lastValidToken;
		public ReactiveCommand<string> onTokenUpdate { get; } = new();

		private readonly ReactiveProperty<string> _lastValidToken = new();
		private readonly Dictionary<string, Action<string>> _clouds = new();

		public void StartCloudMessaging () {
			SubscribeAnalytics();
		}

		private void SubscribeAnalytics () {
			FirebaseMessaging.TokenReceived += OnTokenReceived;
			FirebaseMessaging.MessageReceived += OnCloudMessageReceived;
		}

		private void OnTokenReceived (object sender, TokenReceivedEventArgs e) {
			_lastValidToken.Value = e.Token;
			onTokenUpdate.Execute(e.Token);
		}

		public void SubscribeOnReceivedCloudMessage (string key, Action<string> onMessageReceived) {
			_clouds.Add(key, onMessageReceived);
		}

		private void OnCloudMessageReceived (object sender, MessageReceivedEventArgs e) {
			var messageData = GetMessages(e, out var messages);

			foreach (var message in messages) {
				if (_clouds.Any(x => x.Key == message)) _clouds[message].Invoke(messageData[message]);
			}
		}

		private static IDictionary<string, string> GetMessages (MessageReceivedEventArgs e, out string[] messages) {
			var messageData = e.Message.Data;
			messages = ConvertMessages(messageData).ToArray();
			return messageData;
		}

		private static IEnumerable<string> ConvertMessages (IDictionary<string, string> messageData) {
			return messageData.Select(x => x.Key);
		}

		public void Dispose () {
			onTokenUpdate.Dispose();
			_lastValidToken.Dispose();
		}
	}
}
