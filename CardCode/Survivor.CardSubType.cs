// Survivor.CardSubType
using System;

[Flags]
public enum CardSubType
{
	None = 0,
	Attack = 1,
	Defend = 2,
	Move = 4,
	Short = 8,
	Long = 0x10,
	Draw = 0x20,
	Control = 0x40
}
