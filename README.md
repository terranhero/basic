## basic ORM Framework & Designer

basic ORM 于2012 年发布、2023 年正式开源，该组件已在数百个成熟项目中应用。

经过数十个版本的更新迭代发布全新v6.0版本，支持动态条件查询（Lambda）

数据库持久化 ORM 开发框架，支持多种数据库(MSSQL,MYSQL,ORACLE,DB2,PostgreSQL等)。

支持Linq表达式动态查询数据。支持快速分页

简单的事务处理，自动化事务提交，回滚

### 设计器文件目录

 - 设计器文件(*.dpdl)
    - 数据持久类代码(xxxAccess.cs)
    - 数据持久类代码(xxxAccess.designer.cs)
    - 业务逻辑类代码(xxxContext.cs)
    - 业务逻辑类代码(xxxContext.designer.cs)

![添加持久类截图](https://foruda.gitee.com/images/1675321661518283746/d0f9f8f5_665445.png "append.png")

### NuGet packages
| 包名称 | 包说明 | 状态 |
| ---- | ------- | ------ |
| Standard.EntityLayer | 实体模型包 | [![NuGet version](https://badge.fury.io/nu/Standard.EntityLayer.svg)](https://badge.fury.io/nu/Standard.EntityLayer) |
| Standard.DataAccess | 基础数据库持久包 | [![NuGet version](https://badge.fury.io/nu/Standard.DataAccess.svg)](https://badge.fury.io/nu/Standard.DataAccess) |
| Standard.SqlClientAccess | SQL Server 数据库持久类支持包 | [![NuGet version](https://badge.fury.io/nu/Standard.SqlClientAccess.svg)](https://badge.fury.io/nu/Standard.SqlClientAccess) |
| Standard.MySqlAccess  | MySql 数据库持久类支持包 | [![NuGet version](https://badge.fury.io/nu/Standard.MySqlAccess.svg)](https://badge.fury.io/nu/Standard.MySqlAccess) |
| Standard.OracleAccess  | Oracle 数据库持久类支持包 | [![NuGet version](https://badge.fury.io/nu/Standard.OracleAccess.svg)](https://badge.fury.io/nu/Standard.OracleAccess) |
| Standard.PostgreAccess  | PostgreSQL 数据库持久类支持包 | [![NuGet version](https://badge.fury.io/nu/Standard.PostgreAccess.svg)](https://badge.fury.io/nu/Standard.PostgreAccess) |

### 不使用分布式事务
```c#
using(xxxAccess access = new xxxAccess(connectionstring))
{
   access.Create(entity); or await access.CreateAsync(entity);
}

//使用 Lambda 表达式查询数据
using(xxxAccess access = new xxxAccess(connectionstring))
{
    var queries = access.GetEntities<XXX>(0,0);
    queries.Where(m => m.Enabled == true).Where(m => m.Key >= 1);
    return queries.ToPaginationAsync();
}
```

### 使用分布式事务(xxxContext.cs)
```c#
using(xxxAccess access = new xxxAccess(connectionstring, true))
{   
    access.Create(entity); or await access.CreateAsync(entity);
    access.SetComplate();
}

using(xxxAccess access = new xxxAccess(connectionstring, TimeSpan.FromSeconds(60)))
{   
    access.Create(entity); or await access.CreateAsync(entity);
    access.SetComplate();
}
```