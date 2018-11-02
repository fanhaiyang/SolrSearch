# SolrSearch
一：VS2017 15.8.8以上版本 
1. 创建Asp.net Core Web应用程序
2. 框架选 .Net Framework  &  .NetCore;API模板

二： VS2017 15.8.8以下版本
1. 创建Asp.net Core Web应用程序
2. .NetCore框架
3. 建好工程后手动修改成 .Net461框架

三： 用到的包：
```csharp
<PackageReference Include="log4net" Version="1.2.10" />
    <PackageReference Include="Microsoft.AspNetCore" Version="2.1.4" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc" Version="2.1.3" />
    <PackageReference Include="Microsoft.AspNetCore.StaticFiles" Version="2.1.1" />
    <PackageReference Include="SolrNet" Version="1.0.13" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="3.0.0" />
```
四：配置swagger，创建restful接口,添加日志
