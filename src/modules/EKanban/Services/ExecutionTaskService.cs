using System.Linq;
using System;
/*
 *Author：jxx
 *Contact：283591387@qq.com
 *代码由框架生成,此处任何更改都可能导致被代码生成器覆盖
 *所有业务编写全部应在Partial文件夹下ExecutionTaskService与IExecutionTaskService中编写
 */
using EKanban.IRepositories;
using EKanban.IServices;
using VOL.Core.BaseProvider;
using VOL.Core.Extensions.AutofacManager;
using VOL.Entity.DomainModels;

namespace EKanban.Services
{
    public partial class ExecutionTaskService : ServiceBase<ExecutionTask, IExecutionTaskRepository>
    , IExecutionTaskService, IDependency
    {
        public ExecutionTaskService(IExecutionTaskRepository repository)
        : base(repository)
        {
            Init(repository);
        }
        public static IExecutionTaskService Instance
        {
            get { return AutofacContainerModule.GetService<IExecutionTaskService>(); }
        }
    }
}
