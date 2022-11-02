// Survivor.CardSchoolType
using System;

[Flags]
public enum CardSchoolType
{
	None = 0,
	Tired = 1,
	Hemorrhage = 2,
	Block = 4,
	Hunter = 8,
	Burn = 0x10,
	Focus = 0x20
}
