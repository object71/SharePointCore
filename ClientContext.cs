using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Object71.SharePointCore.Authentication;
using Object71.SharePointCore.Communication;

namespace Object71.SharePointCore {

	public class ClientContext {
		internal HttpClientHandler Handler { get; set; }
		internal HttpMessageInvoker HttpSender { get; set; }
		protected SharePointCommunicator Communicator { get; set; }
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

			this.Handler = new HttpClientHandler();
			this.Handler.UseCookies = true;
			this.Handler.CookieContainer = new CookieContainer();
			this.Handler.AllowAutoRedirect = false;

			this.HttpSender = new HttpMessageInvoker(Handler);

			this.Communicator = new SharePointCommunicator(this.HttpSender);

		}

		public void Authenticate(string username, string password) {
			UserAuthentication.Authenticate(this, username, password).Wait();
		}

		public HttpResponseMessage RawGetRequest(string url) {

			HttpResponseMessage response = Communicator.Ajax(new RequestOptions {
				Method = HttpMethod.Get,
				Url = new Uri(url),
			});

			return response;

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