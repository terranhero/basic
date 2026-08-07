using Basic.Designer;
using Basic.EntityLayer;
using Basic.Enums;

namespace StandardTest.BasicPersistent
{
    public class StringHelperTests
    {
        #region IsUpper Tests

        [Theory]
        [InlineData("ABCDEF", true)]
        [InlineData("ABC", true)]
        [InlineData("A", true)]
        [InlineData("ABC123", false)]
        [InlineData("abc", false)]
        [InlineData("Abc", false)]
        [InlineData("", false)]
        public void IsUpper_ShouldReturnExpectedResult(string input, bool expected)
        {
            bool result = StringHelper.IsUpper(input);
            Assert.Equal(expected, result);
        }

        #endregion

        #region GetLowerCase Tests

        [Fact]
        public void GetLowerCase_WithNull_ReturnsNull()
        {
            Assert.Null(StringHelper.GetLowerCase(null));
        }

        [Theory]
        [InlineData("ABC", "abc")]
        [InlineData("Hello", "hello")]
        [InlineData("TEST123", "test123")]
        public void GetLowerCase_ShouldConvertToLower(string input, string expected)
        {
            string result = StringHelper.GetLowerCase(input);
            Assert.Equal(expected, result);
        }

        #endregion

        #region GetUpperCase Tests

        [Fact]
        public void GetUpperCase_WithNull_ReturnsNull()
        {
            Assert.Null(StringHelper.GetUpperCase(null));
        }

        [Theory]
        [InlineData("abc", "ABC")]
        [InlineData("Hello", "HELLO")]
        [InlineData("test123", "TEST123")]
        public void GetUpperCase_ShouldConvertToUpper(string input, string expected)
        {
            string result = StringHelper.GetUpperCase(input);
            Assert.Equal(expected, result);
        }

        #endregion

        #region GetCamelCase Tests

        [Fact]
        public void GetCamelCase_WithNull_ReturnsNull()
        {
            Assert.Null(StringHelper.GetCamelCase(null));
        }

