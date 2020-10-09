using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace QuickAccess.Infrastructure
{


	public static partial class Helper
	{
		public const StringSplitOptions RemoveEmpty = StringSplitOptions.RemoveEmptyEntries;

		public readonly struct ParsingResult
		{
			public object Value { get; }
			public bool IsParsed { get; }

			public ParsingResult(in bool isParsed, in object value)
			{
				Value = value;
				IsParsed = isParsed;
			}
		}

		private delegate bool TryParseFunc<T>(string text, NumberStyles numberStyles, IFormatProvider f, out T val);
		private delegate bool TryParseNotNumFunc<T>(string text, IFormatProvider f, out T val);
		private delegate bool TryParseDateTimeFunc<T>(string text, IFormatProvider f, DateTimeStyles ds, out T val);

		private static ParsingResult ParseValue<T>(string text, NumberStyles numberStyles, IFormatProvider formatProvider, TryParseFunc<T> tryParseFunc)
		{
			return tryParseFunc.Invoke(text, numberStyles, formatProvider, out var val)
				? new ParsingResult(true, val)
				: new ParsingResult(false, default);
		}

		private static ParsingResult ParseValue<T>(string text, IFormatProvider formatProvider, TryParseNotNumFunc<T> tryParseFunc)
		{
			return tryParseFunc.Invoke(text, formatProvider, out var val)
				? new ParsingResult(true, val)
				: new ParsingResult(false, default);
		}

		private static ParsingResult ParseValue<T>(string text, IFormatProvider formatProvider, TryParseDateTimeFunc<T> tryParseFunc)
		{
			return tryParseFunc.Invoke(text, formatProvider, DateTimeStyles.None, out var val)
				? new ParsingResult(true, val)
				: new ParsingResult(false, default);
		}

		private static ParsingResult ParseNullable<T>(string text, NumberStyles numberStyles, IFormatProvider formatProvider, TryParseFunc<T> tryParseFunc)
			where T : struct
		{
			if (string.IsNullOrEmpty(text))
			{
				return new ParsingResult(true, null);
			}

			return tryParseFunc.Invoke(text, numberStyles, formatProvider, out var val)
				? new ParsingResult(true, new T?(val))
				: new ParsingResult(false, null);
		}

		private static ParsingResult ParseNullable<T>(string text, IFormatProvider formatProvider, TryParseNotNumFunc<T> tryParseFunc)
			where T : struct
		{
			if (string.IsNullOrEmpty(text))
			{
				return new ParsingResult(true, null);
			}

			return tryParseFunc.Invoke(text, formatProvider, out var val)
				? new ParsingResult(true, new T?(val))
				: new ParsingResult(false, null);
		}

		private static ParsingResult ParseNullable<T>(string text, IFormatProvider formatProvider, TryParseDateTimeFunc<T> tryParseFunc)
			where T : struct
		{
			if (string.IsNullOrEmpty(text))
			{
				return new ParsingResult(true, null);
			}

			return tryParseFunc.Invoke(text, formatProvider, DateTimeStyles.None, out var val)
				? new ParsingResult(true, new T?(val))
				: new ParsingResult(false, null);
		}

		private static readonly Dictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> ParsersByType = new Dictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>>(12)
		{
			{typeof(byte), (t, n, f) => ParseValue<byte>(t, n, f, byte.TryParse)},
			{typeof(byte?), (t, n, f) => ParseNullable<byte>(t, n, f, byte.TryParse)},
			{typeof(sbyte), (t, n, f) => ParseValue<sbyte>(t, n, f, sbyte.TryParse)},
			{typeof(sbyte?), (t, n, f) => ParseNullable<sbyte>(t, n, f, sbyte.TryParse)},
			{typeof(short), (t, n, f) => ParseValue<short>(t, n, f, short.TryParse)},
			{typeof(short?), (t, n, f) => ParseNullable<short>(t, n, f, short.TryParse)},
			{typeof(ushort), (t, n, f) => ParseValue<ushort>(t, n, f, ushort.TryParse)},
			{typeof(ushort?), (t, n, f) => ParseNullable<ushort>(t, n, f, ushort.TryParse)},
			{typeof(int), (t, n, f) => ParseValue<int>(t, n, f, int.TryParse)},
			{typeof(int?), (t, n, f) => ParseNullable<int>(t, n, f, int.TryParse)},
			{typeof(uint), (t, n, f) => ParseValue<uint>(t, n, f, uint.TryParse)},
			{typeof(uint?), (t, n, f) => ParseNullable<uint>(t, n, f, uint.TryParse)},
			{typeof(long), (t, n, f) => ParseValue<long>(t, n, f, long.TryParse)},
			{typeof(long?), (t, n, f) => ParseNullable<long>(t, n, f, long.TryParse)},
			{typeof(ulong), (t, n, f) => ParseValue<ulong>(t, n, f, ulong.TryParse)},
			{typeof(ulong?), (t, n, f) => ParseNullable<ulong>(t, n, f, ulong.TryParse)},
			{typeof(double), (t, n, f) => ParseValue<double>(t, n, f, double.TryParse)},
			{typeof(double?), (t, n, f) => ParseNullable<double>(t, n, f, double.TryParse)},
			{typeof(decimal), (t, n, f) => ParseValue<decimal>(t, n, f, decimal.TryParse)},
			{typeof(decimal?), (t, n, f) => ParseNullable<decimal>(t, n, f, decimal.TryParse)},
			{typeof(TimeSpan), (t, n, f) => ParseValue<TimeSpan>(t, f, TimeSpan.TryParse)},
			{typeof(TimeSpan?), (t, n, f) => ParseNullable<TimeSpan>(t, f, TimeSpan.TryParse)},
			{typeof(DateTime), (t, n, f) => ParseValue<DateTime>(t, f, DateTime.TryParse)},
			{typeof(DateTime?), (t, n, f) => ParseNullable<DateTime>(t, f, DateTime.TryParse)},
			
			{typeof(string), (t, n, f) => new ParsingResult(true, t)},
			{typeof(char), (t, n, f) => new ParsingResult(true, t[0])},
			{typeof(char?), (t, n, f) => string.IsNullOrEmpty(t) ? new ParsingResult(false, null) : new ParsingResult(true, t[0])}
		};

		public static void ParseText<T1, T2, T3, T4, T5, T6, T7, T8>(this string text,
		                                                             string sep,
		                                                             out T1 a1,
		                                                             out T2 a2,
		                                                             out T3 a3,
		                                                             out T4 a4,
		                                                             out T5 a5,
		                                                             out T6 a6,
		                                                             out T7 a7,
		                                                             out T8 a8,
		                                                             StringSplitOptions opt = RemoveEmpty, 
		                                                             IFormatProvider formatProvider = null,
		                                                             IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers = null)
		{
			var args = Split(text, 8, sep, opt);
			a1 = Parse<T1>(args[0], formatProvider, parsers);
			a2 = Parse<T2>(args[1], formatProvider, parsers);
			a3 = Parse<T3>(args[2], formatProvider, parsers);
			a4 = Parse<T4>(args[3], formatProvider, parsers);
			a5 = Parse<T5>(args[4], formatProvider, parsers);
			a6 = Parse<T6>(args[5], formatProvider, parsers);
			a7 = Parse<T7>(args[6], formatProvider, parsers);
			a8 = Parse<T8>(args[7], formatProvider, parsers);
		}

		public static void ParseText<T1, T2, T3, T4, T5, T6, T7>(this string text,
		                                                         string sep,
		                                                         out T1 a1,
		                                                         out T2 a2,
		                                                         out T3 a3,
		                                                         out T4 a4,
		                                                         out T5 a5,
		                                                         out T6 a6,
		                                                         out T7 a7,
		                                                         StringSplitOptions opt = RemoveEmpty, 
		                                                         IFormatProvider formatProvider = null,
		                                                         IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers = null)
		{
			var args = Split(text, 7, sep, opt);
			a1 = Parse<T1>(args[0], formatProvider, parsers);
			a2 = Parse<T2>(args[1], formatProvider, parsers);
			a3 = Parse<T3>(args[2], formatProvider, parsers);
			a4 = Parse<T4>(args[3], formatProvider, parsers);
			a5 = Parse<T5>(args[4], formatProvider, parsers);
			a6 = Parse<T6>(args[5], formatProvider, parsers);
			a7 = Parse<T7>(args[6], formatProvider, parsers);
		}

		public static void ParseText<T1, T2, T3, T4, T5, T6>(this string text,
		                                                     string sep,
		                                                     out T1 a1,
		                                                     out T2 a2,
		                                                     out T3 a3,
		                                                     out T4 a4,
		                                                     out T5 a5,
		                                                     out T6 a6,
		                                                     StringSplitOptions opt = RemoveEmpty, 
		                                                     IFormatProvider formatProvider = null,
		                                                     IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers = null)
		{
			var args = Split(text, 6, sep, opt);
			a1 = Parse<T1>(args[0], formatProvider, parsers);
			a2 = Parse<T2>(args[1], formatProvider, parsers);
			a3 = Parse<T3>(args[2], formatProvider, parsers);
			a4 = Parse<T4>(args[3], formatProvider, parsers);
			a5 = Parse<T5>(args[4], formatProvider, parsers);
			a6 = Parse<T6>(args[5], formatProvider, parsers);
		}

		public static void ParseText<T1, T2, T3, T4, T5>(this string text,
		                                                 string sep,
		                                                 out T1 a1,
		                                                 out T2 a2,
		                                                 out T3 a3,
		                                                 out T4 a4,
		                                                 out T5 a5,
		                                                 StringSplitOptions opt = RemoveEmpty, 
		                                                 IFormatProvider formatProvider = null,
		                                                 IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers = null)
		{
			var args = Split(text, 5, sep, opt);
			a1 = Parse<T1>(args[0], formatProvider, parsers);
			a2 = Parse<T2>(args[1], formatProvider, parsers);
			a3 = Parse<T3>(args[2], formatProvider, parsers);
			a4 = Parse<T4>(args[3], formatProvider, parsers);
			a5 = Parse<T5>(args[4], formatProvider, parsers);
		}

		public static void ParseText<T1, T2, T3, T4>(this string text,
		                                             string sep,
		                                             out T1 a1,
		                                             out T2 a2,
		                                             out T3 a3,
		                                             out T4 a4,
		                                             StringSplitOptions opt = RemoveEmpty, 
		                                             IFormatProvider formatProvider = null,
		                                             IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers = null)
		{
			var args = Split(text, 4, sep, opt);
			a1 = Parse<T1>(args[0], formatProvider, parsers);
			a2 = Parse<T2>(args[1], formatProvider, parsers);
			a3 = Parse<T3>(args[2], formatProvider, parsers);
			a4 = Parse<T4>(args[3], formatProvider, parsers);
		}

		public static void ParseText<T1, T2, T3>(this string text,
		                                         string sep,
		                                         out T1 a1,
		                                         out T2 a2,
		                                         out T3 a3,
		                                         StringSplitOptions opt = RemoveEmpty, 
		                                         IFormatProvider formatProvider = null,
		                                         IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers = null)
		{
			var args = Split(text, 3, sep, opt);
			a1 = Parse<T1>(args[0], formatProvider, parsers);
			a2 = Parse<T2>(args[1], formatProvider, parsers);
			a3 = Parse<T3>(args[2], formatProvider, parsers);
		}

		public static void ParseText<T1, T2>(this string text,
		                                     string sep,
		                                     out T1 a1,
		                                     out T2 a2,
		                                     StringSplitOptions opt = RemoveEmpty, 
		                                     IFormatProvider formatProvider = null,
		                                     IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers = null)
		{
			var args = Split(text, 2, sep, opt);
			a1 = Parse<T1>(args[0], formatProvider, parsers);
			a2 = Parse<T2>(args[1], formatProvider, parsers);
		}

		public static void ParseText<T>(this string text, out T a1, 
		                                IFormatProvider formatProvider = null,
		                                IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers = null)
		{
			a1 = Parse<T>(text, formatProvider, parsers);
		}

		public static T ParseText<T>(this string text, 
		                             IFormatProvider formatProvider = null,
		                             IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers = null)
		{
			return Parse<T>(text, formatProvider, parsers);
		}

		public static T[] ParseArray<T>(this string text,
		                                string sep,
		                                int expectedCount = -1,
		                                StringSplitOptions opt = RemoveEmpty, 
		                                IFormatProvider formatProvider = null,
		                                IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers = null)
		{
			var args = Split(text, expectedCount, sep, opt);

			var res = new T[args.Length];

			args.CopyTo(res, v => Parse<T>(v, formatProvider, parsers));

			return res;
		}

		public static void CopyTo<TIn, TOut>(this TIn[] source,
		                                     int sourceOffset,
		                                     TOut[] dest,
		                                     int destOffset,
		                                     Func<TIn, TOut> srcToDestValue,
		                                     int count = int.MinValue)
		{
			if (count == int.MinValue)
			{
				count = source.Length;
			}

			if (count < 0)
			{
				throw new ArgumentException($"Count can't be less than zero but is {count}", nameof(count));
			}

			if (count + destOffset > dest.Length)
			{
				throw new IndexOutOfRangeException("Destination array is to short.");
			}

			if (count + sourceOffset > source.Length)
			{
				throw new IndexOutOfRangeException("Source array is to short.");
			}

			for (var idx = count - 1; idx >= 0; --idx)
			{
				dest[destOffset + idx] = srcToDestValue.Invoke(source[sourceOffset + idx]);
			}
		}

		public static void CopyTo<TIn, TOut>(this TIn[] source,
		                                     TOut[] dest,
		                                     Func<TIn, TOut> srcToDestValue,
		                                     int count = int.MinValue)
		{
			CopyTo(source, 0, dest, 0, srcToDestValue, count);
		}

		public static string[] Split(string text, string sep, int expectedCount = -1, StringSplitOptions opt = RemoveEmpty)
		{
			return Split(text, expectedCount, sep, opt);
		}

		private static string[] Split(string text,
		                             int expectedCount,
		                             string sep,
		                             StringSplitOptions opt = RemoveEmpty)
		{
			var res = text?.Split(new string[] {sep}, opt);
			if (res == null)
			{
				throw new ArgumentNullException(nameof(text));
			}

			if (expectedCount >= 0 && res.Length != expectedCount)
			{
				throw new InvalidOperationException(
					$"Unexpected number of arguments: Expected {expectedCount} but found {res.Length} in text '{text}'.");
			}

			return res;
		}

		private static T Parse<T>(string text, IFormatProvider formatProvider, IReadOnlyDictionary<Type, Func<string, NumberStyles, IFormatProvider, ParsingResult>> parsers)
		{
			if (parsers == null)
			{
				parsers = ParsersByType;
			}

			if (!parsers.TryGetValue(typeof(T), out var parser))
			{
				throw new InvalidOperationException(
					$"Parser for type '{typeof(T).Name}' is not defined, can't parse text '{text}'.");
			}

			try
			{
				var res = parser.Invoke(text.Trim(), NumberStyles.None, formatProvider);

				return res.IsParsed ? (T) res.Value : throw new InvalidOperationException($"Can't parse value of type {typeof(T).Name} from {text}.");
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(
					$"{e.GetType().Name} occurred while parsing value '{text}' of type '{typeof(T).Name}': {e.Message}.", e);
			}
		}
	}

	public static class DateTimeProvider
	{
		public static DateTime UtcNow => DateTime.UtcNow;
		public static DateTime Now => DateTime.Now;

		public static Stopwatch CreateStopwatch()
		{
			return new Stopwatch();
		}
	}
}
