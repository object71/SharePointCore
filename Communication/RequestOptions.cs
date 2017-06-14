using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Object71.SharePointCore.Communication {

	public class RequestOptions {
		public string ContentType { get; set; }
		public string Data { get; set; }
		public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
		public HttpMethod Method { get; set; }
		public Uri Url { get; set; }
		
	}


}