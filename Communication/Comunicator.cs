using System;
using System.Net.Http;

namespace Object71.SharePointCore.Communication {

	public class Communicator {

		private HttpMessageInvoker agent;

		public Communicator(HttpMessageInvoker agent) {
			this.agent = agent;
		}

		public Communicator(HttpClientHandler handler) {
			this.agent = new HttpMessageInvoker(handler);
		}

		public HttpResponseMessage Ajax(string url, RequestOptions options) {
			return null;
		}

		public async void AsyncAjax(string url, RequestOptions options) {
			throw new NotImplementedException();
		}
	}

}