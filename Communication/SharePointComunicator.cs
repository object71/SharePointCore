using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Object71.SharePointCore.Common;

namespace Object71.SharePointCore.Communication {

	internal class SharePointCommunicator {

		private HttpMessageInvoker agent;

		internal SharePointCommunicator(HttpMessageInvoker agent) {
			this.agent = agent;
		}

		internal SharePointCommunicator(HttpClientHandler handler) {
			this.agent = new HttpMessageInvoker(handler);
		}

		internal HttpResponseMessage Ajax(RequestOptions options) {

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
			
			HttpResponseMessage result = agent.SendAsync(request, new CancellationToken()).Sync();

			return result;

		}
	}

}