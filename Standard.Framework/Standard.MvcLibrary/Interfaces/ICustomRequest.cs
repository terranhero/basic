using System.Collections.Specialized;

namespace Basic.Interfaces
{
	/// <summary></summary>
	public interface ICustomRequest
	{
		/// <summary></summary>
		string HttpMethod { get; }

		/// <summary></summary>
		NameValueCollection Form { get; }

		/// <summary></summary>
		NameValueCollection Headers { get; }

		/// <summary></summary>
		string ContentType { get; set; }

		/// <summary></summary>
		NameValueCollection QueryString { get; }
	}
}
