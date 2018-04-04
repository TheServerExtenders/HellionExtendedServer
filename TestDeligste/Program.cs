using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TestDeligste
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {
            HandleEvent += Call1;
            HandleEvent += Call2;
            HandleEvent += Call3;

            Foo f = new Foo();
            f.M();

            Foo b = new Bar();
            b.M();
           
            HandleEvent("asd2");
            Console.WriteLine(HandleEvent.GetInvocationList()[0].GetType().Name +"<<<<<<<<<<<");
            Example d = Call1;
            if (HandleEvent.GetInvocationList()[0].Equals(d))Console.WriteLine("GOOD!!!!!!");
            if (HandleEvent.GetInvocationList()[0] == d)Console.WriteLine("GOO22222222D!!!!!!");
        }

        public void Call1(string data)
        {
            Console.WriteLine("1"+data);
            Thread.Sleep(1000);
            HandleEvent = Call1;
            
        }

        public void Call2(string data)
        {
            Console.WriteLine("2"+data);
        }

        public void Call3(string data)
        {
            Console.WriteLine("3"+data);
        }

        public delegate void Example(string data);

        public Example HandleEvent;
        
        class Foo
        {
            public void M()
            {
                Console.WriteLine("Foo.M");
            }
        }

        class Bar : Foo
        {
            public new void M()
            {
                Console.WriteLine("Bar.M");
            }
        }
    }
}