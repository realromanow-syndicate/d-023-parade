using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace PageHelpers.Jester.LayerLauncher.Api {
	public interface IJesterApiService {
		UniTask<Dictionary<string, object>> GetJesterConfigAsync (Dictionary<string, object> senderData, CancellationToken cancellationToken);
	}
}
