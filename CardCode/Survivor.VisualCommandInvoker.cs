// Survivor.VisualCommandInvoker
using System.Collections;
using System.Collections.Generic;
using Survivor;
using UnityEngine;

public class VisualCommandInvoker : MonoBehaviour
{
	public Queue<ICommand> commands = new Queue<ICommand>();

	public List<ICommand> playingCmds = new List<ICommand>();

	public void ResetVisualCommandInvoker()
	{
		StopAllCoroutines();
		commands.Clear();
	}

	public void AddCommand(ICommand command)
	{
		commands.Enqueue(command);
	}

	public void ClearPlayingCmds()
	{
		playingCmds.Clear();
	}

	public IEnumerator YieldExecuteAllCommands()
	{
		float lastCmdLeftDuration = 0f;
		float waitDuration = 0f;
		while (commands.Count > 0)
		{
			ICommand command = commands.Dequeue();
			if (command == null)
			{
				continue;
			}
			if (command.nextCmdWaitRate == CommandWaitRate.Wait)
			{
				yield return command.Execute();
				waitDuration = 0.5f;
			}
			else
			{
				playingCmds.Add(command);
				StartCoroutine(command.Execute(OnCommandComplete));
				if (commands.Count != 0)
				{
					switch (command.nextCmdWaitRate)
					{
					case CommandWaitRate.Long:
						waitDuration = command.duration;
						break;
					case CommandWaitRate.Normal:
						waitDuration = command.duration * 0.5f;
						break;
					case CommandWaitRate.Short:
						waitDuration = Random.Range(0.05f, 0.1f);
						break;
					}
				}
				else
				{
					waitDuration = command.duration + 0.01f;
				}
				float num = command.duration - waitDuration;
				lastCmdLeftDuration -= waitDuration;
				if (lastCmdLeftDuration < 0f)
				{
					lastCmdLeftDuration = 0f;
				}
				if (num > lastCmdLeftDuration)
				{
					lastCmdLeftDuration = num;
				}
			}
			if (waitDuration > 0f)
			{
				yield return new WaitForSeconds(waitDuration);
			}
		}
		if (lastCmdLeftDuration > 0f)
		{
			Debug.Log("lastCmdLeftDuration:" + lastCmdLeftDuration);
			yield return new WaitForSeconds(lastCmdLeftDuration);
		}
		yield return new WaitForEndOfFrame();
	}

	private void OnCommandComplete(ICommand completedCmd)
	{
		playingCmds.Remove(completedCmd);
	}

	public IEnumerator YieldExecuteAllCommandsStepByStep()
	{
		while (commands.Count > 0)
		{
			ICommand command = commands.Dequeue();
			if (command != null)
			{
				yield return command.Execute();
			}
		}
	}
}
