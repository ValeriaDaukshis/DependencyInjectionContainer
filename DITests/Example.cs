using System;

namespace DITests
{
    public interface IRepository
    {
        int ReturnSmth();
    }

    public class MySQLRepository: IRepository
    {
        public int ReturnSmth()
        {
            return 5;
        }
    }

    public class ServiceImpl<TRepository> where TRepository : IRepository   
    {
        TRepository rep;

        public ServiceImpl(TRepository repository)
        {
            rep = repository;
        }

        public int ReturnSmth()
        {
            return rep.ReturnSmth();
        }
    }

    public interface IExample
    {
        void Print();
    }

    public class ClassForExample:IExample
    {
        public ClassForExample2 example { get; set; }

        public ClassForExample(ClassForExample2 example)
        {
            this.example = example;
        }

        public ClassForExample()
        {
        }

        public void Print()
        {
            Console.WriteLine("I am EXAMPLE 1");
            if (example != null)
                example.Print();
        }
    }
       
    public class ClassForExample2:IExample
    {
        public ClassForExample3 example { get; set; }

        public ClassForExample2(ClassForExample3 example)
        {
            this.example = example;
        }

        public ClassForExample2()
        {
        }

        public void Print()
        {
            Console.WriteLine("I am EXAMPLE 2");
            if (example != null)
                example.Print();
        }
    }

    public class ClassForExample3
    {
        public ClassForExample example { get; set; }

        public ClassForExample3(ClassForExample example)
        {
            this.example = example;
        }

        public ClassForExample3()
        {
        }

        public void Print()
        {
            Console.WriteLine("I am EXAMPLE 3 with link to EXAMPLE 1");
        }
    }
}