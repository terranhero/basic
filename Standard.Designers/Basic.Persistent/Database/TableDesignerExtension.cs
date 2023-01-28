using Basic.DataEntities;
using Basic.Configuration;
using Basic.Enums;
using System.Text;
using Basic.EntityLayer;
using System.Data;
using Basic.Collections;
using System.Collections.Generic;

namespace Basic.Database
{
    /// <summary>
    /// 数据库表列信息
    /// </summary>
    internal static class TableDesignerExtension
    {
        /// <summary>
        /// 将 DesignTableInfo 对象的内容复制到当前实例。
        /// </summary>
        /// <param name="entity">标识需要复制的 DesignTableInfo 类实例。</param>
        public static void CopyFrom(this TableDesignerInfo table, DataEntityElement entity)
        {
            table.Name = entity.TableName;
            table.Common = entity.Comment;
            table.Owner = "dbo";
            table.Columns.Clear();
            foreach (DataEntityPropertyElement property in entity.Properties)
            {
                ColumnDesignerInfo columnInfo = table.Columns.CreateColumn();
                columnInfo.CopyFrom(property);
                table.Columns.Add(columnInfo);
            }
        }

        /// <summary>
        /// 根据数据库列信息，创建查询数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        public static string CreateSelectCommand(this TableDesignerInfo table)
        {
            if (table.Columns == null || table.Columns.Count == 0) { return null; }
            if (table.Tables.CheckedColumns.Count == 0) { return null; }
            StringBuilder selectBuilder = new StringBuilder("SELECT  ", 500);
            //selectBuilder.Append(table.Name).Append(" SET ");
            StringBuilder whereBuilder = new StringBuilder("WHERE ", 500);
            int selectLength = selectBuilder.Length;
            int whereLength = whereBuilder.Length;
            foreach (ColumnDesignerInfo column in table.Columns)
            {
                if (column.Computed || !column.Checked) { continue; }
                if (column.IsWhere)
                {
                    if (whereLength == whereBuilder.Length)
                        whereBuilder.AppendFormat("{0}={{%{0}%}}", column.Name);
                    else
                        whereBuilder.AppendFormat(" AND {0}={{%{0}%}}", column.Name);
                }
                else
                {
                    if (selectLength == selectBuilder.Length)
                    {
                        if (column.CanUseDefault && column.UseDefault)
                            selectBuilder.Append(column.DefaultValue).Append(" AS ").Append(column.Name);
                        else
                            selectBuilder.Append(column.Name);
                    }
                    else
                    {
                        if (column.CanUseDefault && column.UseDefault)
                            selectBuilder.Append(", ").Append(column.DefaultValue).Append(" AS ").Append(column.Name);
                        else
                            selectBuilder.Append(", ").Append(column.Name);
                    }
                }
            }
            selectBuilder.AppendLine("").Append("FROM ").Append(table.Name);
            if (whereBuilder.Length > whereLength)
            {
                selectBuilder.AppendLine("");
                selectBuilder.Append(whereBuilder.ToString());
            }
            return selectBuilder.ToString();
        }

        /// <summary>
        /// 根据数据库列信息，创建查询数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        /// <param name="staticCommand"></param>
        public static bool CreateSelectParameter(this TableDesignerInfo table, DataEntityElement entity, StaticCommandElement staticCommand)
        {
            if (table.Columns == null || table.Columns.Count == 0) { return false; }
            if (table.Tables.CheckedColumns.Count == 0) { return false; }
            bool propertyNotExist = entity.Properties.Count <= 0;
            foreach (ColumnDesignerInfo column in table.Tables.CheckedColumns)
            {
                if (!column.IsWhere) { continue; }
                if (propertyNotExist)
                {
                    DataEntityPropertyElement property = entity.CreateProperty(column.Name);
                    column.CreateProperty(property);
                    entity.Properties.Add(property);
                }
                CommandParameter parameter = new CommandParameter(staticCommand);
                parameter.Name = column.Name;
                parameter.SourceColumn = column.Name;
                parameter.ParameterType = column.DbType;
                switch (column.DbType)
                {
                    case DbTypeEnum.Decimal:
                        parameter.Precision = column.Precision;
                        parameter.Scale = (byte)column.Scale;
                        parameter.Size = 0;
                        break;
                    case DbTypeEnum.NChar:
                    case DbTypeEnum.NVarChar:
                    case DbTypeEnum.Char:
                    case DbTypeEnum.VarChar:
                    case DbTypeEnum.Binary:
                    case DbTypeEnum.VarBinary:
                        parameter.Precision = 0;
                        parameter.Scale = 0;
                        parameter.Size = column.Size;
                        break;
                    default:
                        parameter.Precision = 0;
                        parameter.Scale = 0;
                        parameter.Size = 0;
                        break;
                }
                parameter.Direction = ParameterDirection.Input;
                parameter.Nullable = column.Nullable;
                staticCommand.Parameters.Add(parameter);
            }
            return true;
        }

