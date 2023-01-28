using BM = Basic.MvcLibrary;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public static class ButtonsProvider
	{
		/// <summary>初始化 Toolbar 类实例</summary>
		public static IButtonsProvider<T> Buttons<T>(this IBasicContext<T> basic)
		{
			return new ButtonsProvider<T>(basic);
		}
	}

	/// <summary>表示列按钮输出提供程序</summary>
	/// <typeparam name="T"></typeparam>
	public interface IButtonsProvider<T>
	{
		/// <summary>表示按钮</summary>
		Button Button();

		/// <summary>表示按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button Button(int code);

		/// <summary>表示按钮</summary>
		/// <param name="show">是否显示此按钮</param>
		Button Button(bool show);

		/// <summary>表示按钮</summary>
		Button ElButton();

		/// <summary>表示按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button ElButton(int code);

		/// <summary>表示按钮</summary>
		/// <param name="show">是否显示此按钮</param>
		Button ElButton(bool show);
	}


	/// <summary>表示列按钮输出提供程序</summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ButtonsProvider<T> : IButtonsProvider<T>
	{
		private readonly IBasicContext mBasic;
		/// <summary>初始化 ButtonsProvider 类实例</summary>
		internal ButtonsProvider(IBasicContext bc) { mBasic = bc; }

		/// <summary>表示按钮</summary>
		public Button ElButton()
		{
			return new Button(mBasic, ViewTags.ElButton);
		}

		/// <summary>表示按钮</summary>
		/// <param name="show">是否显示此按钮</param>
		public Button ElButton(bool show)
		{
			if (show == false) { return BM.Button.Empty(mBasic); }
			return ElButton();
		}

		/// <summary>表示按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button ElButton(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(mBasic, code);
			if (isAuthorization == false) { return BM.Button.Empty(mBasic); }
			return ElButton();
		}

		/// <summary>表示按钮</summary>
		public Button Button()
		{
			return new Button(mBasic, ButtonType.Custom);
		}

		/// <summary>表示按钮</summary>
		/// <param name="show">是否显示此按钮</param>
		public Button Button(bool show)
		{
			if (show == false) { return BM.Button.Empty(mBasic); }
			return Button();
		}

		/// <summary>表示按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Button(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(mBasic, code);
			if (isAuthorization == false) { return BM.Button.Empty(mBasic); }
			return Button();
		}
	}
}
