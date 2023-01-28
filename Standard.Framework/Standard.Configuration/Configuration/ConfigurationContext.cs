using System.Configuration;
using System.IO;
using Basic.Exceptions;
using SC = System.Configuration;

namespace Basic.Configuration
{
	/// <summary>
	/// 表示金软基础框架配置文件上下文信息
	/// </summary>
	public static class ConfigurationContext
	{
		private static SC.Configuration configurationFile = null;
		private static ConfigurationGroup configurationGroup = null;
		private static string ConfigurationFilePath;

		static ConfigurationContext()
		{

			//ConfigurationFilePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
		}

		/// <summary>
		/// 判断当前配置文件是否已经读取
		/// </summary>
		public static bool ReadConfiguration { get { return configurationFile != null; } }

		/// <summary>
		/// 重置当前配置文件
		/// </summary>
		public static void ResetConfiguration()
		{
			configurationFile = null; configurationGroup = null; ConfigurationFilePath = null;
			//ConnectionsSection section = GetGroupSection<ConnectionsSection>(ConnectionsSection.ElementName);
			//ConnectionContext.InitializeConnection(section);
		}

		/// <summary>
		/// 初始化配置信息
		/// </summary>
		/// <param name="configPath">配置文件路径</param>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static SC.Configuration ResetConfiguration(string configPath)
		{
			if (configPath == null) { configPath = ConfigurationFilePath; }
			if (!File.Exists(configPath))   //读取配置文件异常，配置文件"{0}"不存在。
				throw new ConfigurationFileException("Access_Configuration_FileNotFound", configPath);
			ConfigurationFileMap fileMap = new ConfigurationFileMap(configPath);
			SC.Configuration _Config = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);

			ConfigurationSectionGroup csc = _Config.GetSectionGroup(ConfigurationGroup.ElementName);
			if (csc == null)        //读取配置文件异常，配置文件"{0}"中，不存在自定义配置组"Basic.Configuration'。
				throw new ConfigurationFileException("Access_Configuration_GroupNotFound", configPath, ConfigurationGroup.ElementName);
			configurationGroup = csc as ConfigurationGroup;
			//读取配置文件异常，配置文件"{0}"中，自定义配置组"Basic.Configuration"，
			//类型必须是"Basic.Configuration.ConfigurationGroup"。
			if (configurationGroup == null)
				throw new ConfigurationFileException("Access_Configuration_SectionTypeError",
					configPath, ConfigurationGroup.ElementName, typeof(ConfigurationGroup));
			configurationFile = _Config;
			return configurationFile;
		}

		/// <summary>
		/// 初始化配置信息
		/// </summary>
		/// <param name="configPath">配置文件路径</param>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static SC.Configuration InitializeConfiguration(string configPath)
		{
			if (configurationFile != null) { return configurationFile; }
			if (configPath == null) { configPath = ConfigurationFilePath; }
			if (!File.Exists(configPath))   //读取配置文件异常，配置文件"{0}"不存在。
				throw new ConfigurationFileException("Access_Configuration_FileNotFound", configPath);
			ConfigurationFileMap fileMap = new ConfigurationFileMap(configPath);
			SC.Configuration _Config = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);

