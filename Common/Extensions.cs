
using System.Threading.Tasks;

namespace Object71.SharePointCore.Common {

	internal static class Extensions {
		internal static void Sync(this Task task) {
			task.Wait();
		}

		internal static T Sync<T>(this Task<T> task) {
			task.Wait();
			return task.Result;
		}
	}

}