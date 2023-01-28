// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Usage", "VSTHRD001", Justification = "<挂起>")]
[assembly: SuppressMessage("Usage", "VSTHRD010:在主线程上调用单线程类型", Justification = "<挂起>")]
[assembly: SuppressMessage("CodeQuality", "IDE0052:删除未读的私有成员", Justification = "<挂起>")]
[assembly: SuppressMessage("Usage", "VSTHRD002:避免有问题的同步等待", Justification = "<挂起>")]
[assembly: SuppressMessage("Usage", "VSTHRD110:观察异步调用的结果", Justification = "<挂起>")]
[assembly: SuppressMessage("Style", "IDE0059:不需要赋值", Justification = "<挂起>")]
