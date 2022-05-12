namespace Game.Systems.DialogueSystem
{
    public interface IActor
    {
        bool IsHaveSomethingToSay { get; }

        void Bark();

        //void StartDialogue();
    }
}