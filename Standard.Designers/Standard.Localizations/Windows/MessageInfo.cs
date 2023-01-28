namespace Basic.Windows
{
    /// <summary>
    /// 资源转换器名称
    /// </summary>
    internal sealed class MessageInfo
    {
        /// <summary>
        /// 采用项目信息初始化 MessageInfo 类实例。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fileName"></param>
        internal MessageInfo(string name, string fileName)
            : base()
        {
            _ConverterName = name;
            _FileName = fileName;
        }

        private string _ConverterName = null;
        private string _FileName = null;

        /// <summary>
        /// Visual Studio项目项目唯一名称
        /// </summary>
        public string ConverterName { get { return _ConverterName; } set { _ConverterName = value; } }

        /// <summary>
        /// Visual Studio项目显示名称
        /// </summary>
        public string FileName { get { return _FileName; } set { _FileName = value; } }

        /// <summary>
        /// 判断当前项目信息是否为空。
        /// </summary>
        public bool IsEmpty { get { return string.IsNullOrWhiteSpace(_ConverterName); } }

        /// <summary>
        /// 判断当前项目信息是否为空。
        /// </summary>
        public bool NotEmpty { get { return !IsEmpty; } }
    }
}
