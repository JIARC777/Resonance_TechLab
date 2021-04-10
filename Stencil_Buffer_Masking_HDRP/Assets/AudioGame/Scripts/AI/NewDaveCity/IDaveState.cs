namespace AudioGame.Scripts.AI
{
    public interface IDaveState
    {
        void OnStateEnter(DAVE bigDave);
        void OnStateExit(DAVE bigDave);
        void OnStateUpdate(DAVE bigDave);
        void OnStateMove(DAVE bigDave);
    }
}