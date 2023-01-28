using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basic.Interfaces
{
	/// <summary>表示单个 HTTP 请求的传出端</summary>
	public interface ICustomResponse
	{
		/// <summary>获取或设置 HTTP 响应代码。</summary>
		int StatusCode { get; set; }

		/// <summary>获取响应标头。</summary>
		IHeaderDictionary Headers { get; }

		/// <summary>获取或设置 Content-Length 响应标头 的值。</summary>
		long? ContentLength { get; set; }

		/// <summary>获取或设置 Content-Type 响应标头 的值。</summary>
		string ContentType { get; set; }

		/// <summary>获取一个 对象，该对象可用于管理此响应的 Cookie。</summary>
		IResponseCookies Cookies { get; }

		/// <summary>获取或设置响应正文 Stream 。</summary>
		System.IO.Stream Body { get; }

		/// <summary>将给定的文本写入响应正文。 将使用 UTF-8 编码</summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void AddHeader(string key, StringValues value);

		/// <summary>将字节序列异步写入当前流，并将流的当前位置提升写入的字节数。</summary>
		/// <param name="buffer">从中写入数据的缓冲区。</param>
		/// <returns>表示异步写入操作的任务。</returns>
		void Write(byte[] buffer);

		/// <summary>将给定的文本写入响应正文。 将使用 UTF-8 编码</summary>
		/// <param name="text">要写入响应中的文本。</param>
		/// <returns>表示完成写入操作的任务</returns>
		void Write(string text);

		/// <summary>使用给定的编码将给定的文本写入响应正文</summary>
		/// <param name="text">要写入响应中的文本。</param>
		/// <param name="encoding">要使用的编码</param>
		/// <returns>表示完成写入操作的任务</returns>
		void Write(string text, Encoding encoding);

		/// <summary>异步清理此流的所有缓冲区，导致所有缓冲数据都写入基础设备，并且监控取消请求</summary>
		/// <returns>表示异步写入操作的任务。</returns>
		void Flush();

		/// <summary>异步清理此流的所有缓冲区，导致所有缓冲数据都写入基础设备，并且监控取消请求</summary>
		/// <param name="cancellationToken">要监视取消请求的标记。 默认值是 None。</param>
		/// <returns>表示异步写入操作的任务。</returns>
		Task FlushAsync(CancellationToken cancellationToken = default);

		/// <summary>将字节序列异步写入当前流，并将流的当前位置提升写入的字节数。</summary>
		/// <param name="buffer">从中写入数据的缓冲区。</param>
		/// <param name="cancellationToken">要监视取消请求的标记。 默认值是 None。</param>
		/// <returns>表示异步写入操作的任务。</returns>
		Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default);

		/// <summary>将给定的文本写入响应正文。 将使用 UTF-8 编码</summary>
		/// <param name="text">要写入响应中的文本。</param>
		/// <param name="cancellationToken">应取消请求操作时发出通知。</param>
		/// <returns>表示完成写入操作的任务</returns>
		Task WriteAsync(string text, CancellationToken cancellationToken = default);

		/// <summary>使用给定的编码将给定的文本写入响应正文</summary>
		/// <param name="text">要写入响应中的文本。</param>
		/// <param name="encoding">要使用的编码</param>
		/// <param name="cancellationToken">应取消请求操作时发出通知。</param>
		/// <returns>表示完成写入操作的任务</returns>
		Task WriteAsync(string text, Encoding encoding, CancellationToken cancellationToken = default);

		/// <summary>清除 HTTP 响应,此调用重置响应标头、响应状态代码和响应正文。</summary>
		void Clear();
	}

	/// <summary>表示单个 HTTP 请求的传出端</summary>
	public class CustomResponse : ICustomResponse
	{
		private readonly HttpResponse _response;
		/// <summary></summary>
		/// <param name="res"></param>
		public CustomResponse(HttpResponse res) { _response = res; }

		/// <summary>获取或设置 HTTP 响应代码。</summary>
		public int StatusCode { get { return _response.StatusCode; } set { _response.StatusCode = value; } }

		/// <summary>获取响应标头。</summary>
		public IHeaderDictionary Headers { get { return _response.Headers; } }

		/// <summary>获取或设置 Content-Length 响应标头 的值。</summary>
		public long? ContentLength { get { return _response.ContentLength; } set { _response.ContentLength = value; } }

		/// <summary>获取或设置 Content-Type 响应标头 的值。</summary>
		public string ContentType { get { return _response.ContentType; } set { _response.ContentType = value; } }

		/// <summary>获取一个 对象，该对象可用于管理此响应的 Cookie。</summary>
		public IResponseCookies Cookies { get { return _response.Cookies; } }

		/// <summary>获取或设置响应正文 Stream 。</summary>
		public System.IO.Stream Body { get { return _response.Body; } }

		/// <summary>将给定的文本写入响应正文。 将使用 UTF-8 编码</summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void AddHeader(string key, StringValues value)
		{
			_response.Headers.Append(key, value);
		}
		/// <summary>将字节序列异步写入当前流，并将流的当前位置提升写入的字节数。</summary>
		/// <param name="buffer">从中写入数据的缓冲区。</param>
		/// <returns>表示异步写入操作的任务。</returns>
		public void Write(byte[] buffer) { _response.Body.Write(buffer, 0, buffer.Length); }

		/// <summary>将给定的文本写入响应正文。 将使用 UTF-8 编码</summary>
		/// <param name="text">要写入响应中的文本。</param>
		/// <returns>表示完成写入操作的任务</returns>
		public void Write(string text)
		{
			if (_response == null) { throw new ArgumentNullException("response"); }
			if (text == null) { throw new ArgumentNullException("text"); }
			Write(text, Encoding.UTF8);
		}

		/// <summary>使用给定的编码将给定的文本写入响应正文</summary>
		/// <param name="text">要写入响应中的文本。</param>
		/// <param name="encoding">要使用的编码</param>
		/// <returns>表示完成写入操作的任务</returns>
		public void Write(string text, Encoding encoding)
		{
			if (_response == null) { throw new ArgumentNullException("response"); }
			if (text == null) { throw new ArgumentNullException("text"); }
			if (encoding == null) { throw new ArgumentNullException("encoding"); }
			Write(encoding.GetBytes(text));
		}

		/// <summary>异步清理此流的所有缓冲区，导致所有缓冲数据都写入基础设备，并且监控取消请求</summary>
		/// <returns>表示异步写入操作的任务。</returns>
		public void Flush() { _response.Body.Flush(); }

		/// <summary>将字节序列异步写入当前流，并将流的当前位置提升写入的字节数。</summary>
		/// <param name="buffer">从中写入数据的缓冲区。</param>
		/// <param name="cancellationToken">要监视取消请求的标记。 默认值是 None。</param>
		/// <returns>表示异步写入操作的任务。</returns>
		public Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
		{
			return _response.Body.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
		}

		/// <summary>异步清理此流的所有缓冲区，导致所有缓冲数据都写入基础设备，并且监控取消请求</summary>
		/// <param name="cancellationToken">要监视取消请求的标记。 默认值是 None。</param>
		/// <returns>表示异步写入操作的任务。</returns>
		public Task FlushAsync(CancellationToken cancellationToken = default)
		{
			return _response.Body.FlushAsync(cancellationToken);
		}

		/// <summary>将给定的文本写入响应正文。 将使用 UTF-8 编码</summary>
		/// <param name="text">要写入响应中的文本。</param>
		/// <param name="cancellationToken">应取消请求操作时发出通知。</param>
		/// <returns>表示完成写入操作的任务</returns>
		public Task WriteAsync(string text, CancellationToken cancellationToken = default)
		{
			if (_response == null) { throw new ArgumentNullException("response"); }
			if (text == null) { throw new ArgumentNullException("text"); }
			return WriteAsync(text, Encoding.UTF8, cancellationToken);
		}

		/// <summary>使用给定的编码将给定的文本写入响应正文</summary>
		/// <param name="text">要写入响应中的文本。</param>
		/// <param name="encoding">要使用的编码</param>
		/// <param name="cancellationToken">应取消请求操作时发出通知。</param>
		/// <returns>表示完成写入操作的任务</returns>
		public Task WriteAsync(string text, Encoding encoding, CancellationToken cancellationToken = default)
		{
			if (_response == null) { throw new ArgumentNullException("response"); }
			if (text == null) { throw new ArgumentNullException("text"); }
			if (encoding == null) { throw new ArgumentNullException("encoding"); }

			byte[] bytes = encoding.GetBytes(text);
			return WriteAsync(bytes, cancellationToken);
		}

		/// <summary>清除 HTTP 响应,此调用重置响应标头、响应状态代码和响应正文。</summary>
		public void Clear() { _response.Clear(); }
	}
}
