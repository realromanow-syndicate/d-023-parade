using UniRx;
using UnityEngine;

namespace PageHelpers.Jester.DI.Binding {
	public class JesterItemBinding<TItem> : MonoBehaviour {
		[SerializeField]
		private string _bindId;
		
		public TItem item { get; private set; }
		public string itemId { get; private set; }
		public string bindId => _bindId;

		protected CompositeDisposable bindingDisposable { get; private set; } = new();

		private void OnDestroy () {
			RegisterDestroy();
		}

		protected virtual void RegisterInitialize () {}

		protected virtual void RegisterDestroy () {
			bindingDisposable.Dispose();
		}

		public void ForceDispose () {
			if (!bindingDisposable.IsDisposed) {
				bindingDisposable.Dispose();
			}
			
			bindingDisposable = new CompositeDisposable();
		}

		public void SetItem (TItem itemModel, string id) {
			item = itemModel;
			itemId = id;

			RegisterInitialize();
		}
	}
}
