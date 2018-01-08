using System;
using MergeGen.Core;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            MergeGenerator mergeGen = new MergeGenerator();
            
            Console.WriteLine("Hello World!");
            mergeGen.TargetName = "TableTest";
            mergeGen.AddValue(fieldName: "TestFieldName", value: "TestValue", toUpdate: true);

            Console.WriteLine(mergeGen.BuildMergeStatement());

        }
    }
}
