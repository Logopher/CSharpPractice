using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection;

namespace Solitaire
{
	public static class Util
	{

		/// <summary>
		/// Plugs the Trace class up to the standard error stream.
		/// 
		/// See also: Util.Log (all overloads).
		/// </summary>
		static Util()
		{
			Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
		}

		/// <summary>
		/// Logs an exception's message to the standard error stream, along with some useful debugging data.
		/// </summary>
		/// <param name="e">an exception</param>
		public static void Log(Exception e,
				[CallerFilePath] string sourceFilePath = "",
				[CallerLineNumber] int sourceLineNumber = 0)
		{
			Trace.WriteLine(string.Format("{0}: {1}", e.GetType().Name, e.Message));
			Trace.Indent();
			Trace.WriteLine(string.Format("thrown from {0}.{1}", e.TargetSite.DeclaringType.FullName, e.TargetSite.Name));
			Trace.WriteLine(string.Format("logged from {0}:{1}", sourceFilePath, sourceLineNumber));
			Trace.Unindent();
		}

		/// <summary>
		/// Logs a message to the standard error stream, along with information about the calling source code.
		/// 
		/// Only the first parameter should be given a value; the others are to be filled in by the compiler. (See the source code for a useful application of C# attributes.)
		/// </summary>
		/// <param name="message">a message</param>
		/// <param name="memberName">the name of the calling method (do not specify)</param>
		/// <param name="sourceFilePath">the filesystem path containing this method invocation (do not specify)</param>
		/// <param name="sourceLineNumber">the line number of this method invocation (do not specify)</param>
		public static void Log(string message,
				[CallerMemberName] string memberName = "",
				[CallerFilePath] string sourceFilePath = "",
				[CallerLineNumber] int sourceLineNumber = 0)
		{
			Trace.WriteLine(message);
			Trace.Indent();
			Trace.WriteLine(string.Format("logged by {0} from {1}:{2}", memberName, sourceFilePath, sourceLineNumber));
			Trace.Unindent();
		}

		/// <summary>
		/// Copies every key-value pair in <paramref name="source"/> to <paramref name="destination"/>, overwriting values
		/// in <paramref name="destination"/> as necessary.
		/// </summary>
		/// <typeparam name="K">the key type</typeparam>
		/// <typeparam name="V">the value type</typeparam>
		/// <param name="destination">"this" dictionary, which is receiving new values (and keys)</param>
		/// <param name="source">another dictionary, which is providing the new key-value pairs</param>
		/// <returns>"this" dictionary, after modifying it in place</returns>
		public static IDictionary<K, V> Merge<K, V>(this IDictionary<K, V> destination, IDictionary<K, V> source)
		{
			foreach (var pair in source)
			{
				destination[pair.Key] = pair.Value;
			}

			return destination;
		}

		/// <summary>
		/// Reacts to each item in "this" collection (<paramref name="source"/>).
		/// </summary>
		/// <typeparam name="T">the type of items in "this" collection</typeparam>
		/// <param name="source">"this" collection, to be iterated over</param>
		/// <param name="action">some logic to be informed by each value in "this" collection</param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T item in source)
			{
				action(item);
			}
		}

		/// <summary>
		/// Constructs a new collection consisting of values calculated from the values in "this" collection
		/// (<paramref name="source"/>).
		/// </summary>
		/// <typeparam name="T">the type of values in "this" collection</typeparam>
		/// <typeparam name="U">the type of the values we are generating</typeparam>
		/// <param name="source">"this" collection, with the original values</param>
		/// <param name="transformation">a method to calculate new values from those in "this" collection</param>
		/// <returns>the new collection</returns>
		public static IEnumerable<U> Map<T, U>(this IEnumerable<T> source, Func<T, U> transformation)
		{
			foreach (T item in source)
			{
				yield return transformation(item);
			}
		}

		/// <summary>
		/// Constructs a dictionary containing all the same key-value pairs as "this" collection has.
		/// </summary>
		/// <typeparam name="K">the key type</typeparam>
		/// <typeparam name="V">the value type</typeparam>
		/// <param name="source">"this" collection, with the key-value pairs being copied into the new dictionary</param>
		/// <returns>the dictionary</returns>
		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> source)
		{
			return source.ToDictionary(pair => pair.Key, pair => pair.Value);
		}
		
		/// <summary>
		/// Constructs a string consisting of all the strings in "this" collection, joined by the given
		/// <paramref name="separator"/>.
		/// </summary>
		/// <param name="source">"this" collection, with the strings to be joined</param>
		/// <param name="separator">a string to be inserted between each adjacent pair of strings from "this" collection</param>
		/// <returns>the composite string</returns>
		public static string Join(this IEnumerable<string> source, string separator)
		{
			IEnumerator<string> e = source.GetEnumerator();
			e.MoveNext();

			string result = e.Current;

			while (e.MoveNext())
			{
				result += separator + e.Current;
			}

			return result;
		}

		/// <summary>
		/// Applies the given <paramref name="transformation"/> method to each item in "this" collection, then joins all
		/// the results together with <paramref name="separator"/>s.
		/// </summary>
		/// <typeparam name="T">the type of the values from which strings will be computed</typeparam>
		/// <param name="source">"this" collection, with the values from which strings will be computed</param>
		/// <param name="separator">a string to be inserted between each adjacent pair of the computed strings</param>
		/// <param name="transformation">a function to compute a string from each value in "this" collection</param>
		/// <returns>the composite string</returns>
		public static string Join<T>(this IEnumerable<T> source, string separator, Func<T, string> transformation)
		{
			return source.Map(transformation).Join(separator);
		}

		/// <summary>
		/// Constructs a dictionary which maps each possible integer equivalents of the values in an enum with all its
		/// possible names as declared in that same enum.
		/// </summary>
		/// <param name="t">a Type object representing some enum, i.e.: typeof(SomeEnum)</param>
		/// <returns>a dictionary representation of the enum behind the specified Type</returns>
		public static Dictionary<int, string[]> MakeDictionaryFromEnum(Type t)
		{
			Dictionary<int, List<string>> result = new Dictionary<int, List<string>>();

			Enum.GetNames(t).ForEach((string name) =>
				{
					int index = (int)Enum.Parse(t, name);

					List<string> bucket;
					if (result.TryGetValue(index, out bucket))
					{
						bucket.Add(name);
					}
					else
					{
						result.Add(index, new List<string>{ name });
					}
				});

			return result.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
		}

		/// <summary>
		/// Returns a new string with no more than <paramref name="maxLength"/> of "this" string's characters. If "this" string
		/// is shorter than <paramref name="maxLength"/>, the strings are identical; otherwise, they match up to that
		/// length.
		/// </summary>
		/// <param name="source">"this" (original) string</param>
		/// <param name="maxLength">the largest allowable length of the output string</param>
		/// <returns>a new string with the contents of <paramref name="source"/> but Length <= <paramref name="maxLength"/></returns>
		public static string Truncate(this string source, int maxLength)
		{
			if (source.Length > maxLength)
			{
				return source.Substring(0, maxLength);
			}
			return source;
		}
	}
}
