namespace Game.Systems.DialogueSystem
{
	public static class Talker
	{
		public static void ABTalk(IActor initiator, IActor actor)
		{
			initiator.TalkWith(actor);
		}
	}
}