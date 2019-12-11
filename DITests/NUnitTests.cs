using System;
using System.Collections;
using System.Collections.Generic;
using DIContainer;
using NUnit.Framework;

namespace DITests
{
    public class Tests
    {
        private DependenciesConfiguration config;
        [SetUp]
        public void SetUp()
        {
            config = new DependenciesConfiguration();
        }
        
        [Test]
        public void SingletonTest()
        {
            config.Register<IExample, ClassForExample>(true);
            var provider = new DependencyProvider(config);
            IExample expected = provider.Resolve<IExample>();
            IExample actual = provider.Resolve<IExample>();

            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void InstancePerDependencyTest()
        {
            config.Register<IExample, ClassForExample>();
            var provider = new DependencyProvider(config);
            IExample expected = provider.Resolve<IExample>();
            IExample actual = provider.Resolve<IExample>();

            Assert.AreNotEqual(expected, actual);
        }      
        
        [Test]
        public void AsSelfRegistrationTest()
        {
            config.Register<ClassForExample, ClassForExample>(true);
            var provider = new DependencyProvider(config);
            ClassForExample actual = provider.Resolve<ClassForExample>();

            Assert.IsNotNull(actual);
        }
        
        [Test]
        public void GetSomeImplementationsTest()
        {
            config.Register<IExample, ClassForExample>(true);
            config.Register<IExample, ClassForExample2>();
            var provider = new DependencyProvider(config);
            IEnumerable<IExample> actual = provider.Resolve<IEnumerable<IExample>>();

            Assert.IsNotNull(actual);
            Assert.AreEqual(2, ((IList)actual).Count);
        }
        
        [Test]
        public void ImplementationIsInterfaceTest()
        {
            config.Register<IExample, IExample>(true);
            Assert.Throws(typeof(ArgumentException), () =>
                    new DependencyProvider(config).Resolve<IExample>()
            );
        }
        
        [Test]
        public void NotRegisteredTypeTest()
        {
            config.Register<IExample, ClassForExample>(true);
            Assert.Throws(typeof(ArgumentException), () =>
                new DependencyProvider(config).Resolve<ClassForExample2>()
            );
        }

        [Test]
        public void OpenGenericTypeTest()
        {
            config.Register<IRepository, MySQLRepository>();
            config.Register(typeof(ServiceImpl<>), typeof(ServiceImpl<>));
            var provider = new DependencyProvider(config);
            ServiceImpl<IRepository> actual = provider.Resolve<ServiceImpl<IRepository>>();
            Assert.IsNotNull(actual);
            Assert.AreEqual(5, actual.ReturnSmth());
        }

        [Test]
        public void CycleDependencyTest()
        {
            config.Register<ClassForExample, ClassForExample>();
            config.Register<ClassForExample2, ClassForExample2>(true); 
            config.Register<ClassForExample3, ClassForExample3>(true);
            
            var provider = new DependencyProvider(config);
            ClassForExample actual = provider.Resolve<ClassForExample>();
            Assert.IsNotNull(actual);
            Assert.AreEqual(null, actual.example.example.example);            
        }
    }
}