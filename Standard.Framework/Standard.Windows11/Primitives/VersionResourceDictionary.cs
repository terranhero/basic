// ***********************************************************************
// Assembly         : Basic.Windows
// Author           : JACKY
// Created          : 12-19-2014
//
// Last Modified By : JACKY
// Last Modified On : 12-19-2014
// ***********************************************************************
// <copyright file="VersionResourceDictionary.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Basic.Primitives
{
	/// <summary>
	/// 程序集资源字典
	/// </summary>
	public class AssemblyResourceDictionary : ResourceDictionary, ISupportInitialize
	{
		/// <summary>
		/// The _initializing count
		/// </summary>
		private int _initializingCount;
		/// <summary>
		/// The _source path
		/// </summary>
		private string _sourcePath;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyResourceDictionary" /> class.
		/// </summary>
		public AssemblyResourceDictionary() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyResourceDictionary" /> class.
		/// </summary>
		/// <param name="sourcePath">The source path.</param>
		public AssemblyResourceDictionary(string sourcePath)
		{
			((ISupportInitialize)this).BeginInit();
			this.SourcePath = sourcePath;
			((ISupportInitialize)this).EndInit();
		}

		/// <summary>
		/// Gets or sets the source path.
		/// </summary>
		/// <value>The source path.</value>
		public string SourcePath
		{
			get { return _sourcePath; }
			set
			{
				this.EnsureInitialization();
				_sourcePath = value;
			}
		}

		/// <summary>
		/// Ensures the initialization.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">VersionResourceDictionary properties can only be set while initializing</exception>
		private void EnsureInitialization()
		{
			if (_initializingCount <= 0)
				throw new InvalidOperationException("VersionResourceDictionary properties can only be set while initializing");
		}

		/// <summary>
		/// 开始此 <see cref="T:System.Windows.ResourceDictionary" /> 的初始化阶段。
		/// </summary>
		void ISupportInitialize.BeginInit()
		{
			base.BeginInit();
			_initializingCount++;
		}

		/// <summary>
		/// 结束初始化阶段，并使上一个树无效，以便在初始化阶段对键所做的更改都可以得到解决。
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Source property cannot be initialized on the VersionResourceDictionary</exception>
		void ISupportInitialize.EndInit()
		{
			_initializingCount--;
			Debug.Assert(_initializingCount >= 0);

			if (_initializingCount <= 0)
			{
				if (this.Source != null)
					throw new InvalidOperationException("Source property cannot be initialized on the VersionResourceDictionary");

				AssemblyName assemblyInfo = typeof(AssemblyResourceDictionary).Assembly.GetName();
				string assemblyName = assemblyInfo.Name;
				string uriStr = string.Format(@"/{0};component/{1}", assemblyName, this.SourcePath);
				this.Source = new Uri(uriStr, UriKind.Relative);
			}

			base.EndInit();
		}


		/// <summary>
		/// Enum InitState
		/// </summary>
		private enum InitState
		{
			/// <summary>
			/// The not initialized
			/// </summary>
			NotInitialized,
			/// <summary>
			/// The initializing
			/// </summary>
			Initializing,
			/// <summary>
			/// The initialized
			/// </summary>
			Initialized
		};
	}
}
