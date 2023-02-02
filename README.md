## basic ORM Framework & Designer

basic ORM 于2012 年发布、2023 年正式开源，该组件已在数百个成熟项目中应用。

经过数十个版本的更新迭代发布全新v6.0版本，支持动态条件查询（Lambda）

数据库持久化 ORM 开发框架，支持多种数据库(MSSQL,MYSQL,ORACLE,DB2等)。

支持Linq表达式动态查询数据。支持快速分页

简单的事务处理，自动化事务提交，回滚

# using(xxxxxAccess access = new xxxxxAccess(connectionstring, true))
{
         access.SetComplate();

}