using System.Diagnostics;
using System.Text;

namespace IPC.Reorganize.Json
{
    public class JsonParser(JsonSerializerOptions? options = null)
    {
        private StringReader? _reader;
        private readonly JsonSerializerOptions _options = options ?? JsonSerializerOptions.Empty;

        public static JsonObject? ParseObject(string json, JsonSerializerOptions options)
        {
            var parser = new JsonParser(options);
            var tokens = parser.Parse(json);
            return parser.ProcessTokens(tokens);
        }

        public List<JsonToken> Parse(string json)
        {
            var tokens = new List<JsonToken>();
            _reader = new StringReader(json);
            var charCount = 0; //for debugging - to see which character it crashes on
            while (_reader.Peek() != -1)
            {
                var c = (char)_reader.Peek();
                if (char.IsWhiteSpace(c))
                {
                    _reader.Read();
                }
                else if (c == 'N' && _options.DeserializeMongoDbTypes)
                {
                    //parse mongo specific number
                    tokens.Add(new NumberToken(ParseMongoNumber()));
                }
                else if (c == 'I' && _options.DeserializeMongoDbTypes)
                {
                    //parse mongo specific date
                    tokens.Add(new StringToken(ParseMongoDate()));
                }
                else if (c.IsQuote())
                {
                    tokens.Add(new StringToken(ParseString()));
                }
                else if (c.IsNumber())
                {
                    tokens.Add(new NumberToken(ParseNumber()));
                }
                else if (c.IsUnquotedConstant())
                {
                    tokens.Add(new UnquotedConstantToken(ParseUnquotedConstant()));
                }
                else if (c.IsObjectStart())
                {
                    tokens.Add(new ObjectStartToken());
                    _reader.Read();
                }
                else if (c.IsObjectEnd())
                {
                    tokens.Add(new ObjectEndToken());
                    _reader.Read();
                }
                else if (c.IsListStart())
                {
                    tokens.Add(new ListStartToken());
                    _reader.Read();
                }
                else if (c.IsListEnd())
                {
                    tokens.Add(new ListEndToken());
                    _reader.Read();
                }
                else if (c.IsMemberSeparator())
                {
                    tokens.Add(new MemberSeparatorToken());
                    _reader.Read();
                }
                else if (c.IsComma())
                {
                    tokens.Add(new CommaToken());
                    _reader.Read();
                }
                else
                    throw new Exception($"Unknown character in expression: [{c}] - at position: {charCount}");

                charCount++;
            }

            return tokens;
        }

        private string ParseString()
        {
            _reader!.Read(); //skip first quote
            var sb = new StringBuilder();
            while (_reader.Peek() != '"')
            {
                var c = (char)_reader.Read();
                if (c == '\\')
                    c = (char)_reader.Read(); //handle escape
                sb.Append(c);
            }

            _reader.Read(); //skip last quote
            return sb.ToString();
        }

        private string ParseMongoDate()
        {
            //ISODate("2024-05-25T15:58:03.555+0200")
            var sb = new StringBuilder();
            while (_reader!.Peek() != ')')
            {
                var c = (char)_reader.Read();
                sb.Append(c);
            }

            _reader.Read(); //skip last bracket

            sb.Replace("ISODate(\"", "");
            sb.Replace("\"", "");
            return sb.ToString();
        }

        private string ParseNumber()
        {
            var sb = new StringBuilder();
            while (char.IsDigit((char)_reader!.Peek()) || _reader.Peek() == '.' || _reader.Peek() == '-')
                sb.Append((char)_reader.Read());

            return sb.ToString();
        }

        private string ParseMongoNumber()
        {
            //NumberInt(1)
            //NumberLong(1)
            //NumberFloat(1.65)
            //NumberDecimal(1.56)
            var sb = new StringBuilder();
            while (_reader!.Peek() != ')')
                sb.Append((char)_reader.Read());
            _reader.Read();

            sb.Replace("NumberInt(", "");
            sb.Replace("NumberLong(", "");
            sb.Replace("NumberFloat(", "");
            sb.Replace("NumberDecimal(", "");

            return sb.ToString();
        }

        private string ParseUnquotedConstant()
        {
            var sb = new StringBuilder();
            var c = char.ToLower((char)_reader!.Peek());
            while ("true".Any(s => s == c) || "false".Any(s => s == c) || "null".Any(s => s == c))
            {
                sb.Append((char)_reader.Read());
                c = char.ToLower((char)_reader.Peek());
            }

            var test = sb.ToString()
                .ToLowerInvariant();
            if (test != "true" && test != "false" && test != "null")
                throw new Exception("Invalid - expected true false or null");

            return test;
        }

        public JsonObject? ProcessTokens(List<JsonToken> tokens)
        {
            var index = 0;
            return ProcessTokens(tokens.ToArray(), ref index);
        }

        private JsonObject? ProcessTokens(JsonToken[] tokens, ref int index)
        {
            var token = tokens[index];
            return token switch
            {
                StringToken stringToken => new JsonObjectValue(stringToken.Value),
                NumberToken numberToken => new JsonObjectValue(numberToken.Value),
                UnquotedConstantToken unquotedConstantToken => new JsonObjectValue(unquotedConstantToken.Value),
                ObjectStartToken => ProcessObject(tokens, ref index),
                ListStartToken => ProcessList(tokens, ref index),
                _ => null
            };
        }

        private JsonObjectList ProcessList(JsonToken[] tokens, ref int index)
        {
            //current token should be a start list token
            Debug.Assert(tokens[index] is ListStartToken);

            var list = new JsonObjectList();
            var token = tokens[++index];
            while (token is not ListEndToken && index < tokens.Length)
            {
                switch (token)
                {
                    case StringToken stringToken:
                        list.Array.Add(new JsonObjectValue(value: stringToken.Value));
                        break;
                    case ObjectStartToken:
                        list.Array.Add(ProcessObject(tokens, ref index));
                        break;
                    case ListStartToken:
                        list.Array.Add(ProcessList(tokens, ref index));
                        break;
                }

                token = tokens[++index];
            }

            //current token should be end list
            Debug.Assert(token is ListEndToken);
            return list;
        }

        private JsonObject ProcessObject(JsonToken[] tokens, ref int index)
        {
            //current token should be objectstart
            Debug.Assert(tokens[index] is ObjectStartToken);

            var obj = new JsonObjectComplex();
            var token = tokens[++index];
            while (token is not ObjectEndToken && index < tokens.Length)
            {
                switch (token)
                {
                    case StringToken nameToken:
                    {
                        token = tokens[++index];
                        if (token is MemberSeparatorToken)
                        {
                            //next token is either constant or complex or list
                            index++;
                            var value = ProcessTokens(tokens, ref index);
                            obj.Complex[nameToken.Value] = value;
                        }
                        else
                            throw new Exception("Invalid token - expected separator");

                        break;
                    }
                    case CommaToken:
                        break;
                    default:
                        throw new Exception("Invalid token - expected name");
                }

                token = tokens[++index];
            }

            //the current token should be an end token
            Debug.Assert(token is ObjectEndToken);

            if (tokens[index - 1] is ObjectStartToken)
                obj.IsNull = true;

            return obj;
        }
    }
}