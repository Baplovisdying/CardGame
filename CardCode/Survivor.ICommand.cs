// Survivor.ICommand
using System;
using System.Collections;
using Survivor;

public interface ICommand
{
	float duration { get; }

	CommandWaitRate nextCmdWaitRate { get; }

	IEnumerator Execute(Action<ICommand> onComplete = null);
}
