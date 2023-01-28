using Basic.Configuration;
using Basic.Views;
using System.Collections.ObjectModel;

namespace Basic.Builders
{
	/// <summary>
	/// 构建经典视图(*.cshtml)
	/// </summary>
	internal class ClassicViewBuilder : AbstractMvcViewBuilder
	{
		/// <summary>
		/// 初始化 ClassicViewBuilder 类实例。
		/// </summary>
		internal ClassicViewBuilder(PersistentService service, EnvDTE.ProjectItem item) : base(service, item) { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="views"></param>
		protected override void InilitilizeViews(ObservableCollection<MvcView> views)
		{
			if (views.Count == 0)
			{
				views.Add(new MvcIndexView(this, TemplateFiles, _AbstractEntities));
				views.Add(new MvcGridView(this, TemplateFiles, _AbstractEntities));
				views.Add(new MvcEditView(this, TemplateFiles, _AbstractEntities, "Create"));
				views.Add(new MvcEditView(this, TemplateFiles, _AbstractEntities, "Edit"));
				views.Add(new MvcDetailView(this, TemplateFiles, _AbstractEntities, "Detail"));
				views.Add(new MvcEditView(this, TemplateFiles, _AbstractEntities, "ComplexSearch"));
				views.Add(new MvcScriptView(this, TemplateFiles, _AbstractEntities));
			}
		}
	}
}