			ConfigurationSectionGroup csc = _Config.GetSectionGroup(ConfigurationGroup.ElementName);
			if (csc == null)        //读取配置文件异常，配置文件"{0}"中，不存在自定义配置组"Basic.Configuration'。
				throw new ConfigurationFileException("Access_Configuration_GroupNotFound", configPath, ConfigurationGroup.ElementName);
			configurationGroup = csc as ConfigurationGroup;
			//读取配置文件异常，配置文件"{0}"中，自定义配置组"Basic.Configuration"，
			//类型必须是"Basic.Configuration.ConfigurationGroup"。
			if (configurationGroup == null)
				throw new ConfigurationFileException("Access_Configuration_SectionTypeError",
					configPath, ConfigurationGroup.ElementName, typeof(ConfigurationGroup));
			configurationFile = _Config;
			return configurationFile;
		}

		///// <summary>
		///// 从配置文件中获取配置节组
		///// </summary>
		///// <param name="configPath">配置文件路径</param>
		///// <returns></returns>
		//public static ConfigurationGroup GetConfigurationGroup(string configPath)
		//{
		//	if (configurationGroup != null) { return configurationGroup; }
		//	SC.Configuration configuration = InitializeConfiguration(configPath);
		//	ConfigurationSectionGroup csc = configuration.GetSectionGroup(ConfigurationGroup.ElementName);
		//	if (csc == null)        //读取配置文件异常，配置文件"{0}"中，不存在自定义配置组"Basic.Configuration'。
		//		throw new ConfigurationFileException("Access_Configuration_GroupNotFound", ConfigurationFilePath, ConfigurationGroup.ElementName);
		//	configurationGroup = csc as ConfigurationGroup;
		//	//读取配置文件异常，配置文件"{0}"中，自定义配置组""，类型必须是""。
		//	if (configurationGroup == null)
		//		throw new ConfigurationFileException("Access_Configuration_SectionTypeError",
		//			ConfigurationFilePath, ConfigurationGroup.ElementName, typeof(ConfigurationGroup));
		//	return configurationGroup;
		//}

		///// <summary>
		///// 从配置文件中获取配置节组
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="groupName"></param>
		///// <returns></returns>
		//public static T GetSectionGroup<T>(string groupName) where T : ConfigurationSectionGroup
		//{
		//	SC.Configuration configuration = InitializeConfiguration(ConfigurationFilePath);
		//	ConfigurationSectionGroup csc = configuration.GetSectionGroup(groupName);
		//	if (csc == null)        //读取配置文件异常，配置文件"{0}"中，不存在自定义配置组"Basic.Configuration'。
		//		throw new ConfigurationFileException("Access_Configuration_GroupNotFound", ConfigurationFilePath, groupName);
		//	T configurationSectionGroup = csc as T;
		//	//读取配置文件异常，配置文件"{0}"中，自定义配置组""，类型必须是""。
		//	if (configurationSectionGroup == null)
		//		throw new ConfigurationFileException("Access_Configuration_SectionTypeError",
		//			ConfigurationFilePath, groupName, typeof(T));
		//	return configurationSectionGroup;
		//}

		/// <summary>
		/// 从配置文件中获取配置节
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		public static T GetSection<T>(string sectionName) where T : SC.ConfigurationSection
		{
			SC.Configuration configuration = InitializeConfiguration(ConfigurationFilePath);
			SC.ConfigurationSection section = configuration.GetSection(sectionName);
			if (section == null)        //读取配置文件异常，配置文件"{0}"中，不存在自定义配置组"'。
				throw new ConfigurationFileException("Access_Configuration_GroupNotFound", ConfigurationFilePath, sectionName);
			T configurationSection = section as T;
			//读取配置文件异常，配置文件"{0}"中，自定义配置组""，类型必须是""。
			if (configurationSection == null)
				throw new ConfigurationFileException("Access_Configuration_SectionTypeError",
					ConfigurationFilePath, sectionName, typeof(T));
			return configurationSection;
		}

		/// <summary>
		/// 从配置文件中获取配置节
		/// </summary>
		/// <typeparam name="T">ConfigurationSection 子类类型。</typeparam>
		/// <param name="sectionName">配置节名称。</param>
		/// <param name="required">当前配置节是否必须。</param>
		/// <returns>返回 ConfigurationSection 子类实例。</returns>
		public static T GetGroupSection<T>(string sectionName, bool required = true) where T : SC.ConfigurationSection
		{
			string configName = ConfigurationGroup.ElementName;
			object section = ConfigurationManager.GetSection(string.Concat(configName, "/", sectionName));
			if (section == null)  /*读取配置文件异常，配置文件"{0}"中，自定义配置组""，类型必须是""*/
				throw new ConfigurationFileException("Access_Configuration_SectionTypeError", ConfigurationFilePath, sectionName, typeof(T));

			T configurationSection = section as T;
			if (configurationSection == null && required == true)
				throw new ConfigurationFileException("Access_Configuration_SectionTypeError",
					ConfigurationFilePath, sectionName, typeof(T));
			return configurationSection;
		}

		/// <summary>
		/// 从配置文件中获取数据库连接信息的配置节
		/// </summary>
		/// <returns>表示数据库配置节集合，</returns>
		public static ConnectionsSection Connections
		{
			get { return GetGroupSection<ConnectionsSection>(ConnectionsSection.ElementName); }
		}
		private static EasyLibrarySection _EasyUILibrary = null;
		/// <summary>
		/// 读取配置文件中关于 EasyUI 配置信息
		/// </summary>
		/// <returns>表示EasyUI配置信息</returns>
		public static EasyLibrarySection GetEasyLibrary()
		{
			if (_EasyUILibrary == null)
			{
				_EasyUILibrary = GetGroupSection<EasyLibrarySection>(EasyLibrarySection.ElementName, false);
				if (_EasyUILibrary == null) { _EasyUILibrary = new EasyLibrarySection(); }
			}
			else if (_EasyUILibrary != null && _EasyUILibrary.Modified)
			{
				_EasyUILibrary = GetGroupSection<EasyLibrarySection>(EasyLibrarySection.ElementName, false);
				if (_EasyUILibrary == null) { _EasyUILibrary = new EasyLibrarySection(); }
			}
			return _EasyUILibrary;
		}

		/// <summary>
		/// 读取配置文件中关于多语言配置信息的节
		/// </summary>
		/// <returns>表示数据库配置节集合，</returns>
		public static CulturesSection Cultures
		{
			get { return GetGroupSection<CulturesSection>(CulturesSection.ElementName); }
		}

		/// <summary>
		/// 读取配置文件中关于多语言配置信息的节
		/// </summary>
		/// <returns>表示数据库配置节集合，</returns>
		public static EventLogsSection EventLogs
		{
			get { return GetGroupSection<EventLogsSection>(EventLogsSection.ElementName); }
		}

		/// <summary>
		/// 读取配置文件中关于多语言配置信息的节
		/// </summary>
		/// <returns>表示数据库配置节集合，</returns>
		public static MessagersSection Messagers
		{
			get { return GetGroupSection<MessagersSection>(MessagersSection.ElementName, false); }
		}
	}
}
