# v5.1.x 更新日志
### v5.1.2
#### New Features
1.  优化配置文件使用，使得配置文件读取一次即可复用
2.  增加全局路由前缀扩展方法
3.  新增获取Logger服务的扩展
#### Fix Bugs
1.  修复StopWatch因并发导致的异常
2.  修复DistinctBy失效
#### Optimization
1.  调整雪花ID生成器使用时间为UTC时间
2.  移除IEnumerableExtension中未验证的扩展
3.  对某些获取服务的扩展方法名进行调整
4.  调整Pinyin相关实现
---
### v5.1.1
本次更新将原有的ORM迁移到SqlSugar，将TaskSchedule迁移到Quartz。并在服务配置、初始化等方面进行了比较大的优化和调整。
#### New Features
1.  移除ORM、TaskSchedule库相关，并新增SqlSugar、Quartz库相关
2.  SqlSugar支持运行时修改配置
3.  定义环境类型
4.  新增雪花ID生成器注入
5.  支持任务调度的依赖注入
6.  新增动态模块导入
#### Optimization
1.  优化IConfiguration扩展
2.  调整Web初始化过程
3.  调整RabbitMQ初始化过程
4.  调整缓存插件注入方式
5.  调整版本号生成规则
6.  合并Core与Plugins
7.  移除仓储类中事务调用，防止事务嵌套
8.  移除字符串生成器（表字符串）