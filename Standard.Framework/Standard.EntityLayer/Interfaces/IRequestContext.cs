using System.Collections.Specialized;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public interface IRequestContext
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

		/// <summary></summary>
		string Browser { get; }
	}
}