        [Theory]
        [InlineData("TABLE_NAME", "TableName")]
        [InlineData("USER_ID", "UserId")]
        [InlineData("FIRST_NAME", "FirstName")]
        [InlineData("COLUMN_A_B", "ColumnAB")]
        public void GetCamelCase_WithUnderscoreSeparatedInput_ShouldReturnCamelCase(string input, string expected)
        {
            string result = StringHelper.GetCamelCase(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("USERNAME", "Username")]
        [InlineData("FIRSTNAME", "Firstname")]
        public void GetCamelCase_WithoutUnderscore_AllUpperCase_ShouldReturnCamelCase(string input, string expected)
        {
            string result = StringHelper.GetCamelCase(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("alreadyCamelCase", "alreadyCamelCase")]
        [InlineData("AlreadyPascalCase", "AlreadyPascalCase")]
        public void GetCamelCase_MixedCaseInput_ShouldReturnAsIs(string input, string expected)
        {
            string result = StringHelper.GetCamelCase(input);
            Assert.Equal(expected, result);
        }

        #endregion

        #region GetPascalCase Tests

        [Fact]
        public void GetPascalCase_WithNull_ReturnsNull()
        {
            Assert.Null(StringHelper.GetPascalCase(null));
        }

        [Theory]
        [InlineData("TABLE_NAME", "TableName")]
        [InlineData("USER_ID", "UserId")]
        [InlineData("FIRST_NAME", "FirstName")]
        public void GetPascalCase_WithUnderscoreSeparatedInput_ShouldReturnPascalCase(string input, string expected)
        {
            string result = StringHelper.GetPascalCase(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("USERNAME", "Username")]
        [InlineData("FIRSTNAME", "Firstname")]
        public void GetPascalCase_WithoutUnderscore_AllUpperCase_ShouldReturnPascalCase(string input, string expected)
        {
            string result = StringHelper.GetPascalCase(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("alreadyCamelCase", "alreadyCamelCase")]
        [InlineData("AlreadyPascalCase", "AlreadyPascalCase")]
        public void GetPascalCase_MixedCaseInput_ShouldReturnAsIs(string input, string expected)
        {
            string result = StringHelper.GetPascalCase(input);
            Assert.Equal(expected, result);
        }

        #endregion

        #region DbTypeToNetTypeString Tests

        [Theory]
        [InlineData(DbTypeEnum.Guid, "System.Guid")]
        [InlineData(DbTypeEnum.Boolean, "bool")]
        [InlineData(DbTypeEnum.Int16, "short")]
        [InlineData(DbTypeEnum.Int32, "int")]
        [InlineData(DbTypeEnum.Int64, "long")]
        [InlineData(DbTypeEnum.Decimal, "decimal")]
        [InlineData(DbTypeEnum.Single, "float")]
        [InlineData(DbTypeEnum.Double, "double")]
        [InlineData(DbTypeEnum.Binary, "byte[]")]
        [InlineData(DbTypeEnum.VarBinary, "byte[]")]
        [InlineData(DbTypeEnum.Image, "byte[]")]
        [InlineData(DbTypeEnum.Char, "string")]
        [InlineData(DbTypeEnum.VarChar, "string")]
        [InlineData(DbTypeEnum.Text, "string")]
        [InlineData(DbTypeEnum.NChar, "string")]
        [InlineData(DbTypeEnum.NVarChar, "string")]
        [InlineData(DbTypeEnum.NText, "string")]
        [InlineData(DbTypeEnum.Time, "System.TimeSpan")]
        [InlineData(DbTypeEnum.Date, "System.DateTime")]
        [InlineData(DbTypeEnum.Timestamp, "System.DateTime")]
        [InlineData(DbTypeEnum.DateTime, "System.DateTime")]
        [InlineData(DbTypeEnum.DateTime2, "System.DateTime")]
        [InlineData(DbTypeEnum.DateTimeOffset, "System.DateTime")]
        public void DbTypeToNetTypeString_ShouldReturnCorrectString(DbTypeEnum dbType, string expected)
        {
            string result = StringHelper.DbTypeToNetTypeString(dbType);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DbTypeToNetTypeString_UnknownEnumValue_ReturnsDefaultString()
        {
            // Cast an undefined value to DbTypeEnum
            DbTypeEnum unknownType = (DbTypeEnum)9999;
            string result = StringHelper.DbTypeToNetTypeString(unknownType);
            Assert.Equal("string", result);
        }

        #endregion

        #region DbTypeToNetType Tests

        [Theory]
        [InlineData(DbTypeEnum.Guid, typeof(System.Guid))]
        [InlineData(DbTypeEnum.Boolean, typeof(bool))]
        [InlineData(DbTypeEnum.Int16, typeof(short))]
        [InlineData(DbTypeEnum.Int32, typeof(int))]
        [InlineData(DbTypeEnum.Int64, typeof(long))]
        [InlineData(DbTypeEnum.Decimal, typeof(decimal))]
        [InlineData(DbTypeEnum.Single, typeof(float))]
        [InlineData(DbTypeEnum.Double, typeof(double))]
        [InlineData(DbTypeEnum.Binary, typeof(byte[]))]
        [InlineData(DbTypeEnum.VarBinary, typeof(byte[]))]
        [InlineData(DbTypeEnum.Image, typeof(byte[]))]
        [InlineData(DbTypeEnum.Char, typeof(string))]
        [InlineData(DbTypeEnum.VarChar, typeof(string))]
        [InlineData(DbTypeEnum.Text, typeof(string))]
        [InlineData(DbTypeEnum.NChar, typeof(string))]
        [InlineData(DbTypeEnum.NVarChar, typeof(string))]
        [InlineData(DbTypeEnum.NText, typeof(string))]
        [InlineData(DbTypeEnum.Time, typeof(System.TimeSpan))]
        [InlineData(DbTypeEnum.Date, typeof(System.DateTime))]
        [InlineData(DbTypeEnum.Timestamp, typeof(System.DateTime))]
        [InlineData(DbTypeEnum.DateTime, typeof(System.DateTime))]
        [InlineData(DbTypeEnum.DateTime2, typeof(System.DateTime))]
        [InlineData(DbTypeEnum.DateTimeOffset, typeof(System.DateTime))]
        public void DbTypeToNetType_ShouldReturnCorrectType(DbTypeEnum dbType, Type expected)
        {
            Type result = StringHelper.DbTypeToNetType(dbType);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DbTypeToNetType_UnknownEnumValue_ReturnsDefaultStringType()
        {
            DbTypeEnum unknownType = (DbTypeEnum)9999;
            Type result = StringHelper.DbTypeToNetType(unknownType);
            Assert.Equal(typeof(string), result);
        }

        #endregion
    }
}