        /// <summary>
        /// 根据数据库列信息，创建新增数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        public static string CreateInsertCommand(this TableDesignerInfo table)
        {
            if (table.Columns == null || table.Columns.Count == 0) { return null; }
            if (table.Tables.CheckedColumns.Count == 0) { return null; }
            StringBuilder insertBuilder = new StringBuilder("INSERT INTO ", 500);
            insertBuilder.Append(table.Name).Append("(");
            StringBuilder valueBuilder = new StringBuilder("VALUES(", 500);
            int curLength = insertBuilder.Length;
            foreach (ColumnDesignerInfo column in table.Columns)
            {
                if (column.Computed || !column.Checked) { continue; }
                if (curLength == insertBuilder.Length)
                {
                    insertBuilder.Append(column.Name);
                    if (column.CanUseDefault && column.UseDefault)
                        valueBuilder.Append(column.DefaultValue);
                    else
                        valueBuilder.Append("{%").Append(column.Name).Append("%}");
                }
                else
                {
                    insertBuilder.Append(", ").Append(column.Name);
                    if (column.CanUseDefault && column.UseDefault)
                        valueBuilder.Append(", ").Append(column.DefaultValue);
                    else
                        valueBuilder.Append(", {%").Append(column.Name).Append("%}");
                }
            }
            insertBuilder.AppendLine(")"); valueBuilder.Append(")");
            insertBuilder.Append(valueBuilder.ToString());
            return insertBuilder.ToString();
        }

        /// <summary>
        /// 根据数据库列信息，创建新增数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        /// <param name="staticCommand"></param>
        public static bool CreateInsertParameter(this TableDesignerInfo table, DataEntityElement entity, StaticCommandElement staticCommand)
        {
            if (table.Columns == null || table.Columns.Count == 0) { return false; }
            if (table.Tables.CheckedColumns.Count == 0) { return false; }
            bool propertyNotExist = entity.Properties.Count <= 0;
            foreach (ColumnDesignerInfo column in table.Tables.CheckedColumns)
            {
                if (column.Computed || (column.CanUseDefault && column.UseDefault)) { continue; }
                if (propertyNotExist)
                {
                    DataEntityPropertyElement property = entity.CreateProperty(column.Name);
                    column.CreateProperty(property);
                    entity.Properties.Add(property);
                }
                CommandParameter parameter = new CommandParameter(staticCommand);
                parameter.Name = column.Name;
                parameter.SourceColumn = column.Name;
                parameter.ParameterType = column.DbType;
                switch (column.DbType)
                {
                    case DbTypeEnum.Decimal:
                        parameter.Precision = column.Precision;
                        parameter.Scale = (byte)column.Scale;
                        parameter.Size = 0;
                        break;
                    case DbTypeEnum.NChar:
                    case DbTypeEnum.NVarChar:
                    case DbTypeEnum.Char:
                    case DbTypeEnum.VarChar:
                    case DbTypeEnum.Binary:
                    case DbTypeEnum.VarBinary:
                        parameter.Precision = 0;
                        parameter.Scale = 0;
                        parameter.Size = column.Size;
                        break;
                    default:
                        parameter.Precision = 0;
                        parameter.Scale = 0;
                        parameter.Size = 0;
                        break;
                }
                parameter.Direction = ParameterDirection.Input;
                parameter.Nullable = column.Nullable;
                staticCommand.Parameters.Add(parameter);
            }
            return true;
        }

