using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using Basic.Enums;

namespace Basic.DataEntities
{
    /// <summary>
    /// 实体类属性信息
    /// </summary>
    public sealed class DataConditionPropertyElement : AbstractPropertyElement
    {
        private readonly DataConditionElement dataCondition;

        #region 构造函数
        /// <summary>
        /// Initializes a new instance of a DataEntityPropertyElement object.
        /// </summary>
        /// <param name="owner">拥有此属性的实体定义文件</param>
        internal DataConditionPropertyElement(DataConditionElement owner) : this(owner, null, typeof(string), false) { }

        /// <summary>
        /// Initializes a new instance of a DataEntityPropertyElement object.
        /// </summary>
        /// <param name="owner">拥有此属性的实体定义文件</param>
        /// <param name="name">连接字符串的名称。</param>
        internal DataConditionPropertyElement(DataConditionElement owner, string name) : this(owner, name, typeof(string), false) { }

        /// <summary>
        /// Initializes a new instance of a DataEntityPropertyElement object.
        /// </summary>
        /// <param name="owner">拥有此属性的实体定义文件</param>
        /// <param name="name">连接字符串的名称。</param>
        /// <param name="type">属性类型。</param>
        internal DataConditionPropertyElement(DataConditionElement owner, string name, Type type) : this(owner, name, type, false) { }

        /// <summary>
        /// Initializes a new instance of a DataEntityPropertyElement object.
        /// </summary>
        /// <param name="owner">拥有此属性的实体定义文件</param>
        /// <param name="name">属性名称。</param>
        /// <param name="type">属性类型。</param>
        /// <param name="nullable">属性是否不能为空。</param>
        internal DataConditionPropertyElement(DataConditionElement owner, string name, Type type, bool nullable)
            : base(owner, name, type, nullable) { this.dataCondition = owner; }
        #endregion

        /// <summary>
        /// 获取条件属性是否存在属性名称。
        /// </summary>
        internal string HasName { get { return string.Concat("Has", base.Name); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityClass"></param>
        /// <param name="pkConstructor"></param>
        /// <returns></returns>
        protected internal override CodeMemberProperty WriteDesignerCode(CodeTypeDeclaration entityClass, CodeConstructor pkConstructor)
        {
            CodeBaseReferenceExpression baseReference = new CodeBaseReferenceExpression();
            int index = dataCondition.Arguments.IndexOf(this);
            CodeMemberProperty property = base.WriteDesignerCode(entityClass, pkConstructor);
            property.SetStatements.Clear();
            if (Modifier == PropertyModifierEnum.Public)
                property.Attributes = MemberAttributes.Public;
            else if (Modifier == PropertyModifierEnum.Internal)
                property.Attributes = MemberAttributes.Assembly;
            else if (Modifier == PropertyModifierEnum.Private)
                property.Attributes = MemberAttributes.Private;
            else if (Modifier == PropertyModifierEnum.Protected)
                property.Attributes = MemberAttributes.Family;
            else if (Modifier == PropertyModifierEnum.ProtectedInternal)
                property.Attributes = MemberAttributes.FamilyOrAssembly;
            if (Override) { property.Attributes = property.Attributes | MemberAttributes.Override; }
            else if (!Virtual) { property.Attributes = property.Attributes | MemberAttributes.Final; }

            CodeVariableReferenceExpression field = new CodeVariableReferenceExpression(FieldName);
            // 产生 m_property = value;
            CodeConditionStatement conditionStatement = new CodeConditionStatement();
            conditionStatement.Condition = new CodeBinaryOperatorExpression(field,
                 CodeBinaryOperatorType.IdentityInequality, new CodePropertySetValueReferenceExpression());

            CodeMethodInvokeExpression setBitValueMethod = new CodeMethodInvokeExpression();
            setBitValueMethod.Method = new CodeMethodReferenceExpression(baseReference, "SetBitValue");
            setBitValueMethod.Parameters.Add(new CodePrimitiveExpression(index));
            conditionStatement.TrueStatements.Add(setBitValueMethod);

            CodeAssignStatement propertyAssignment = new CodeAssignStatement(field, new CodePropertySetValueReferenceExpression());
            conditionStatement.TrueStatements.Add(propertyAssignment);

            CodeMethodInvokeExpression propertyChanged = new CodeMethodInvokeExpression();
            propertyChanged.Method = new CodeMethodReferenceExpression(baseReference, "OnPropertyChanged");
            propertyChanged.Parameters.Add(new CodePrimitiveExpression(Name));
            conditionStatement.TrueStatements.Add(propertyChanged);
            property.SetStatements.Add(conditionStatement);

            CodeMemberProperty hasValue = new CodeMemberProperty();
            hasValue.Name = HasName;
            hasValue.Attributes = MemberAttributes.Final;
            if (Modifier == PropertyModifierEnum.Public)
                hasValue.Attributes = hasValue.Attributes | MemberAttributes.Public;
            else if (Modifier == PropertyModifierEnum.Internal)
                hasValue.Attributes = hasValue.Attributes | MemberAttributes.Assembly;
            else if (Modifier == PropertyModifierEnum.Private)
                hasValue.Attributes = hasValue.Attributes | MemberAttributes.Private;
            else if (Modifier == PropertyModifierEnum.Protected)
                hasValue.Attributes = hasValue.Attributes | MemberAttributes.Family;
            else if (Modifier == PropertyModifierEnum.ProtectedInternal)
                hasValue.Attributes = hasValue.Attributes | MemberAttributes.FamilyOrAssembly;
            hasValue.Comments.Add(new CodeCommentStatement("<summary>", true));
            if (string.IsNullOrWhiteSpace(Comment))
                hasValue.Comments.Add(new CodeCommentStatement(string.Format("属性\"{0}\"的值是否已更改。", Name), true));
            else
                hasValue.Comments.Add(new CodeCommentStatement(Comment, true));
            hasValue.Comments.Add(new CodeCommentStatement("</summary>", true));
            hasValue.Comments.Add(new CodeCommentStatement(string.Concat("<value>如果属性值已更改，则为 true；否则为 false。默认值为 false。</value>"), true));
            hasValue.Type = new CodeTypeReference(typeof(bool));
            hasValue.HasGet = true;
            CodeMethodInvokeExpression hasValueMethod = new CodeMethodInvokeExpression();
            hasValueMethod.Method = new CodeMethodReferenceExpression(baseReference, "HasValue");
            hasValueMethod.Parameters.Add(new CodePrimitiveExpression(index));
            hasValue.GetStatements.Add(new CodeMethodReturnStatement(hasValueMethod));
            entityClass.Members.Add(hasValue);
            return property;
        }
    }
}
