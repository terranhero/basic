using System;
using System.Collections.Generic;

namespace Basic.Interfaces
{
	/// <summary>表示用户权限的验证</summary>
	public interface IAuthorizeValidation
	{
		/// <summary>获取角色导航菜单</summary>
		/// <returns>当前角色的导航菜单</returns>
		NavigateMenuCollection GetNavigateMenus();

		/// <summary>检查授权码是否有效</summary>
		/// <param name="code">The authorization code.</param>
		/// <returns>如果有效则返回true，否则返回false。</returns>
		bool CheckCode(int code);

		/// <summary>检查区段授权码是否有效(从开始授权码检查到结束授权码，只要有一个条件则授权成功)。</summary>
		/// <param name="bCode">按钮授权码开始编号</param>
		/// <param name="eCode">按钮授权码结束编号</param>
		/// <returns>如果有效则返回true，否则返回false。</returns>
		bool CheckCode(int bCode, int eCode);

		/// <summary>检查一组授权码是否成功。</summary>
		/// <param name="codes">表示一组授权码</param>
		/// <returns>这组授权码其中一个检测成功则返回true，所有授权码检测都不成功则返回false。</returns>
		bool CheckCode(params int[] codes);

		/// <summary>检查授权码是否有效</summary>
		/// <param name="code">授权码</param>
		/// <returns>如果有效则返回true，否则返回false。</returns>
		bool CheckCode(string code);

		/// <summary>检查一组授权码是否成功。</summary>
		/// <param name="codes">表示一组授权码</param>
		/// <returns>这组授权码其中一个检测成功则返回true，所有授权码检测都不成功则返回false。</returns>
		bool CheckCode(params string[] codes);

		/// <summary>检查授权码是否有效</summary>
		/// <param name="code">授权码</param>
		/// <returns>如果有效则返回true，否则返回false。</returns>
		bool CheckCode(Guid code);

		/// <summary>检查一组授权码是否成功。</summary>
		/// <param name="codes">表示一组授权码</param>
		/// <returns>这组授权码其中一个检测成功则返回true，所有授权码检测都不成功则返回false。</returns>
		bool CheckCode(params Guid[] codes);

	}

	/// <summary>导航菜单</summary>
	public sealed class NavigateMenu : global::Basic.EntityLayer.AbstractEntity
	{
		private readonly NavigateMenuCollection _menus;

		/// <summary>初始化 NavigateMenu 类的实例。</summary>
		public NavigateMenu() : base() { _menus = new NavigateMenuCollection(this); }

		/// <summary>子菜单</summary>
		public NavigateMenuCollection Children { get { return _menus; } }

		/// <summary>菜单关键字</summary>
		public int MenuKey { get; set; }

		/// <summary>排序</summary>
		public int ShowOrder { get; set; }

		/// <summary>菜单文本</summary>
		public string MenuName { get; set; }

		/// <summary>菜单简称</summary>
		public string ShortName { get; set; }

		/// <summary>菜单英文名称</summary>
		public string MenuText { get; set; }

		/// <summary>菜单英文简称</summary>
		public string ShortText { get; set; }

		/// <summary>图片路径</summary>
		public string ImagePath { get; set; }

		/// <summary>菜单导航路径</summary>
		public string UrlPath { get; set; }

		/// <summary>查询字段</summary>
		public string UrlQuery { get; set; }

		/// <summary>帮助URL</summary>
		public string UrlHelp { get; set; }

		/// <summary>菜单是否展开</summary>
		public bool Expanded { get; set; }

		/// <summary>备注</summary>
		public string Memo { get; set; }

		/// <summary>将对象添加到 NavigateMenuCollection 的结尾处</summary>
		/// <param name="item">要添加到 NavigateMenuCollection 末尾的对象。 对于引用类型，该值可以为 null</param>
		/// <returns>返回添加到集合末尾的元素</returns>
		public NavigateMenu Add(NavigateMenu item)
		{
			return _menus.Add(item);
		}

		/// <summary>将指定集合的元素添加到 NavigateMenuCollection 的末尾</summary>
		/// <param name="collection">一个集合，其元素应被添加到 NavigateMenuCollection 的末尾。 
		/// 集合自身不能为 null，但它可以包含为 null 的元素（如果类型 NavigateMenu 为引用类型）。</param>
		public void AddRange(IEnumerable<NavigateMenu> collection)
		{
			_menus.AddRange(collection);
		}
	}

	/// <summary>导航菜单集合</summary>
	[global::System.Runtime.InteropServices.GuidAttribute("66B154DD-B11C-4663-9FEF-9760A2BEC7DA")]
	public partial class NavigateMenuCollection : global::Basic.Collections.AbstractEntityCollection<NavigateMenu>
	{
		/// <summary>初始化 NavigateMenuCollection 类的实例。</summary>
		public NavigateMenuCollection() : base() { }

		private readonly NavigateMenu _owner;
		/// <summary>初始化 NavigateMenuCollection 类的实例。</summary>
		public NavigateMenuCollection(NavigateMenu owner) : base(owner) { _owner = owner; }

		/// <summary>初始化 NavigateMenuCollection 类的实例。</summary>
		/// <param name="list">从中复制元素的集合</param>
		private NavigateMenuCollection(Basic.Interfaces.IPagination<NavigateMenu> list) : base(list) { }

		/// <summary>初始化 NavigateMenuCollection 类的实例。</summary>
		/// <param name="collection">从中复制元素的集合</param>
		private NavigateMenuCollection(System.Collections.Generic.IEnumerable<NavigateMenu> collection) : base(collection) { }
	}
}
