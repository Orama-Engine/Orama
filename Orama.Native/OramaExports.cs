using Orama.Entities;
using System.Runtime.InteropServices;

namespace Orama.Native;

/// <summary>
/// C-Style API Exports.
/// </summary>
public static class OramaExports
{
	[UnmanagedCallersOnly(EntryPoint = "orama_entity_create")]
	public static IntPtr EntityCreate()
	{
		Entity entity = new();
		GCHandle handle = GCHandle.Alloc(entity, GCHandleType.Normal);
		return GCHandle.ToIntPtr(handle);
	}

	[UnmanagedCallersOnly(EntryPoint = "orama_entity_destroy")]
	public static void EntityDestroy(IntPtr entity)
	{
		GCHandle handle = GCHandle.FromIntPtr(entity);
		handle.Free();
	}

	[UnmanagedCallersOnly(EntryPoint = "orama_entity_get_name")]
	public static IntPtr EntityGetName(IntPtr entityPtr)
	{
		Entity entity = (Entity)GCHandle.FromIntPtr(entityPtr).Target!;

		// Convert the Name string to UTF-8 bytes
		byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(entity.Name + '\0');

		// Allocate unmanaged memory and copy the bytes
		IntPtr unmanagedPtr = Marshal.AllocHGlobal(utf8.Length);
		Marshal.Copy(utf8, 0, unmanagedPtr, utf8.Length);

		return unmanagedPtr;
	}

	[UnmanagedCallersOnly(EntryPoint = "orama_entity_set_name")]
	public static void EntitySetName(IntPtr entityPtr, IntPtr namePtr)
	{
		Entity entity = (Entity)GCHandle.FromIntPtr(entityPtr).Target!;
		entity.Name = Marshal.PtrToStringAnsi(namePtr);
	}
}
