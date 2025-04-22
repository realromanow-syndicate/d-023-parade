using Cysharp.Threading.Tasks;
using System.Threading;

namespace PageHelpers.Jester.LayerLauncher.Api {
	public interface IJesterLauncherService {
		UniTask LaunchJesterMetaAsync (CancellationToken cancellationToken);
	}
}
