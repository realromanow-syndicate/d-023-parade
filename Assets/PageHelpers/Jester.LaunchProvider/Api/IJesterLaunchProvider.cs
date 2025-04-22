using Cysharp.Threading.Tasks;
using System.Threading;

namespace PageHelpers.Jester.LaunchProvider.Api {
	public interface IJesterLaunchProvider {
		UniTask LaunchJesterMetaLayer (CancellationToken cancellationToken);
	}
}