        /// <summary>
        /// 根据数据库列信息，创建修改数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        public static string CreateUpdateCommand(this TableDesignerInfo table)
        {
            if (table.Columns == null || table.Columns.Count == 0) { return null; }
            if (table.Tables.CheckedColumns.Count == 0) { return null; }
            StringBuilder updateBuilder = new StringBuilder("UPDATE  ", 500);
            updateBuilder.Append(table.Name).Append(" SET ");
            StringBuilder whereBuilder = new StringBuilder("WHERE ", 500);
            int updateLength = updateBuilder.Length;
            int whereLength = whereBuilder.Length;
            foreach (ColumnDesignerInfo column in table.Columns)
            {
                if (column.Computed || !column.Checked) { continue; }
                if (column.IsWhere)
                {
                    if (whereLength == whereBuilder.Length)
                        whereBuilder.AppendFormat("{0}={{%{0}%}}", column.Name);
                    else
                        whereBuilder.AppendFormat(" AND {0}={{%{0}%}}", column.Name);
                }
                else
                {
                    if (updateLength == updateBuilder.Length)
                    {
                        if (column.CanUseDefault && column.UseDefault)
                            updateBuilder.AppendFormat("{0}={1}", column.Name, column.DefaultValue);
                        else
                            updateBuilder.AppendFormat("{0}={{%{0}%}}", column.Name);
                    }
                    else
                    {
                        if (column.CanUseDefault && column.UseDefault)
                            updateBuilder.AppendFormat(", {0}={1}", column.Name, column.DefaultValue);
                        else
                            updateBuilder.AppendFormat(", {0}={{%{0}%}}", column.Name);
                    }
                }
            }
            if (whereBuilder.Length > whereLength)
            {
                updateBuilder.AppendLine("");
                updateBuilder.Append(whereBuilder.ToString());
            }
            return updateBuilder.ToString();
        }

        /// <summary>
        /// 根据数据库列信息，创建修改数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        /// <param name="staticCommand"></param>
        public static bool CreateUpdateParameter(this TableDesignerInfo table, DataEntityElement entity, StaticCommandElement staticCommand)
        {
            if (table.Columns == null || table.Columns.Count == 0) { return false; }
            if (table.Tables.CheckedColumns.Count == 0) { return false; }
            bool propertyNotExist = entity.Properties.Count <= 0;
            foreach (ColumnDesignerInfo column in table.Tables.CheckedColumns)
            {
                if ((!column.IsWhere) && (column.Computed || (column.CanUseDefault && column.UseDefault))) { continue; }
                if (propertyNotExist)
                {
                    DataEntityPropertyElement property = entity.CreateProperty(column.Name);
                    column.CreateProperty(property);
                    entity.Properties.Add(property);
                }
                CommandParameter parameter = new CommandParameter(staticCommand);
                parameter.Name = column.Name;
                parameter.SourceColumn = column.Name;
                parameter.ParameterType = column.DbType;
                switch (column.DbType)
                {
                    case DbTypeEnum.Decimal:
                        parameter.Precision = column.Precision;
                        parameter.Scale = (byte)column.Scale;
                        parameter.Size = 0;
                        break;
                    case DbTypeEnum.NChar:
                    case DbTypeEnum.NVarChar:
                    case DbTypeEnum.Char:
                    case DbTypeEnum.VarChar:
                    case DbTypeEnum.Binary:
                    case DbTypeEnum.VarBinary:
                        parameter.Precision = 0;
                        parameter.Scale = 0;
                        parameter.Size = column.Size;
                        break;
                    default:
                        parameter.Precision = 0;
                        parameter.Scale = 0;
                        parameter.Size = 0;
                        break;
                }
                parameter.Direction = ParameterDirection.Input;
                parameter.Nullable = column.Nullable;
                staticCommand.Parameters.Add(parameter);
            }
            return true;
        }

