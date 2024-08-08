using System.Runtime.CompilerServices;
namespace IPC.Reorganize.Json;

public static class ParserExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsObjectStart(this char value) => value == '{';

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsObjectEnd(this char value) => value == '}';

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsUnquotedConstant(this char value) => value is 't' or 'T' or 'f' or 'F' or 'n' or 'N';

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNumber(this char value) => char.IsDigit(value) || value == '-';

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsQuote(this char value) => value == '"';
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsListStart(this char value) => value == '[';
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsListEnd(this char value) => value == ']';

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsMemberSeparator(this char value) => value == ':';
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsComma(this char value) => value == ',';
}