using IPC.Reorganize.Json;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace IPC.Reorganize.Json.Tests
{
    [TestFixture]
    public class TokeniserTests
    {
        [Test]
        public void TestString()
        {
            var json = "\"this is a string\"";
            var parser = new JsonParser();
            var result = parser.Parse(json);
            ClassicAssert.AreEqual(typeof(StringToken), result[0].GetType());
            ClassicAssert.AreEqual("this is a string", (result[0] as StringToken).Value);
        }

        [Test]
        public void TestStringWithWhitespace()
        {
            var json = "  \t\"this is a string\"   \t\r\n";
            var parser = new JsonParser();
            var result = parser.Parse(json);
            ClassicAssert.AreEqual(typeof(StringToken), result[0].GetType());
            ClassicAssert.AreEqual("this is a string", (result[0] as StringToken).Value);
        }

        [Test]
        public void TestStringWithEscape()
        {
            var json = "\"\\\"this is a string\\\"\"";
            var parser = new JsonParser();
            var result = parser.Parse(json);
            ClassicAssert.AreEqual(typeof(StringToken), result[0].GetType());
            ClassicAssert.AreEqual("\"this is a string\"", (result[0] as StringToken).Value);
        }

        [Test]
        public void TestNumber()
        {
            var json = "  -123.5 ";
            var parser = new JsonParser();
            var result = parser.Parse(json);
            ClassicAssert.AreEqual(typeof(NumberToken), result[0].GetType());
            ClassicAssert.AreEqual("-123.5", (result[0] as NumberToken).Value);
        }

        [Test]
        public void TestBoolean()
        {
            var json = "  false  TruE ";
            var parser = new JsonParser();
            var result = parser.Parse(json);
            ClassicAssert.AreEqual(typeof(UnquotedConstantToken), result[0].GetType());
            ClassicAssert.AreEqual(typeof(UnquotedConstantToken), result[1].GetType());
            ClassicAssert.AreEqual("false", (result[0] as UnquotedConstantToken).Value);
            ClassicAssert.AreEqual("true", (result[1] as UnquotedConstantToken).Value);
        }

        [Test]
        public void TestCombined()
        {
            var json = "  false -123.5  TruE \"this is a string\" ";
            var parser = new JsonParser();
            var result = parser.Parse(json);

            ClassicAssert.AreEqual(typeof(UnquotedConstantToken), result[0].GetType());
            ClassicAssert.AreEqual("false", (result[0] as UnquotedConstantToken).Value);

            ClassicAssert.AreEqual(typeof(NumberToken), result[1].GetType());
            ClassicAssert.AreEqual("-123.5", (result[1] as NumberToken).Value);

            ClassicAssert.AreEqual(typeof(UnquotedConstantToken), result[2].GetType());
            ClassicAssert.AreEqual("true", (result[2] as UnquotedConstantToken).Value);

            ClassicAssert.AreEqual(typeof(StringToken), result[3].GetType());
            ClassicAssert.AreEqual("this is a string", (result[3] as StringToken).Value);
        }

        [Test]
        public void TestTokeniseBasic()
        {
            var json =
                "{\"str1\":null,\"str2\":{\"Id\":\"123\",\"Source\":\"src1\",\"Tags\":[{\"Name\":\"name1\",\"Value\":\"value1\",\"Category\":0},{\"Name\":\"name2\",\"Value\":\"\",\"Category\":0}]}}";
            var parser = new JsonParser();
            var result = parser.Parse(json);
            var expectedResult = new List<JsonToken>
            {
                new ObjectStartToken(),
                new StringToken("str1"),
                new MemberSeparatorToken(),
                new UnquotedConstantToken("null"),
                new CommaToken(),
                new StringToken("str2"),
                new MemberSeparatorToken(),
                new ObjectStartToken(),
                new StringToken("Id"),
                new MemberSeparatorToken(),
                new StringToken("123"),
                new CommaToken(),
                new StringToken("Source"),
                new MemberSeparatorToken(),
                new StringToken("src1"),
                new CommaToken(),
                new StringToken("Tags"),
                new MemberSeparatorToken(),
                new ListStartToken(),
                new ObjectStartToken(),
                new StringToken("Name"),
                new MemberSeparatorToken(),
                new StringToken("name1"),
                new CommaToken(),
                new StringToken("Value"),
                new MemberSeparatorToken(),
                new StringToken("value1"),
                new CommaToken(),
                new StringToken("Category"),
                new MemberSeparatorToken(),
                new NumberToken("0"),
                new ObjectEndToken(),
                new CommaToken(),
                new ObjectStartToken(),
                new StringToken("Name"),
                new MemberSeparatorToken(),
                new StringToken("name2"),
                new CommaToken(),
                new StringToken("Value"),
                new MemberSeparatorToken(),
                new StringToken(""),
                new CommaToken(),
                new StringToken("Category"),
                new MemberSeparatorToken(),
                new NumberToken("0"),
                new ObjectEndToken(),
                new ListEndToken(),
                new ObjectEndToken(),
                new ObjectEndToken()
            };

            for (int i = 0; i < expectedResult.Count; i++)
            {
                var expected = expectedResult[i];
                var actual = result[i];
                ClassicAssert.AreEqual(expected.GetType(), actual.GetType());
                if (expected is StringToken stringToken)
                {
                    ClassicAssert.AreEqual(stringToken.Value, (actual as StringToken).Value);
                }
                else if (expected is NumberToken numberToken)
                {
                    ClassicAssert.AreEqual(numberToken.Value, (actual as NumberToken).Value);
                }
                else if (expected is UnquotedConstantToken unquotedConstantToken)
                {
                    ClassicAssert.AreEqual(unquotedConstantToken.Value, (actual as UnquotedConstantToken).Value);
                }
            }
        }


        [Test]
        public void TestTokeniseList()
        {
            var json = "[\"value1\",\"value2\",0,\"\",false]";
            var parser = new JsonParser();
            var result = parser.Parse(json);

            ClassicAssert.AreEqual(11, result.Count);

            ClassicAssert.AreEqual(typeof(ListStartToken), result[0].GetType());

            ClassicAssert.AreEqual(typeof(StringToken), result[1].GetType());
            ClassicAssert.AreEqual("value1", (result[1] as StringToken).Value);

            ClassicAssert.AreEqual(typeof(CommaToken), result[2].GetType());

            ClassicAssert.AreEqual(typeof(StringToken), result[3].GetType());
            ClassicAssert.AreEqual("value2", (result[3] as StringToken).Value);

            ClassicAssert.AreEqual(typeof(CommaToken), result[4].GetType());

            ClassicAssert.AreEqual(typeof(NumberToken), result[5].GetType());
            ClassicAssert.AreEqual("0", (result[5] as NumberToken).Value);

            ClassicAssert.AreEqual(typeof(CommaToken), result[6].GetType());

            ClassicAssert.AreEqual(typeof(StringToken), result[7].GetType());
            ClassicAssert.AreEqual("", (result[7] as StringToken).Value);

            ClassicAssert.AreEqual(typeof(CommaToken), result[8].GetType());

            ClassicAssert.AreEqual(typeof(UnquotedConstantToken), result[9].GetType());
            ClassicAssert.AreEqual("false", (result[9] as UnquotedConstantToken).Value);

            ClassicAssert.AreEqual(typeof(ListEndToken), result[10].GetType());
        }

        [Test]
        public void TestObjectList()
        {
            var json = "{\n    \"list\":[\n        \"value1\",\n        \"value2\"\n    ]\n}";
            var parser = new JsonParser();
            var result = parser.Parse(json);
            foreach (var item in result)
                Console.WriteLine(item);
        }

        [Test]
        public void TestObjectListObject()
        {
            var json = "{\n    \"list\":[\n        {\"Value\":\"value1\"},\n        {\"Value\":\"value2\"}\n    ]\n}";
            var parser = new JsonParser();
            var result = parser.Parse(json);
            foreach (var item in result)
                Console.WriteLine(item);
        }


        [Test]
        public void TestParseValues()
        {
            var json = "  \t\"this is a string\"   \t\r\n";
            var parser = new JsonParser();
            var tokens = parser.Parse(json);
            var result = parser.ProcessTokens(tokens);
            ClassicAssert.AreEqual(typeof(JsonObjectValue), result.GetType());
            ClassicAssert.AreEqual("this is a string", (result as JsonObjectValue).Value);

            json = "  -123.5 ";
            tokens = parser.Parse(json);
            result = parser.ProcessTokens(tokens);
            ClassicAssert.AreEqual(typeof(JsonObjectValue), result.GetType());
            ClassicAssert.AreEqual("-123.5", (result as JsonObjectValue).Value);
        }
    }
}