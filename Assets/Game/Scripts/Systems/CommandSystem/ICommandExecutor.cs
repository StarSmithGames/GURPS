using Game.Managers.PartyManager;

using System;

using Zenject;

namespace Game.Systems.CommandCenter
{
	public interface IExecutor { }

	public interface ICommandExecutor<COMMAND> : IExecutor where COMMAND : ICommand
	{
		void Execute(COMMAND command);
	}

	public class PartyManagerCommandExecutor : ICommandExecutor<IPartyManagerCommand>, IInitializable, IDisposable
	{
		private CommandCenter commandCenter;
		private PartyManager partyManager;

		public PartyManagerCommandExecutor(CommandCenter commandCenter, PartyManager partyManager)
		{
			this.commandCenter = commandCenter;
			this.partyManager = partyManager;
		}

		public void Execute(IPartyManagerCommand command)
		{
			command.Execute(partyManager.PlayerParty);
		}

		public void Initialize()
		{
			commandCenter.Registrator.Registrate(this);
		}

		public void Dispose()
		{
			commandCenter.Registrator.UnRegistrate(this);
		}
	}
}