        /// <summary>
        /// 根据数据库列信息，创建删除数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        public static string CreateDeleteCommand(this TableDesignerInfo table)
        {
            if (table.Columns == null || table.Columns.Count == 0) { return null; }
            if (table.Tables.CheckedColumns.Count == 0) { return null; }
            StringBuilder deleteBuilder = new StringBuilder("DELETE FROM ", 500);
            deleteBuilder.Append(table.Name).Append(" WHERE ");
            int deleteLength = deleteBuilder.Length;
            foreach (ColumnDesignerInfo column in table.Columns)
            {
                if (!column.Checked) { continue; }
                if (deleteLength == deleteBuilder.Length)
                    deleteBuilder.AppendFormat("{0}={{%{0}%}}", column.Name);
                else
                    deleteBuilder.AppendFormat(" AND {0}={{%{0}%}}", column.Name);
            }
            return deleteBuilder.ToString();
        }

        /// <summary>
        /// 根据数据库列信息，创建删除数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        /// <param name="staticCommand"></param>
        public static bool CreateDeleteParameter(this TableDesignerInfo table, DataEntityElement entity, StaticCommandElement staticCommand)
        {
            if (table.Columns == null || table.Columns.Count == 0) { return false; }
            if (table.Tables.CheckedColumns.Count == 0) { return false; }
            bool propertyNotExist = entity.Properties.Count <= 0;
            foreach (ColumnDesignerInfo column in table.Tables.CheckedColumns)
            {
                if (propertyNotExist)
                {
                    DataEntityPropertyElement property = entity.CreateProperty(column.Name);
                    column.CreateProperty(property);
                    entity.Properties.Add(property);
                }
                CommandParameter parameter = new CommandParameter(staticCommand);
                parameter.Name = column.Name;
                parameter.SourceColumn = column.Name;
                parameter.ParameterType = column.DbType;
                switch (column.DbType)
                {
                    case DbTypeEnum.Decimal:
                        parameter.Precision = column.Precision;
                        parameter.Scale = (byte)column.Scale;
                        parameter.Size = 0;
                        break;
                    case DbTypeEnum.NChar:
                    case DbTypeEnum.NVarChar:
                    case DbTypeEnum.Char:
                    case DbTypeEnum.VarChar:
                    case DbTypeEnum.Binary:
                    case DbTypeEnum.VarBinary:
                        parameter.Precision = 0;
                        parameter.Scale = 0;
                        parameter.Size = column.Size;
                        break;
                    default:
                        parameter.Precision = 0;
                        parameter.Scale = 0;
                        parameter.Size = 0;
                        break;
                }
                parameter.Direction = ParameterDirection.Input;
                parameter.Nullable = column.Nullable;
                staticCommand.Parameters.Add(parameter);
            }
            return true;
        }

        private static void CreateSelectCommand(ColumnDesignerInfo column, List<string> selectList,
        StringBuilder fromBuilder, List<string> whereList, List<string> orderList)
        {
            if (!column.Checked) { return; }
            if (column.Selected)
                selectList.Add(string.Concat(column.Alias, ".", column.Name));
            if (column.IsFrom && column.Table.Tables.Relations.Count > 0)
                fromBuilder.Append(" AND ").Append(column.SelectName).Append("={%").Append(column.Name).Append("%}");
            if (column.IsWhere)
                whereList.Add(string.Concat(column.SelectName, "={%", column.Name, "%}"));
            if (column.SortOrder == OrderEnum.Ascending)
                orderList.Add(column.SelectName);
            else if (column.SortOrder == OrderEnum.Descending)
                orderList.Add(string.Concat(column.SelectName, " DESC"));
        }

