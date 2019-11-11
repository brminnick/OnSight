﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace OnSight
{
    public static class DebugService
	{
        [Conditional("DEBUG")]
		public static void PrintException(
			Exception exception,
			[CallerMemberName] string callerMemberName = "",
			[CallerLineNumber] int lineNumber = 0,
			[CallerFilePath] string filePath = "")
		{
			var fileName = System.IO.Path.GetFileName(filePath);

			Debug.WriteLine(exception.GetType());
			Debug.WriteLine($"Error: {exception.Message}");
			Debug.WriteLine($"Line Number: {lineNumber}");
			Debug.WriteLine($"Caller Name: {callerMemberName}");
			Debug.WriteLine($"File Name: {fileName}");
		}
	}
}
