using System.Reflection;
using Abp.EntityFrameworkCore;
using Abp.GeneralTree;
using Abp.Modules;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore;

namespace RegionTree
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule), typeof(GeneralTreeModule))]
    public class RegionModule : AbpModule
    {
        /// <summary>
        ///     This is the first event called on application startup.
        ///     Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public override void PreInitialize()
        {
            /*
            var builder = new DbContextOptionsBuilder<RegionDbContext>();
            builder.UseInMemoryDatabase("region");
            Configuration.UnitOfWork.IsTransactional = false;
            */

            var builder = new DbContextOptionsBuilder<RegionDbContext>();
            builder.UseSqlServer("Server=.;Database=Region2;Trusted_Connection=True;");

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<RegionDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}