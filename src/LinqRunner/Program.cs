using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqFilter.Filter.Entities;
using LinqFilter.Filter.Impl;

namespace LinqRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = PrepareInput();

            var request = new LinqFilterRequest<TestClass>
                              {
                                  Collection = input,
                                  Filters = new List<LinqFilterOperation>
                                                {
                                                    new LinqFilterOperation
                                                        {
                                                            EntityPropertyAccessPath = "Name",
                                                            OperationKind = OperationKind.Contains,
                                                            TestValues = new string[] {"A", "B"}
                                                        },
                                                    new LinqFilterOperation
                                                        {
                                                            EntityPropertyAccessPath = "InnerTestClass.Age",
                                                            OperationKind = OperationKind.Equals,
                                                            TestValues = new[]
                                                                             {
                                                                                 "10"
                                                                             }
                                                        }
                                                }
                              };

            var result = new GenericFilterCommand<TestClass>().Process(request);

            foreach(var res in result.FilterResults)
            {
                Console.WriteLine(res);
            }

            Console.ReadKey();
        }

        static IQueryable<TestClass> PrepareInput()
        {
            return new List<TestClass>
                       {
                           new TestClass
                               {
                                   Name = "A",
                                   InnerTestClass = new InnerTestClass
                                                        {
                                                            Age = 10
                                                        }
                               },
                               new TestClass
                               {
                                   Name = "A",
                                   InnerTestClass = new InnerTestClass
                                                        {
                                                            Age = 11
                                                        }
                               },
                               new TestClass
                               {
                                   Name = "B",
                                   InnerTestClass = new InnerTestClass
                                                        {
                                                            Age = 10
                                                        }
                               },
                               new TestClass
                               {
                                   Name = "B",
                                   InnerTestClass = new InnerTestClass
                                                        {
                                                            Age = 10
                                                        }
                               }
                       }.AsQueryable();
        }
    }

    class TestClass
    {
        public string Name { get; set; }
        public InnerTestClass InnerTestClass { get; set; }

        public override string ToString()
        {
            return Name + " " + InnerTestClass.Age;
        }
    }

    public class InnerTestClass
    {
        public int Age { get; set; }
    }
}
