/* nativefiledialog# - C# Wrapper for nativefiledialog
 *
 * Copyright (c) 2021 Ethan Lee.
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. If you use this software in a
 * product, an acknowledgment in the product documentation would be
 * appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source distribution.
 *
 * Ethan "flibitijibibo" Lee <flibitijibibo@flibitijibibo.com>
 *
 */

#region Using Statements
using System;
using System.Runtime.InteropServices;
using System.Text;
#endregion

public static class nativefiledialog
{
	#region Native Library Name

	const string nativeLibName = "nfd";

	#endregion

	#region UTF8 Marshaling

	private static int Utf8Size(string str)
	{
		return (str.Length * 4) + 1;
	}

	private static unsafe byte* Utf8EncodeNullable(string str)
	{
		if (str == null)
		{
			return (byte*) 0;
		}
		int bufferSize = Utf8Size(str);
		byte* buffer = (byte*) Marshal.AllocHGlobal(bufferSize);
		fixed (char* strPtr = str)
		{
			Encoding.UTF8.GetBytes(
				strPtr,
				(str != null) ? (str.Length + 1) : 0,
				buffer,
				bufferSize
			);
		}
		return buffer;
	}

	private static unsafe string UTF8_ToManaged(IntPtr s, bool freePtr = false)
	{
		if (s == IntPtr.Zero)
		{
			return null;
		}

		/* We get to do strlen ourselves! */
		byte* ptr = (byte*) s;
		while (*ptr != 0)
		{
			ptr++;
		}

		/* TODO: This #ifdef is only here because the equivalent
		 * .NET 2.0 constructor appears to be less efficient?
		 * Here's the pretty version, maybe steal this instead:
		 *
		string result = new string(
			(sbyte*) s, // Also, why sbyte???
			0,
			(int) (ptr - (byte*) s),
			System.Text.Encoding.UTF8
		);
		 * See the CoreCLR source for more info.
		 * -flibit
		 */
#if NETSTANDARD2_0
		/* Modern C# lets you just send the byte*, nice! */
		string result = System.Text.Encoding.UTF8.GetString(
			(byte*) s,
			(int) (ptr - (byte*) s)
		);
#else
		/* Old C# requires an extra memcpy, bleh! */
		int len = (int) (ptr - (byte*) s);
		if (len == 0)
		{
			return string.Empty;
		}
		char* chars = stackalloc char[len];
		int strLen = System.Text.Encoding.UTF8.GetChars((byte*) s, len, chars, len);
		string result = new string(chars, 0, strLen);
#endif

		/* Some SDL functions will malloc, we have to free! */
		if (freePtr)
		{
			free(s);
		}
		return result;
	}

