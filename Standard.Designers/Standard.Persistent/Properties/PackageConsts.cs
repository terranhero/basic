using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Package Guid List
/// </summary>
public static class Guids
{
    /// <summary>
    /// 实体模型设计器Package Guid
    /// </summary>
    public const string ModelPackageGuidString = "B54CB7D4-8524-48BF-9394-7C88DEF073A7";

    /// <summary>
    /// 
    /// </summary>
    public const string ModelPanelGuidString = "02B6E868-3BE3-423E-93D2-7588517B16AA";
    /// <summary>
    /// 
    /// </summary>
    public static readonly Guid ModelPackageGuid = new Guid(ModelPackageGuidString);
    /// <summary>
    /// 
    /// </summary>
    public const string ModelFactoryGuidString = "17F6BE82-F3A0-4995-AC13-AC8600A492CA";
    public static readonly Guid ModelFactoryGuid = new Guid(ModelFactoryGuidString);

    public const string EntityOptionGuidString = "0213C581-24F4-454B-A46D-FB9E5BAC378D";
    public const string ConditionOptionGuidString = "9F534AB5-F6AD-4013-A864-CA80DB508487";
}

/// <summary>
/// 表示基础开发工具常量
/// </summary>
public static class PackageConsts
{
    /// <summary>
    /// 产品版本号
    /// </summary>
    public const string ProductVersion = "4.0.0.0";


}