        /// <summary>
        /// 根据数据库列信息，创建新增数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        public static bool CreateSelectCommand(this TableDesignerCollection tables, DynamicCommandElement dynamicCommand)
        {
            if (tables.CheckedColumns.Count == 0) { return false; }
            if (tables.Group) { return tables.CreateGroupCommand(dynamicCommand); }
            CheckedColumnCollection outputColumns = new CheckedColumnCollection(tables);
            List<string> selectList = new List<string>(20);
            StringBuilder fromBuilder = new StringBuilder(500);
            List<string> whereList = new List<string>(20);
            List<string> orderList = new List<string>(20);

            if (tables.Relations.Count == 0)
            {
                foreach (TableDesignerInfo table in tables)
                {
                    if (fromBuilder.Length == 5)
                        fromBuilder.Append(table.FromName);
                    else
                        fromBuilder.Append(",").Append(table.FromName);

                    foreach (ColumnDesignerInfo column in table.Columns)
                    {
                        CreateSelectCommand(column, selectList, fromBuilder, whereList, orderList);
                    }
                }
            }
            else
            {
                foreach (RelationDesignerInfo relation in tables.Relations)
                {
                    fromBuilder.Append(relation.JoinTable());
                    foreach (ColumnDesignerInfo column in relation.Parent.Columns)
                    {
                        if (outputColumns.Contains(column)) { continue; }
                        outputColumns.Add(column);
                        CreateSelectCommand(column, selectList, fromBuilder, whereList, orderList);
                    }
                    foreach (ColumnDesignerInfo column in relation.Child.Columns)
                    {
                        if (outputColumns.Contains(column)) { continue; }
                        outputColumns.Add(column);
                        CreateSelectCommand(column, selectList, fromBuilder, whereList, orderList);
                    }
                }
            }
            dynamicCommand.SelectText = string.Join(",", selectList.ToArray());
            dynamicCommand.FromText = fromBuilder.ToString();
            if (whereList.Count > 0)
                dynamicCommand.WhereText = string.Join(" AND ", whereList.ToArray());
            if (orderList.Count > 0)
                dynamicCommand.OrderText = string.Join(",", orderList.ToArray());

            return true;
        }

        /// <summary>
        /// 创建普通查询的 SELECT 语句
        /// </summary>
        /// <param name="table">数据库表信息</param>
        public static string CreateSelectCommand(this TableDesignerCollection tables)
        {
            if (tables.CheckedColumns.Count == 0) { return null; }
            if (tables.Group) { return tables.CreateGroupCommand(); }
            CheckedColumnCollection outputColumns = new CheckedColumnCollection(tables);
            List<string> selectList = new List<string>(20);
            StringBuilder fromBuilder = new StringBuilder("FROM ", 500);
            List<string> whereList = new List<string>(20);
            List<string> orderList = new List<string>(20);

            if (tables.Relations.Count == 0)
            {
                foreach (TableDesignerInfo table in tables)
                {
                    if (fromBuilder.Length == 5)
                        fromBuilder.Append(table.FromName);
                    else
                        fromBuilder.Append(",").Append(table.FromName);

                    foreach (ColumnDesignerInfo column in table.Columns)
                    {
                        CreateSelectCommand(column, selectList, fromBuilder, whereList, orderList);
                    }
                }
            }
            else
            {
                foreach (RelationDesignerInfo relation in tables.Relations)
                {
                    fromBuilder.Append(relation.JoinTable());
                    foreach (ColumnDesignerInfo column in relation.Parent.Columns)
                    {
                        if (outputColumns.Contains(column)) { continue; }
                        outputColumns.Add(column);
                        CreateSelectCommand(column, selectList, fromBuilder, whereList, orderList);
                    }
                    foreach (ColumnDesignerInfo column in relation.Child.Columns)
                    {
                        if (outputColumns.Contains(column)) { continue; }
                        outputColumns.Add(column);
                        CreateSelectCommand(column, selectList, fromBuilder, whereList, orderList);
                    }
                }
            }
            StringBuilder sqlBuilder = new StringBuilder("SELECT ", 500);
            sqlBuilder.Append(string.Join(",", selectList.ToArray())).AppendLine().Append(fromBuilder);

            if (whereList.Count > 0)
                sqlBuilder.AppendLine().Append("WHERE ").Append(string.Join(" AND ", whereList.ToArray()));
            if (orderList.Count > 0)
                sqlBuilder.AppendLine().Append("ORDER BY ").Append(string.Join(",", orderList.ToArray()));
            return sqlBuilder.ToString();
        }

