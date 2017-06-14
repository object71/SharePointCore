using System;
using System.Net;
using System.Xml;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Object71.SharePointCore;
using Object71.SharePointCore.Common;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Object71.SharePointCore.Authentication {

	public class UserAuthentication {

		internal static async Task Authenticate(ClientContext context, string username, string password) {
			
			string securityToken = await UserAuthentication.GetSecurityToken(context, username, password);
			await UserAuthentication.GetAccessToken(context, securityToken);
			
		}

		private static async Task<string> GetSecurityToken(ClientContext context, string username, string password) {

			string envelope = FormatEnvelope(username, password, context.SharePointUri.AbsoluteUri);

			return await UserAuthentication.GetSecurityToken(context, envelope, context.SharePointUri.AbsoluteUri);
		}

		private static async Task<string> GetSecurityToken(ClientContext context, string envelope) {

			byte[] requestData = Encoding.ASCII.GetBytes(envelope);
			
			HttpRequestMessage request = new HttpRequestMessage();
			request.RequestUri = new Uri(Constants.LoginUrl);
			request.Method = HttpMethod.Post;
			request.Content = new ByteArrayContent(requestData);
			request.Content.Headers.Add("Content-Type", "application/xml");
			request.Content.Headers.Add("Content-Length", requestData.Length.ToString());
			
			HttpResponseMessage result = await context.HttpSender.SendAsync(request, new CancellationToken());

			string message = await result.Content.ReadAsStringAsync();

			Regex binaryTokenMatcher = new Regex(@"<([a-zA-Z]*:)?BinarySecurityToken.*>(.*)<\/([a-zA-Z]*:)?BinarySecurityToken>");
			
			string binaryToken = binaryTokenMatcher.Match(message).Groups[2].Value;

			return binaryToken;
		}

		private static async Task GetAccessToken(ClientContext context, string securityToken) {

			byte[] requestData = Encoding.ASCII.GetBytes(securityToken);
			
			HttpRequestMessage request = new HttpRequestMessage();
			request.RequestUri = new Uri("https://" + context.SharePointUri.Host + "/_forms/default.aspx?wa=wsignin1.0");
			request.Method = HttpMethod.Post;
			request.Content = new ByteArrayContent(requestData);
			request.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
			request.Content.Headers.Add("Content-Length", requestData.Length.ToString());
			
			HttpResponseMessage result = await context.HttpSender.SendAsync(request, new CancellationToken());

		}

		private static string FormatEnvelope(string username, string password, string endpoint) {
			
			string envelope = Envelopes.TokenEnvelope;
			envelope = envelope.Replace("{{username}}", WebUtility.HtmlEncode(username));
			envelope = envelope.Replace("{{password}}", WebUtility.HtmlEncode(password));
			envelope = envelope.Replace("{{endpoint}}", WebUtility.HtmlEncode(endpoint));

			return envelope;
		}
	}

}