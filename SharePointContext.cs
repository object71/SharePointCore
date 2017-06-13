using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Object71.SharePointCore.Authentication;

namespace Object71.SharePointCore {

	public class SharePointContext {
		public ClientContext CreateUserClientContextForSPHost(string url, string username, string password) {
			
			ClientContext context = new ClientContext();
			Task task = UserAuthentication.Authenticate(context, url, username, password);
			task.Wait();
			return context;
		}
	}

}