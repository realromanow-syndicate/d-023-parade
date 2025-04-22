using System.Threading;
using UnityEngine;

namespace PageHelpers {
	public class InternetChecker : MonoBehaviour {
		private CancellationTokenSource _cancellationTokenSource;

		private void Awake () {
			_cancellationTokenSource = new CancellationTokenSource();
		}

		private void OnDestroy () {
			_cancellationTokenSource.Cancel();
			_cancellationTokenSource.Dispose();
		}
	}
}
