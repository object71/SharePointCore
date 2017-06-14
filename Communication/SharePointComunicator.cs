using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Object71.SharePointCore.Communication {

	public class SharePointCommunicator {

		private HttpMessageInvoker agent;

		public SharePointCommunicator(HttpMessageInvoker agent) {
			this.agent = agent;
		}

		public SharePointCommunicator(HttpClientHandler handler) {
			this.agent = new HttpMessageInvoker(handler);
		}

		public HttpResponseMessage Ajax(RequestOptions options) {

			HttpRequestMessage request = new HttpRequestMessage();

			request.RequestUri = options.Url;
			request.Method = options.Method;

			if(options.Data != null) {

				byte[] requestData = Encoding.ASCII.GetBytes(options.Data);
				request.Content.Headers.Add("Content-Type", options.ContentType ?? "text/plain");
				request.Content = new ByteArrayContent(requestData);
				request.Content.Headers.Add("Content-Length", requestData.Length.ToString());
				
			}

			foreach(KeyValuePair<string, string> header in options.Headers) {
				request.Headers.Add(header.Key, header.Value);
			}
			
			Task<HttpResponseMessage> promise = agent.SendAsync(request, new CancellationToken());
			promise.Wait();

			return promise.Result;

		}
	}

}