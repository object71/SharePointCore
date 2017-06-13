using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Object71.SharePointCore.Authentication;

namespace Object71.SharePointCore {

	public class ClientContext {
		protected HttpClientHandler handler;
		internal HttpMessageInvoker httpSender;

		internal ClientContext() {

			this.InitHttp();

		}

		internal void AddCookie(string endpoint, string name, string value) {
			this.handler.CookieContainer.Add(new Uri(endpoint), new Cookie(name, value));
		}

		private void InitHttp() {

			handler = new HttpClientHandler();
			handler.UseCookies = true;
			handler.CookieContainer = new CookieContainer();
			handler.AllowAutoRedirect = false;

			httpSender = new HttpMessageInvoker(handler);
		}

		public HttpResponseMessage RawGetRequest(string url) {

			HttpRequestMessage request = new HttpRequestMessage();
			request.RequestUri = new Uri(url);
			request.Method = HttpMethod.Get;
			Task<HttpResponseMessage> promise = this.httpSender.SendAsync(request, new CancellationToken());
			promise.Wait();

			return promise.Result;

		}

		public HttpResponseMessage RawPostRequest(string url, string content = "") {

			byte[] requestData = Encoding.ASCII.GetBytes(content);

			HttpRequestMessage request = new HttpRequestMessage();
			request.RequestUri = new Uri(url);
			request.Method = HttpMethod.Post;
			
			request.Content = new ByteArrayContent(requestData);
			request.Content.Headers.Add("Content-Type", "application/xml");
			request.Content.Headers.Add("Content-Length", requestData.Length.ToString());
			
			Task<HttpResponseMessage> promise = this.httpSender.SendAsync(request, new CancellationToken());
			promise.Wait();

			return promise.Result;

		}

		
	}

}