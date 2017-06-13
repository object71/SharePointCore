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

		internal static async Task Authenticate(ClientContext context, string endpoint, string username, string password) {
			
			string securityToken = await UserAuthentication.GetSecurityToken(context, username, password, endpoint);
			await UserAuthentication.GetAccessToken(context, securityToken, endpoint);
			
		}

		private static async Task<string> GetSecurityToken(ClientContext context, string username, string password, string endpoint) {

			string envelope = FormatEnvelope(username, password, endpoint);

			return await UserAuthentication.GetSecurityToken(context, envelope, endpoint);
		}

		private static async Task<string> GetSecurityToken(ClientContext context, string envelope, string endpoint) {

			byte[] requestData = Encoding.ASCII.GetBytes(envelope);
			
			HttpRequestMessage request = new HttpRequestMessage();
			request.RequestUri = new Uri("https://login.microsoftonline.com/extSTS.srf");
			request.Method = HttpMethod.Post;
			request.Content = new ByteArrayContent(requestData);
			request.Content.Headers.Add("Content-Type", "application/xml");
			request.Content.Headers.Add("Content-Length", requestData.Length.ToString());
			
			HttpResponseMessage result = await context.httpSender.SendAsync(request, new CancellationToken());

			string message = await result.Content.ReadAsStringAsync();

			Regex binaryTokenMatcher = new Regex(@"<([a-zA-Z]*:)?BinarySecurityToken.*>(.*)<\/([a-zA-Z]*:)?BinarySecurityToken>");
			
			string binaryToken = binaryTokenMatcher.Match(message).Groups[2].Value;

			return binaryToken;
		}

		private static async Task GetAccessToken(ClientContext context, string securityToken, string endpoint) {

			byte[] requestData = Encoding.ASCII.GetBytes(securityToken);
			
			HttpRequestMessage request = new HttpRequestMessage();
			request.RequestUri = new Uri(endpoint + "_forms/default.aspx?wa=wsignin1.0");
			request.Method = HttpMethod.Post;
			request.Content = new ByteArrayContent(requestData);
			request.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
			request.Content.Headers.Add("Content-Length", requestData.Length.ToString());
			
			HttpResponseMessage result = await context.httpSender.SendAsync(request, new CancellationToken());

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