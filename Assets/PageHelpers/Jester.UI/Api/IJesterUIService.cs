using UnityEngine;

namespace PageHelpers.Jester.UI.Api {
	public interface IJesterUIService {
		void SetupJesterForm<TArg> (GameObject form, Canvas canvas, TArg item = default);

		void RemoveJesterForm (GameObject form, Canvas canvas);
	}
}
