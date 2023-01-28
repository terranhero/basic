using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using SC = System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 数据库连接上下文类
	/// </summary>
	public static class EventLogElementContext
	{
		static EventLogElementContext()
		{
			string configName = ConfigurationGroup.ElementName;
			string secName = CulturesSection.ElementName;
			object section = ConfigurationManager.GetSection(string.Concat(configName, "/", secName));
			if (section != null && section is CulturesSection configurationSection)
			{
				InitializeCultures(configurationSection);
			}
		}

		/// <summary>
		/// 读取配置文件中关于多语言配置信息的节
		/// </summary>
		/// <returns>表示数据库配置节集合，</returns>
		public static CulturesSection Cultures
		{
			get { return _DefaultCultures; }
		}

		private static CulturesSection _DefaultCultures;
		private static string _DefaultName;
		/// <summary>
		/// 默认数据库连接配置信息
		/// </summary>
		public static string DefaultName
		{
			get { return _DefaultName; }
			private set { _DefaultName = value; }
		}

		/// <summary>初始化数据库连接参数</summary>
		internal static void InitializeCultures(CulturesSection section)
		{
			_DefaultCultures = section;
			//ConnectionCollection connections = section.Connections;
			//_DefaultName = section.DefaultName;
			//foreach (ConnectionElement element in section.Connections)
			//{
			//	string connectionString = ConnectionStringBuilder.CreateConnectionString(element);
			//	string display = ConnectionStringBuilder.CreateDisplayString(element);
			//	ConnectionInfo info = Create(element.Name, element.ConnectionType, element.Version, connectionString, display);
			//	if (_DefaultName == element.Name) { _DefaultConnection = info; }
			//}
		}
	}

}
