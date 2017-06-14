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
		protected HttpClientHandler Handler { get; set; }
		internal HttpMessageInvoker HttpSender { get; set; }
		private readonly Uri sharePointUri;
		public Uri SharePointUri { get; }

		public ClientContext(string url) {

			this.sharePointUri = new Uri(url);
			this.InitHttp();

		}

		public ClientContext(Uri uri) {

			this.sharePointUri = uri;
			this.InitHttp();

		}

		internal void AddCookie(string endpoint, string name, string value) {
			this.Handler.CookieContainer.Add(new Uri(endpoint), new Cookie(name, value));
		}

		private void InitHttp() {

			Handler = new HttpClientHandler();
			Handler.UseCookies = true;
			Handler.CookieContainer = new CookieContainer();
			Handler.AllowAutoRedirect = false;

			HttpSender = new HttpMessageInvoker(Handler);
		}

		public void Authenticate(string username, string password) {
			UserAuthentication.Authenticate(context, username, password);
		}

		public HttpResponseMessage RawGetRequest(string url) {

			HttpRequestMessage request = new HttpRequestMessage();
			request.RequestUri = new Uri(url);
			request.Method = HttpMethod.Get;
			Task<HttpResponseMessage> promise = this.HttpSender.SendAsync(request, new CancellationToken());
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
			
			Task<HttpResponseMessage> promise = this.HttpSender.SendAsync(request, new CancellationToken());
			promise.Wait();

			return promise.Result;

		}

		
	}

}