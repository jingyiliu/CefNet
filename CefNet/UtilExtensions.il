.class auto ansi abstract sealed beforefieldinit CefNet.UtilsExtensions
{
	.method public hidebysig static void InitBlock(native int startAddress, uint8 'value', int32 size)
		cil managed aggressiveinlining 
	{
		.maxstack 3

		ldarg.0
		ldarg.1
		ldarg.2
		unaligned. 1
		initblk
		ret
	}

}