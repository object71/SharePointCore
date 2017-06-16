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
using Object71.SharePointCore.Communication;

namespace Object71.SharePointCore.Authentication {

	public class UserAuthentication {

		internal static void Authenticate(ClientContext context, string username, string password) {
			
			string securityToken = UserAuthentication.GetSecurityToken(context, username, password);
			UserAuthentication.GetAccessToken(context, securityToken);
			
		}

		private static string GetSecurityToken(ClientContext context, string username, string password) {

			string envelope = FormatEnvelope(username, password, context.SharePointUri.AbsoluteUri);

			return UserAuthentication.GetSecurityToken(context, envelope);
		}

		private static string GetSecurityToken(ClientContext context, string envelope) {

			
			
			HttpResponseMessage result = context.Communicator.Ajax(new RequestOptions {
				Url = new Uri(Constants.LoginUrl),
				Method = HttpMethod.Post,
				Data = envelope,
				ContentType = "application/xml",
			});

			// TODO: Implement logic for federation authentication.

			string message = result.Content.ReadAsStringAsync().Sync();

			Regex binaryTokenMatcher = new Regex(@"<([a-zA-Z]*:)?BinarySecurityToken.*>(.*)<\/([a-zA-Z]*:)?BinarySecurityToken>");
			
			string binaryToken = binaryTokenMatcher.Match(message).Groups[2].Value;

			return binaryToken;
		}

		private static void GetAccessToken(ClientContext context, string securityToken) {

			HttpResponseMessage result = context.Communicator.Ajax(new RequestOptions {
				Url = new Uri("https://" + context.SharePointUri.Host + "/_forms/default.aspx?wa=wsignin1.0"),
				Method = HttpMethod.Post,
				Data = securityToken,
				ContentType = "application/x-www-form-urlencoded",
			});

		}

		internal static string GetRequestDigest(ClientContext context) {

			HttpResponseMessage result = context.Communicator.Ajax(new RequestOptions {
				Url = new Uri("https://" + context.SharePointUri.Host + "/_api/contextinfo"),
				Method = HttpMethod.Post,
				Data = "",
			});

			string message = result.Content.ReadAsStringAsync().Sync();

			Regex formDigestValueMatcher = new Regex(@"<([a-zA-Z]*:)?FormDigestValue.*>(.*)<\/([a-zA-Z]*:)?FormDigestValue>");
			
			string requestDigest = formDigestValueMatcher.Match(message).Groups[2].Value;

			return requestDigest;

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