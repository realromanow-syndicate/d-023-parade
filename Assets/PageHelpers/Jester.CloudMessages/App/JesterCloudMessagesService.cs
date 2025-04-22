using Cysharp.Threading.Tasks;
using PageHelpers.Jester.CloudMessages.Api;
using PageHelpers.Jester.CloudMessages.UI.Api;
using PageHelpers.Jester.CloudMessages.UI.ViewModels;
using System;
using UniRx;
using UnityEngine;

namespace PageHelpers.Jester.CloudMessages.App {
	public class JesterCloudMessagesService : IJesterCloudMessagesService, IDisposable {
		private const string USER_CHARGE_NOTIFICATION_PERMISSION_KEY = "user_notifications";
		private const string USER_CHARGE_NOTIFICATION_TIME_KEY = "user_notification_time";

		public ReactiveCommand<bool> onPermissionStatusUpdated { get; } = new();
		public IReadOnlyReactiveProperty<bool> hasMessagesPermissionStatus => _hasMessagesPermissionStatus;
		public IReadOnlyReactiveProperty<bool> timeMessagesAskPermissionExpiredStatus => _timeMessagesAskPermissionExpiredStatus;

		private readonly IJesterCloudMessagesUIScreenService _uiScreenService;
		private readonly CompositeDisposable _compositeDisposable = new();
		private readonly ReactiveProperty<bool> _hasMessagesPermissionStatus = new();
		private readonly ReactiveProperty<bool> _timeMessagesAskPermissionExpiredStatus = new();

		public JesterCloudMessagesService (IJesterCloudMessagesUIScreenService uiScreenService) {
			_uiScreenService = uiScreenService;

			_hasMessagesPermissionStatus.Value = HasPermission();
			_timeMessagesAskPermissionExpiredStatus.Value = TimeAskExpiredAt() <= DateTime.UtcNow;
		}

		public async UniTask RequestQuickMessagesPermission () {
			var viewModel = CreateViewModel();

			viewModel.permissionUpdate
				.Subscribe(ObserveStatus)
				.AddTo(_compositeDisposable);

			_uiScreenService.RegisterJesterPermissionScreen(viewModel);

			await WhiteUntilPermissionReceived(viewModel);
		}

		private static async UniTask WhiteUntilPermissionReceived (JesterMessagesPermissionScreenViewModel viewModel) {
			while (!viewModel.isHasPermitAnswer) {
				await UniTask.Yield();
			}
		}

		private void ObserveStatus (bool isPermit) {
			if (isPermit) SetPositivePermission();
			else SetAskTime(DateTime.UtcNow.AddSeconds(259200));

			_uiScreenService.RemoveJesterPermissionScreen();
		}

		private JesterMessagesPermissionScreenViewModel CreateViewModel () {
			var viewModel = new JesterMessagesPermissionScreenViewModel().AddTo(_compositeDisposable);
			return viewModel;
		}

		private static bool HasPermission () {
			return PlayerPrefs.GetInt(USER_CHARGE_NOTIFICATION_PERMISSION_KEY, 0) >= 1;
		}

		private static DateTime TimeAskExpiredAt () {
			var dateTimeRaw = PlayerPrefs.GetString(USER_CHARGE_NOTIFICATION_TIME_KEY, string.Empty);

			return string.IsNullOrEmpty(dateTimeRaw) ? DateTime.UtcNow : DateTime.Parse(dateTimeRaw);
		}

		private void SetPositivePermission () {
			PlayerPrefs.SetInt(USER_CHARGE_NOTIFICATION_PERMISSION_KEY, 1);
			UpdatePermissionValueToPositive();
		}

		private void UpdatePermissionValueToPositive () {
			_hasMessagesPermissionStatus.Value = true;
			onPermissionStatusUpdated.Execute(true);
		}

		private void SetAskTime (DateTime dateTime) {
			PlayerPrefs.SetString(USER_CHARGE_NOTIFICATION_TIME_KEY, dateTime.ToString());
			UpdatePermissionValueToNegative();
		}

		private void UpdatePermissionValueToNegative () {
			_hasMessagesPermissionStatus.Value = TimeAskExpiredAt() <= DateTime.UtcNow;
			onPermissionStatusUpdated.Execute(false);
		}

		public void Dispose() {
			_hasMessagesPermissionStatus.Dispose();
			_timeMessagesAskPermissionExpiredStatus.Dispose();
			_compositeDisposable.Dispose();
			onPermissionStatusUpdated.Dispose();
		}
	}
}
