using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

namespace Standard.Extensions
{
    /// <summary>
    /// Extension entrypoint for the VisualStudio.Extensibility extension.
    /// </summary>
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Extension
    {
        /// <inheritdoc/>
        public override ExtensionConfiguration ExtensionConfiguration => new()
        {
            Metadata = new(
                    id: "Standard.Extensions.5cc3877f-4752-4e34-8a4a-4facbf7426b3",
                    version: this.ExtensionAssemblyVersion,
                    publisherName: "GoldSoft Technology",
                    displayName: "ASP.NET MVC 资源和数据持久设计器 2022+ ",
                    description: "ASP.NET MVC 资源和数据持久设计器 For Visual Studio 2022 and Visual Studio 2026, 支持 Microsoft SQL Server, ORACLE , MySQL等数据库"),
        };

        /// <inheritdoc />
        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
            base.InitializeServices(serviceCollection);

            // You can configure dependency injection here by adding services to the serviceCollection.
        }
    }
}
