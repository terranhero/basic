using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace Basic.Localizations
{
	internal static class PackageExtensions
	{
		internal static void AddCommand(this OleMenuCommandService mcs, CommandID id, EventHandler invokeHandler, EventHandler beforeQueryStatus)
		{
			mcs.AddCommand(new OleMenuCommand(invokeHandler, null, beforeQueryStatus, id));
		}

		internal static void AddCommand(this OleMenuCommandService mcs, CommandID id, EventHandler invokeHandler)
		{
			mcs.AddCommand(new OleMenuCommand(invokeHandler, id));
		}

		internal static void AddCommand(this OleMenuCommandService mcs, CommandID id, string text, EventHandler invokeHandler)
		{
			mcs.AddCommand(new OleMenuCommand(invokeHandler, id, text));
		}

		internal static void AddCommand(this OleMenuCommandService mcs, CommandID id, string text, EventHandler invokeHandler, EventHandler beforeQueryStatus)
		{
			mcs.AddCommand(new OleMenuCommand(invokeHandler, null, beforeQueryStatus, id, text));
		}
	}
}
