using Basic.Designer;
using Basic.Enums;
using System.ComponentModel;
using System.Globalization;

namespace StandardTest.BasicPersistent
{
    public class SupportDatabasesConverterTests
    {
        private readonly SupportDatabasesConverter _converter = new SupportDatabasesConverter();

        #region ConvertFrom Tests

        [Fact]
        public void ConvertFrom_WithEmptyString_ReturnsEmptyArray()
        {
            object result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "");
            Assert.NotNull(result);
            Assert.IsType<ConnectionTypes[]>(result);
            Assert.Empty((ConnectionTypes[])result);
        }

        [Fact]
        public void ConvertFrom_WithSingleValidType_ReturnsArrayWithOneElement()
        {
            object result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "SQLSERVER");
            Assert.NotNull(result);
            var array = Assert.IsType<ConnectionTypes[]>(result);
            Assert.Single(array);
            Assert.Equal(ConnectionTypes.SQLSERVER, array[0]);
        }

        [Fact]
        public void ConvertFrom_WithMultipleValidTypes_ReturnsArrayWithMultipleElements()
        {
            object result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "SQLSERVER,MYSQL,PGSQL");
            var array = Assert.IsType<ConnectionTypes[]>(result);
            Assert.Equal(3, array.Length);
            Assert.Contains(ConnectionTypes.SQLSERVER, array);
            Assert.Contains(ConnectionTypes.MYSQL, array);
            Assert.Contains(ConnectionTypes.PGSQL, array);
        }

        [Fact]
        public void ConvertFrom_WithNPGSQL_Alias_MapsToPGSQL()
        {
            object result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "NPGSQL");
            var array = Assert.IsType<ConnectionTypes[]>(result);
            Assert.Single(array);
            Assert.Equal(ConnectionTypes.PGSQL, array[0]);
        }

        [Fact]
        public void ConvertFrom_WithMixedValidAndInvalidTypes_IgnoresInvalid()
        {
            object result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "SQLSERVER,INVALIDTYPE,MYSQL");
            var array = Assert.IsType<ConnectionTypes[]>(result);
            Assert.Equal(2, array.Length);
            Assert.Contains(ConnectionTypes.SQLSERVER, array);
            Assert.Contains(ConnectionTypes.MYSQL, array);
        }

        [Fact]
        public void ConvertFrom_WithAllInvalidTypes_ReturnsEmptyArray()
        {
            object result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "INVALID1,INVALID2");
            Assert.NotNull(result);
            var array = Assert.IsType<ConnectionTypes[]>(result);
            Assert.Empty(array);
        }

        [Fact]
        public void ConvertFrom_WithNonStringValue_ReturnsBaseResult()
        {
            // Passing a non-string value should fall through to base
            object result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, 123);
            // The base TypeConverter will throw NotSupportedException for non-string input
            Assert.NotNull(result);
        }

        #endregion

        #region ConvertTo Tests

        [Fact]
        public void ConvertTo_WithEmptyArray_ReturnsEmptyString()
        {
            var input = Array.Empty<ConnectionTypes>();
            object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, input, typeof(string));
            Assert.Equal("", result);
        }

        [Fact]
        public void ConvertTo_WithSingleElement_ReturnsSingleTypeName()
        {
            var input = new ConnectionTypes[] { ConnectionTypes.SQLSERVER };
            object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, input, typeof(string));
            Assert.Equal("SQLSERVER", result);
        }

        [Fact]
        public void ConvertTo_WithMultipleElements_ReturnsCommaSeparatedNames()
        {
            var input = new ConnectionTypes[] { ConnectionTypes.SQLSERVER, ConnectionTypes.MYSQL, ConnectionTypes.PGSQL };
            object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, input, typeof(string));
            Assert.Equal("SQLSERVER,MYSQL,PGSQL", result);
        }

        [Fact]
        public void ConvertTo_WithDefaultType_ReturnsDefaultName()
        {
            var input = new ConnectionTypes[] { ConnectionTypes.Default };
            object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, input, typeof(string));
            Assert.Equal("Default", result);
        }

        [Fact]
        public void ConvertTo_WithNonArrayValue_ReturnsBaseResult()
        {
            // Passing a non-array value should fall through to base
            object result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, "some string", typeof(string));
            Assert.NotNull(result);
        }

        #endregion

        #region Round-Trip Tests

        [Fact]
        public void ConvertFromThenConvertTo_ShouldProduceOriginalString()
        {
            string original = "SQLSERVER,MYSQL,PGSQL";
            object fromResult = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, original);
            object toResult = _converter.ConvertTo(null, CultureInfo.InvariantCulture, fromResult, typeof(string));
            Assert.Equal(original, toResult);
        }

        [Fact]
        public void ConvertToThenConvertFrom_ShouldProduceOriginalArray()
        {
            var original = new ConnectionTypes[] { ConnectionTypes.SQLSERVER, ConnectionTypes.ORACLE, ConnectionTypes.MYSQL };
            object toResult = _converter.ConvertTo(null, CultureInfo.InvariantCulture, original, typeof(string));
            object fromResult = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, toResult);
            var resultArray = Assert.IsType<ConnectionTypes[]>(fromResult);
            Assert.Equal(original.Length, resultArray.Length);
            for (int i = 0; i < original.Length; i++)
                Assert.Equal(original[i], resultArray[i]);
        }

        #endregion
    }
}
