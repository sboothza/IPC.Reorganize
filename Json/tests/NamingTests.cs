using IPC.Reorganize.Json;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace IPC.Reorganize.Json.Tests;

[TestFixture]
public class NamingTests
{
    [Test]
    public void TestPascalNormal()
    {
        var test = "ThisIsATest";
        var match = test.ToPascalCase();
        ClassicAssert.AreEqual(test, match);

        test = "thisIsATest";
        match = test.ToPascalCase();
        ClassicAssert.AreEqual("ThisIsATest", match);

        test = "this_is_a_test";
        match = test.ToPascalCase();
        ClassicAssert.AreEqual("ThisIsATest", match);
    }

    [Test]
    public void TestCamelNormal()
    {
        var test = "ThisIsATest";
        var match = test.ToCamelCase();
        ClassicAssert.AreEqual("thisIsATest", match);

        test = "thisIsATest";
        match = test.ToCamelCase();
        ClassicAssert.AreEqual("thisIsATest", match);

        test = "this_is_a_test";
        match = test.ToCamelCase();
        ClassicAssert.AreEqual("thisIsATest", match);
    }

    [Test]
    public void TestSnakeNormal()
    {
        var test = "ThisIsATest";
        var match = test.ToSnakeCase();
        ClassicAssert.AreEqual("this_is_a_test", match);

        test = "thisIsATest";
        match = test.ToSnakeCase();
        ClassicAssert.AreEqual("this_is_a_test", match);

        test = "this_is_a_test";
        match = test.ToSnakeCase();
        ClassicAssert.AreEqual("this_is_a_test", match);
    }

    [Test]
    public void TestCombinedNormal()
    {
        var test = "ThisIsATest";
        var match = test.ConvertName(NamingOptions.PascalCase);
        ClassicAssert.AreEqual("ThisIsATest", match);

        match = test.ConvertName(NamingOptions.CamelCase);
        ClassicAssert.AreEqual("thisIsATest", match);

        match = test.ConvertName(NamingOptions.SnakeCase);
        ClassicAssert.AreEqual("this_is_a_test", match);
    }

    [Test]
    public void TestCultureSafety()
    {
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");

        var test = "ThisIsATest";
        var match = test.ConvertName(NamingOptions.SnakeCase);
        ClassicAssert.AreEqual("this_is_a_test", match);
    }

    [Test]
    public void TestNull()
    {
        string test = null;
        var match = test.ConvertName(NamingOptions.PascalCase);
        ClassicAssert.AreEqual("", match);

        test = "";
        match = test.ConvertName(NamingOptions.PascalCase);
        ClassicAssert.AreEqual("", match);
    }
}