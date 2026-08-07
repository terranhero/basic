using Basic.Enums;

namespace StandardTest.BasicPersistent
{
    public class EnumsTests
    {
        #region ArgumentsTypeEnum Tests

        [Fact]
        public void ArgumentsTypeEnum_ShouldHaveCorrectValues()
        {
            Assert.Equal(1, (int)ArgumentsTypeEnum.SingleModel);
            Assert.Equal(2, (int)ArgumentsTypeEnum.ArrayModels);
            Assert.Equal(3, (int)ArgumentsTypeEnum.Parameters);
            Assert.Equal(4, (int)ArgumentsTypeEnum.NoneArguments);
        }

        #endregion

        #region BaseAccessEnum Tests

        [Fact]
        public void BaseAccessEnum_ShouldHaveCorrectValues()
        {
            Assert.Equal(0, (int)BaseAccessEnum.AbstractDbAccess);
            Assert.Equal(1, (int)BaseAccessEnum.AbstractAccess);
        }

        #endregion

        #region ClassModifierEnum Tests

        [Fact]
        public void ClassModifierEnum_ShouldHaveCorrectValues()
        {
            Assert.Equal(0, (int)ClassModifierEnum.Public);
            Assert.Equal(1, (int)ClassModifierEnum.Internal);
        }

        #endregion

        #region PropertyModifierEnum Tests

        [Fact]
        public void PropertyModifierEnum_ShouldHaveCorrectValues()
        {
            Assert.Equal(0, (int)PropertyModifierEnum.Public);
            Assert.Equal(1, (int)PropertyModifierEnum.Internal);
            Assert.Equal(2, (int)PropertyModifierEnum.Private);
            Assert.Equal(3, (int)PropertyModifierEnum.Protected);
            Assert.Equal(4, (int)PropertyModifierEnum.ProtectedInternal);
        }

        #endregion

        #region StaticMethodEnum Tests

        [Fact]
        public void StaticMethodEnum_ShouldHaveAllValues()
        {
            var values = Enum.GetValues<StaticMethodEnum>();
            Assert.Equal(9, values.Length);
        }

        #endregion

        #region DynamicMethodEnum Tests

        [Fact]
        public void DynamicMethodEnum_ShouldHaveAllValues()
        {
            var values = Enum.GetValues<DynamicMethodEnum>();
            Assert.Equal(3, values.Length);
        }

        #endregion

        #region GenerateActionEnum Tests

        [Fact]
        public void GenerateActionEnum_ShouldHaveCorrectValues()
        {
            Assert.Equal(1, (int)GenerateActionEnum.Single);
            Assert.Equal(2, (int)GenerateActionEnum.Multiple);
        }

        #endregion

        #region GenerateModeEnum Tests

        [Fact]
        public void GenerateModeEnum_ShouldHaveCorrectValues()
        {
            Assert.Equal(1, (int)GenerateModeEnum.DataEntity);
            Assert.Equal(2, (int)GenerateModeEnum.DataTable);
            Assert.Equal(3, (int)GenerateModeEnum.TableEntity);
        }

        #endregion

        #region MethodModifierEnum Tests

        [Fact]
        public void MethodModifierEnum_ShouldHaveCorrectValues()
        {
            Assert.Equal(0, (int)MethodModifierEnum.Public);
            Assert.Equal(1, (int)MethodModifierEnum.Private);
            Assert.Equal(2, (int)MethodModifierEnum.Internal);
            Assert.Equal(3, (int)MethodModifierEnum.Protected);
            Assert.Equal(4, (int)MethodModifierEnum.ProtectedInternal);
        }

        #endregion

        #region ObjectTypeEnum Tests (Flags)

        [Fact]
        public void ObjectTypeEnum_ShouldBeFlags()
        {
            Assert.True(typeof(ObjectTypeEnum).GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0);
        }

        [Fact]
        public void ObjectTypeEnum_ShouldHaveCorrectValues()
        {
            Assert.Equal(0x1, (int)ObjectTypeEnum.UserTable);
            Assert.Equal(0x2, (int)ObjectTypeEnum.UserView);
            Assert.Equal(0x4, (int)ObjectTypeEnum.ClrTableFunction);
            Assert.Equal(0x8, (int)ObjectTypeEnum.SqlTableFunction);
            Assert.Equal(0x10, (int)ObjectTypeEnum.InlineTableFunction);
            Assert.Equal(0x20, (int)ObjectTypeEnum.StoredProcedure);
        }

        [Fact]
        public void ObjectTypeEnum_ShouldSupportBitwiseCombination()
        {
            var combined = ObjectTypeEnum.UserTable | ObjectTypeEnum.UserView;
            Assert.True(combined.HasFlag(ObjectTypeEnum.UserTable));
            Assert.True(combined.HasFlag(ObjectTypeEnum.UserView));
            Assert.False(combined.HasFlag(ObjectTypeEnum.StoredProcedure));
        }

        #endregion

        #region OrderEnum Tests

        [Fact]
        public void OrderEnum_ShouldHaveCorrectValues()
        {
            Assert.Equal(0, (int)OrderEnum.None);
            Assert.Equal(1, (int)OrderEnum.Ascending);
            Assert.Equal(2, (int)OrderEnum.Descending);
        }

        #endregion

        #region SelectedTypeEnum Tests

        [Fact]
        public void SelectedTypeEnum_ShouldHaveCorrectValues()
        {
            Assert.Equal(0, (byte)SelectedTypeEnum.None);
            Assert.Equal(1, (byte)SelectedTypeEnum.DataEntityItem);
            Assert.Equal(2, (byte)SelectedTypeEnum.ConditionItem);
            Assert.Equal(3, (byte)SelectedTypeEnum.CommandItem);
            Assert.Equal(4, (byte)SelectedTypeEnum.EntityProperty);
            Assert.Equal(5, (byte)SelectedTypeEnum.ConditionProperty);
            Assert.Equal(6, (byte)SelectedTypeEnum.DataCommand);
        }

        #endregion

        #region ConnectionTypes Tests

        [Fact]
        public void ConnectionTypes_ShouldHaveCorrectValues()
        {
            Assert.Equal(0, (int)ConnectionTypes.Default);
            Assert.Equal(1, (int)ConnectionTypes.SQLSERVER);
            Assert.Equal(2, (int)ConnectionTypes.ORACLE);
            Assert.Equal(4, (int)ConnectionTypes.MYSQL);
            Assert.Equal(6, (int)ConnectionTypes.DB2);
            Assert.Equal(8, (int)ConnectionTypes.PGSQL);
            Assert.Equal(10, (int)ConnectionTypes.SQLITE);
        }

        #endregion
    }
}