        private static void CreateGroupCommand(ColumnDesignerInfo column, List<string> selectList,
        StringBuilder fromBuilder, List<string> groupList, List<string> havingList, List<string> orderList)
        {
            if (!column.Checked) { return; }
            if (!column.HasAggregate)
                groupList.Add(column.GroupName);
            if (column.Selected)
                selectList.Add(column.GroupName);
            if (column.IsFrom && column.Table.Tables.Relations.Count > 0)
                fromBuilder.Append(" AND ").Append(column.SelectName).Append("={%").Append(column.Name).Append("%}");
            if (column.IsWhere)
                havingList.Add(string.Concat(column.AggregateName, "={%", column.Name, "%}"));
            if (column.SortOrder == OrderEnum.Ascending)
            {
                if (column.HasAggregate)
                    orderList.Add(column.GroupName);
                else
                    orderList.Add(column.SelectName);
            }
            else if (column.SortOrder == OrderEnum.Descending)
            {
                if (column.HasAggregate)
                    orderList.Add(string.Concat(column.GroupName, " DESC"));
                else
                    orderList.Add(string.Concat(column.SelectName, " DESC"));
            }
        }

        /// <summary>
        /// 创建分组查询的 SELECT 语句。
        /// </summary>
        /// <param name="table">数据库表信息</param>
        private static string CreateGroupCommand(this TableDesignerCollection tables)
        {
            if (tables.CheckedColumns.Count == 0) { return null; }
            CheckedColumnCollection outputColumns = new CheckedColumnCollection(tables);
            List<string> selectList = new List<string>(20);
            StringBuilder fromBuilder = new StringBuilder("FROM ", 500);

            List<string> groupList = new List<string>(20);
            List<string> havingList = new List<string>(20);
            List<string> orderList = new List<string>(20);

            if (tables.Relations.Count == 0)
            {
                foreach (TableDesignerInfo table in tables)
                {
                    if (fromBuilder.Length == 5)
                        fromBuilder.Append(table.FromName);
                    else
                        fromBuilder.Append(",").Append(table.FromName);

                    foreach (ColumnDesignerInfo column in table.Columns)
                    {
                        CreateGroupCommand(column, selectList, fromBuilder, groupList, havingList, orderList);
                    }
                }
            }
            else
            {
                foreach (RelationDesignerInfo relation in tables.Relations)
                {
                    fromBuilder.Append(relation.JoinTable());
                    foreach (ColumnDesignerInfo column in relation.Parent.Columns)
                    {
                        if (outputColumns.Contains(column)) { continue; }
                        outputColumns.Add(column);
                        CreateGroupCommand(column, selectList, fromBuilder, groupList, havingList, orderList);
                    }
                    foreach (ColumnDesignerInfo column in relation.Child.Columns)
                    {
                        if (outputColumns.Contains(column)) { continue; }
                        outputColumns.Add(column);
                        CreateGroupCommand(column, selectList, fromBuilder, groupList, havingList, orderList);
                    }
                }
            }
            StringBuilder sqlBuilder = new StringBuilder("SELECT ", 1000);
            sqlBuilder.Append(string.Join(",", selectList.ToArray())).AppendLine().Append(fromBuilder);

            if (groupList.Count > 0)
                sqlBuilder.AppendLine().Append("GROUP BY ").Append(string.Join(",", groupList.ToArray()));
            if (havingList.Count > 0)
                sqlBuilder.AppendLine().Append("HAVING ").Append(string.Join(" AND ", havingList.ToArray()));
            if (orderList.Count > 0)
                sqlBuilder.AppendLine().Append("ORDER BY ").Append(string.Join(",", orderList.ToArray()));
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 根据数据库列信息，创建新增数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        private static bool CreateGroupCommand(this TableDesignerCollection tables, DynamicCommandElement dynamicCommand)
        {
            if (tables.CheckedColumns.Count == 0) { return false; }
            CheckedColumnCollection outputColumns = new CheckedColumnCollection(tables);
            List<string> selectList = new List<string>(20);
            StringBuilder fromBuilder = new StringBuilder("FROM ", 500);
            List<string> groupList = new List<string>(20);
            List<string> havingList = new List<string>(20);
            List<string> orderList = new List<string>(20);
            if (tables.Relations.Count == 0)
            {
                foreach (TableDesignerInfo table in tables)
                {
                    if (fromBuilder.Length == 5)
                        fromBuilder.Append(table.FromName);
                    else
                        fromBuilder.Append(",").Append(table.FromName);

                    foreach (ColumnDesignerInfo column in table.Columns)
                    {
                        CreateGroupCommand(column, selectList, fromBuilder, groupList, havingList, orderList);
                    }
                }
            }
            else
            {
                foreach (RelationDesignerInfo relation in tables.Relations)
                {
                    fromBuilder.Append(relation.JoinTable());
                    foreach (ColumnDesignerInfo column in relation.Parent.Columns)
                    {
                        if (outputColumns.Contains(column)) { continue; }
                        outputColumns.Add(column);
                        CreateGroupCommand(column, selectList, fromBuilder, groupList, havingList, orderList);
                    }
                    foreach (ColumnDesignerInfo column in relation.Child.Columns)
                    {
                        if (outputColumns.Contains(column)) { continue; }
                        outputColumns.Add(column);
                        CreateGroupCommand(column, selectList, fromBuilder, groupList, havingList, orderList);
                    }
                }
            }

            dynamicCommand.SelectText = string.Join(",", selectList.ToArray());
            dynamicCommand.FromText = fromBuilder.ToString();
            if (groupList.Count > 0)
                dynamicCommand.GroupText = string.Join(",", groupList.ToArray());
            if (havingList.Count > 0)
                dynamicCommand.HavingText = string.Join(" AND ", havingList.ToArray());
            if (orderList.Count > 0)
                dynamicCommand.OrderText = string.Join(",", orderList.ToArray());


            return true;
        }

        /// <summary>
        /// 根据数据库列信息，创建新增数据库命令及其参数
        /// </summary>
        /// <param name="table">数据库表信息</param>
        /// <param name="dynamicCommand"></param>
        public static bool CreateSelectParameter(this TableDesignerCollection tables, DataEntityElement entity, DynamicCommandElement dynamicCommand)
        {
            if (tables.CheckedColumns.Count == 0) { return false; }
            bool propertyNotExist = entity.Properties.Count <= 0;

            foreach (TableDesignerInfo tableInfo in tables)
            {
                bool isSelect = false;
                foreach (ColumnDesignerInfo column in tableInfo.Columns)
                {
                    if (!column.Checked) { continue; }
                    isSelect = true;
                    if (propertyNotExist)
                    {
                        DataEntityPropertyElement property = entity.CreateProperty(column.Name);
                        column.CreateProperty(property);
                        entity.Properties.Add(property);
                    }
                    if (!column.IsWhere && !column.IsFrom) { continue; }
                    DataConditionPropertyElement conProperty;
                    if (!entity.Condition.Arguments.TryGetValue(column.Name, out conProperty))
                    {
                        conProperty = new DataConditionPropertyElement(entity.Condition);
                        column.CreateProperty(conProperty);
                        entity.Condition.Arguments.Add(conProperty);
                    }
                    if (!dynamicCommand.Parameters.ContainsKey(column.Name))
                    {
                        CommandParameter parameter = new CommandParameter(dynamicCommand);
                        column.CreateParameter(parameter);
                        dynamicCommand.Parameters.Add(parameter);
                    }
                }
                if (tableInfo is TableFunctionInfo && isSelect)
                {
                    TableFunctionInfo functionInfo = tableInfo as TableFunctionInfo;
                    foreach (FunctionParameterInfo parameterInfo in functionInfo.Parameters)
                    {
                        DataConditionPropertyElement conProperty;
                        if (!entity.Condition.Arguments.TryGetValue(parameterInfo.Name, out conProperty))
                        {
                            conProperty = new DataConditionPropertyElement(entity.Condition);
                            parameterInfo.CreateProperty(conProperty);
                            entity.Condition.Arguments.Add(conProperty);
                        }
                        if (!dynamicCommand.Parameters.ContainsKey(parameterInfo.Name))
                        {
                            CommandParameter parameter = new CommandParameter(dynamicCommand);
                            parameterInfo.CreateParameter(parameter);
                            dynamicCommand.Parameters.Add(parameter);
                        }
                    }
                }
            }
            return true;
        }
    }
}