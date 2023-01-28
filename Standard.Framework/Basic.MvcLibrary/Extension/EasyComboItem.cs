using System.Web;
using Basic.EntityLayer;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
    /// <summary>
    /// 表示 easyui 类的实例中的选定项。
    /// </summary>
    public sealed class EasyComboItem
    {
        /// <summary>
        /// 初始化 EasyComboItem 类的新实例。
        /// </summary>
        public EasyComboItem() { }

        /// <summary>
        /// 初始化 EasyComboItem 类的新实例。
        /// </summary>
        /// <param name="itemValue">选定项的值</param>
        /// <param name="itemText">定项的文本</param>
        public EasyComboItem(object itemValue, string itemText) : this(itemValue, itemText, false) { }

        /// <summary>
        /// 初始化 EasyComboItem 类的新实例。
        /// </summary>
        /// <param name="itemValue">选定项的值</param>
        /// <param name="itemText">定项的文本</param>
        /// <param name="isSelected">是否选择此项。</param>
        public EasyComboItem(object itemValue, string itemText, bool isSelected) { Value = itemValue; Text = itemText; Selected = isSelected; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否选择此 EasyComboItem。
        /// </summary>
        /// <value>如果选定此项，则为 true；否则为 false。</value>
        public bool Selected { get; set; }

        /// <summary>
        /// 获取或设置选定项的文本。
        /// </summary>
        /// <value>文本。</value>
        public string Attrs { get; set; }

        /// <summary>
        /// 获取或设置选定项的文本。
        /// </summary>
        /// <value>文本。</value>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置选定项的值。
        /// </summary>
        /// <value>值。</value>
        public object Value { get; set; }

        /// <summary>
        /// 获取节点的Json数据格式表示形式
        /// </summary>
        /// <returns></returns>
        public void WriteJson(HttpResponseBase response)
        {
            response.Write("\"value\":"); response.Write(JsonSerializer.SerializeObject(Value));
            response.Write(",\"text\":"); response.Write(JsonSerializer.SerializeObject(Text));
            //response.Write(string.Format("\"value\":{0}", ));
            //response.Write(string.Format(",\"text\":{0}", JsonSerializer.SerializeObject(Text)));
            if (Selected) { response.Write(",\"selected\":\"true\""); }
            //if (!string.IsNullOrEmpty(Attrs))
            //    response.Write(string.Format(",\"attrs\":\"{0}\"", Attrs));
        }
    }
}
