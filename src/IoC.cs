using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRLib
{
    public static class IoC
    {
        public static IKernel Container { get; private set; }

        public static T Get<T>()
        {
            return Container.Get<T>();
        }

        public static void Init()
        {
            Container = new StandardKernel();

            Container.Bind<Search.SearchEngine>().To<Search.DumbSearchEngine>().InSingletonScope();
        }
    }
}
