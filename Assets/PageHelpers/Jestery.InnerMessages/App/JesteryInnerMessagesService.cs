using Cysharp.Threading.Tasks;
using PageHelpers.Jester.CloudMessages.Api;
using System;
using UniRx;
using UnityEngine;

namespace PageHelpers.Jestery.InnerMessages.App {
	public class JesteryInnerMessagesService : IDisposable {
		private readonly IJesterCloudMessagesListener  _jesterCloudMessagesListener;
		private readonly IJesterCloudMessagesService _jesterCloudMessagesService;
		private readonly CompositeDisposable  _disposables = new();
		
		public JesteryInnerMessagesService(IJesterCloudMessagesListener jesterCloudMessagesListener, IJesterCloudMessagesService jesterCloudMessagesService) {
			_jesterCloudMessagesListener = jesterCloudMessagesListener;
			_jesterCloudMessagesService = jesterCloudMessagesService;
		}

		public async UniTask Init () {
			PlayerPrefs.SetString("PlayerName", "You");
			
			_jesterCloudMessagesService.hasMessagesPermissionStatus
				.Subscribe(status => {
					if (status) _jesterCloudMessagesListener.StartCloudMessaging();
				})
				.AddTo(_disposables);
			
			if (!_jesterCloudMessagesService.hasMessagesPermissionStatus.Value && _jesterCloudMessagesService.timeMessagesAskPermissionExpiredStatus.Value) {
				await _jesterCloudMessagesService.RequestQuickMessagesPermission();
			}
		}
		
		public void Dispose() {
			_disposables.Dispose();
		}
	}
}
