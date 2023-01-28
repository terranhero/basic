using System.Text;

namespace Basic.MvcLibrary
{
	/// <summary>表示 HttpResponse 上下文操作</summary>
	public interface IResponseContext
	{
		/// <summary></summary>
		void ClearContent();

		/// <summary></summary>
		void ClearHeaders();

		/// <summary></summary>
		void BinaryWrite(byte[] bytes);

		/// <summary></summary>
		void AddHeader(string header, string value);

		/// <summary></summary>
		string ContentType { get; set; }

		/// <summary></summary>
		System.Text.Encoding ContentEncoding { get; set; }

		/// <summary></summary>
		void Flush();

		/// <summary></summary>
		void End();
	}
}