	[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
	private static extern void free(IntPtr ptr);

	#endregion

	#region Types

	public enum nfdresult_t
	{
		NFD_ERROR,
		NFD_OKAY,
		NFD_CANCEL
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct nfdpathset_t
	{
		IntPtr buf; /* nfdchar_t* */
		IntPtr indices; /* size_t* */
		IntPtr count; /* size_t */
	}

	#endregion

	#region Entry Points

	[DllImport(nativeLibName, EntryPoint = "NFD_OpenDialog", CallingConvention = CallingConvention.Cdecl)]
	private static extern unsafe nfdresult_t INTERNAL_NFD_OpenDialog(
		byte* filterList,
		byte* defaultPath,
		out IntPtr outPath
	);
	public static unsafe nfdresult_t NFD_OpenDialog(
		string filterList,
		string defaultPath,
		out string outPath
	) {
		byte* filterListPtr = Utf8EncodeNullable(filterList);
		byte* defaultPathPtr = Utf8EncodeNullable(defaultPath);
		IntPtr outPathPtr;

		nfdresult_t result = INTERNAL_NFD_OpenDialog(
			filterListPtr,
			defaultPathPtr,
			out outPathPtr
		);

		Marshal.FreeHGlobal((IntPtr) filterListPtr);
		Marshal.FreeHGlobal((IntPtr) defaultPathPtr);
		outPath = UTF8_ToManaged(outPathPtr, true);
		return result;
	}

	[DllImport(nativeLibName, EntryPoint = "NFD_OpenDialogMultiple", CallingConvention = CallingConvention.Cdecl)]
	private static extern unsafe nfdresult_t INTERNAL_NFD_OpenDialogMultiple(
		byte* filterList,
		byte* defaultPath,
		out nfdpathset_t outPaths
	);
	public static unsafe nfdresult_t NFD_OpenDialogMultiple(
		string filterList,
		string defaultPath,
		out nfdpathset_t outPaths
	) {
		byte* filterListPtr = Utf8EncodeNullable(filterList);
		byte* defaultPathPtr = Utf8EncodeNullable(defaultPath);

		nfdresult_t result = INTERNAL_NFD_OpenDialogMultiple(
			filterListPtr,
			defaultPathPtr,
			out outPaths
		);

		Marshal.FreeHGlobal((IntPtr) filterListPtr);
		Marshal.FreeHGlobal((IntPtr) defaultPathPtr);
		return result;
	}

	[DllImport(nativeLibName, EntryPoint = "NFD_SaveDialog", CallingConvention = CallingConvention.Cdecl)]
	private static extern unsafe nfdresult_t INTERNAL_NFD_SaveDialog(
		byte* filterList,
		byte* defaultPath,
		out IntPtr outPath
	);
	public static unsafe nfdresult_t NFD_SaveDialog(
		string filterList,
		string defaultPath,
		out string outPath
	) {
		byte* filterListPtr = Utf8EncodeNullable(filterList);
		byte* defaultPathPtr = Utf8EncodeNullable(defaultPath);
		IntPtr outPathPtr;

		nfdresult_t result = INTERNAL_NFD_SaveDialog(
			filterListPtr,
			defaultPathPtr,
			out outPathPtr
		);

		Marshal.FreeHGlobal((IntPtr) filterListPtr);
		Marshal.FreeHGlobal((IntPtr) defaultPathPtr);
		outPath = UTF8_ToManaged(outPathPtr, true);
		return result;
	}

	[DllImport(nativeLibName, EntryPoint = "NFD_PickFolder", CallingConvention = CallingConvention.Cdecl)]
	private static extern unsafe nfdresult_t INTERNAL_NFD_PickFolder(
		byte* defaultPath,
		out IntPtr outPath
	);
	public static unsafe nfdresult_t NFD_PickFolder(
		string defaultPath,
		out string outPath
	) {
		byte* defaultPathPtr = Utf8EncodeNullable(defaultPath);
		IntPtr outPathPtr;

		nfdresult_t result = INTERNAL_NFD_PickFolder(
			defaultPathPtr,
			out outPathPtr
		);

		Marshal.FreeHGlobal((IntPtr) defaultPathPtr);
		outPath = UTF8_ToManaged(outPathPtr, true);
		return result;
	}

	[DllImport(nativeLibName, EntryPoint = "NFD_GetError", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr INTERNAL_NFD_GetError();
	public static string NFD_GetError()
	{
		return UTF8_ToManaged(INTERNAL_NFD_GetError());
	}

	/* IntPtr refers to a size_t */
	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr NFD_PathSet_GetCount(ref nfdpathset_t pathset);

	[DllImport(nativeLibName, EntryPoint = "NFD_PathSet_GetPath", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr INTERNAL_NFD_PathSet_GetPath(
		ref nfdpathset_t pathset,
		IntPtr index /* size_t */
	);
	public static string NFD_PathSet_GetPath(
		ref nfdpathset_t pathset,
		IntPtr index /* size_t */
	) {
		return UTF8_ToManaged(INTERNAL_NFD_PathSet_GetPath(ref pathset, index));
	}

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	public static extern void NFD_PathSet_Free(ref nfdpathset_t pathset);

	#endregion
}
