using UnityEngine;
using UnityEngine.UI;

namespace PageHelpers {
	public class NameChecker : MonoBehaviour {
		[SerializeField]
		private Text _label;

		private void Start () {
			_label.text = PlayerPrefs.GetString("PlayerName", "Jester");
		}
	}
}
