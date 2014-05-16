﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ProtoCore.DSASM;
using ProtoTestFx.TD;

namespace ProtoFFITests
{
    [TestFixture]
    class ArgumentMarshalingTests 
    {
        TestFrameWork theTest = null;
        [SetUp]
        public void Setup()
        {
            theTest = new TestFrameWork();
        }

        [TearDown]
        public void TearDown()
        {
            theTest.CleanUp();
        }

        [Test]
        public void TestReturnIList()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = 1..5;                b = DummyCollection.ReturnIList(a);                ";
            
            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "ReturnIList");
            //IList is marshaled as arbitrary rank var array
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeVar, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, args[0].UID);
            Assert.AreEqual(1, args[0].rank); //Expecting it tobe marshaled as 1D array

            theTest.Verify("b", new int[] { 1, 2, 3, 4, 5 });
        }

        [Test]
        public void TestReturnIEnumerableOfInt()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = 1..5;                b = DummyCollection.ReturnIEnumerableOfInt(a);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "ReturnIEnumerableOfInt");
            //IEnumerable<int> ==> int[]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(1, methods[0].ReturnType.Value.rank);

            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, args[0].UID);
            Assert.AreEqual(1, args[0].rank); //Expecting it tobe marshaled as 1D array

            theTest.Verify("b", new int[] { 1, 2, 3, 4, 5 });
        }

        [Test]
        public void TestReturnIEnumerablOfIList()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = {1..5, 6..10};                b = DummyCollection.ReturnIEnumerablOfIList(a);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "ReturnIEnumerablOfIList");
            //IEnumerable<IList> ==> var[]..[]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeVar, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeVar, args[0].UID);
            Assert.AreEqual(Constants.kArbitraryRank, args[0].rank); //Expecting it tobe marshaled as arbitrary dimension array

            theTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestAcceptIEnumerablOfIList()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = {1..5, 6..10};                b = DummyCollection.AcceptIEnumerablOfIList(a);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "AcceptIEnumerablOfIList");
            //IEnumerable<IList> ==> var[]..[]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeVar, args[0].UID);
            Assert.AreEqual(Constants.kArbitraryRank, args[0].rank); //Expecting it tobe marshaled as arbitrary dimension array

            theTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestReturnIListOfIListInt()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = {1..5, 6..10};                b = DummyCollection.ReturnIListOfIListInt(a);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "ReturnIListOfIListInt");
            //IEnumerable<IList<int>> ==> int[][]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(2, methods[0].ReturnType.Value.rank);

            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, args[0].UID);
            Assert.AreEqual(2, args[0].rank); //Expecting it tobe marshaled as 2D array

            theTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestAcceptIEnumerablOfIListInt()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = {1..5, 6..10};                b = DummyCollection.AcceptIEnumerablOfIListInt(a);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "AcceptIEnumerablOfIListInt");
            //IEnumerable<IList<int>> ==> int[][]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, args[0].UID);
            Assert.AreEqual(2, args[0].rank); //Expecting it tobe marshaled as 2D array

            theTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestReturnListOfList()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = {1..5, 6..10};                b = DummyCollection.ReturnListOfList(a);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "ReturnListOfList");
            //List<List<int>> ==> int[][]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(2, methods[0].ReturnType.Value.rank);

            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, args[0].UID);
            Assert.AreEqual(2, args[0].rank); //Expecting it tobe marshaled as 2D array

            theTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestAcceptListOfList()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = {1..5, 6..10};                b = DummyCollection.AcceptListOfList(a);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "AcceptListOfList");
            //List<List<int>> ==> int[][]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, args[0].UID);
            Assert.AreEqual(2, args[0].rank); //Expecting it tobe marshaled as 2D array

            theTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestReturn3DList()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = {{1..5}};                b = DummyCollection.Return3DList(a);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "Return3DList");
            //List<List<List<int>>> ==> int[][][]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(3, methods[0].ReturnType.Value.rank); //Expecting it tobe marshaled as 3D array

            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, args[0].UID);
            Assert.AreEqual(3, args[0].rank); //Expecting it tobe marshaled as 3D array

            theTest.Verify("b", new List<object> { new List<object> { new int[] { 1, 2, 3, 4, 5 } } });
        }

        [Test]
        public void TestAccept3DList()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = {{1..5}};                b = DummyCollection.Accept3DList(a);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "Accept3DList");
            //List<List<List<int>>> ==> int[][][]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, args[0].UID);
            Assert.AreEqual(3, args[0].rank); //Expecting it tobe marshaled as 3D array

            theTest.Verify("b", new List<object> { new List<object> { new int[] { 1, 2, 3, 4, 5 } } });
        }

        [Test]
        public void TestReturnListOf5Points()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                b = DummyCollection.ReturnListOf5Points();                c = Count(b);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "ReturnListOf5Points");
            //Verify DummyPoint class is marshaled
            Assert.IsTrue(ProtoCore.DSASM.Constants.kInvalidIndex != theTest.GetClassIndex("DummyPoint"));

            //IList<DummyPoint> ==> DummyPoint[]
            Assert.AreEqual("FFITarget.DummyPoint", methods[0].ReturnType.Value.Name);
            Assert.AreEqual(1, methods[0].ReturnType.Value.rank);

            var args = methods[0].GetArgumentTypes();
            Assert.IsTrue(args == null || args.Count == 0);
            
            theTest.Verify("c", 5);
        }

        [Test]
        public void TestAcceptListOf5PointsReturnAsObject()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = DummyCollection.ReturnListOf5Points();                b = DummyCollection.AcceptListOf5PointsReturnAsObject(a);                c = Count(b);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "AcceptListOf5PointsReturnAsObject");
            //Verify DummyPoint class is marshaled
            Assert.IsTrue(ProtoCore.DSASM.Constants.kInvalidIndex != theTest.GetClassIndex("DummyPoint"));

            //object is marshaled as arbitrary rank var array
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeVar, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            //IEnumerable<DummyPoint> ==> DummyPoint[]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual("FFITarget.DummyPoint", args[0].Name);
            Assert.AreEqual(1, args[0].rank); //Expecting it tobe marshaled as 3D array

            theTest.Verify("c", 5);
        }

        [Test]
        public void TestAcceptObjectAsVar()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = DummyCollection.ReturnListOf5Points();                b = DummyCollection.AcceptObjectAsVar(a);                c = Count(b);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "AcceptObjectAsVar");
            //Return object ==> var[]..[]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeVar, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            //Arg object ==> var
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeVar, args[0].UID);
            Assert.AreEqual(0, args[0].rank); //Expecting it tobe marshaled as singleton

            theTest.Verify("c", 5);
            //Replication will cause it to return a collection
            theTest.Verify("b", FFITarget.DummyCollection.ReturnListOf5Points());
        }

        [Test]
        public void TestObjectAsArbitraryDimensionArrayImport()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = DummyCollection.ReturnListOf5Points();                b = DummyCollection.ObjectAsArbitraryDimensionArrayImport({a, 1..5});                c = Count(b[0]);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "ObjectAsArbitraryDimensionArrayImport");

            //[ArbitraryDimensionArrayImport] object ==> var[]..[]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeVar, args[0].UID);
            Assert.AreEqual(Constants.kArbitraryRank, args[0].rank); //Expecting it tobe marshaled as 3D array

            theTest.Verify("c", 5);
            theTest.Verify("b", new List<object> { FFITarget.DummyCollection.ReturnListOf5Points(), new int[] { 1, 2, 3, 4, 5 } });
        }

        [Test]
        public void TestReturnIDictionary()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = DummyCollection.ReturnIDictionary();                b = { GetKeys(a), GetValues(a)};                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "ReturnIDictionary");
            
            //IDictionary is marshaled as arbitrary rank var array
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeVar, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            theTest.Verify("b", new List<object> { new string[] { "A", "B", "C", "D" }, new int[] { 1, 2, 3, 4 } });
        }

        [Test]
        public void TestReturnDictionaryAsObject()
        {
            string code = @"                import(DummyCollection from ""FFITarget.dll"");                a = DummyCollection.ReturnDictionaryAsObject();                b = { GetKeys(a), GetValues(a)};                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("DummyCollection", "ReturnDictionaryAsObject");
            
            //object is marshaled as arbitrary rank var array
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeVar, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            theTest.Verify("b", new List<object> { new string[] { "A", "B", "C", "D" }, new int[] { 1, 2, 3, 4 } });
        }

        [Test]
        public void TestDefaultValueOnNullableArgument()
        {
            string code = @"                import(NullableArgumentTest from ""FFITarget.dll"");                a = NullableArgumentTest.GetValue();                b = NullableArgumentTest.GetValue(5);                ";

            theTest.RunScriptSource(code);
            var methods = theTest.GetMethods("NullableArgumentTest", "GetValue");

            //Nullable type int? is migrated as int.
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, args[0].UID);

            //Return type is int.
            Assert.AreEqual((int)ProtoCore.PrimitiveType.kTypeInt, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            theTest.Verify("a", -1);
            theTest.Verify("b", 5);
        }
    }
}
