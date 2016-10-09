namespace Assets.Scripts
{
    interface ICharacter
    {
        int GetInitiative();
        bool IsMyTurn();
        void IsMyTurn(bool set);
        void ReceiveActPts();
        int GetCurrentActionPoints();
    }
}
