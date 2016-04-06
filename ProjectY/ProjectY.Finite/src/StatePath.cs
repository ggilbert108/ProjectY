namespace ProjectY.Finite
{
    public class StatePath
    {
        public IState State;
        public string Recorded;
        public bool Marked;

        public StatePath(IState state)
        {
            State = state;
            Recorded = "";
            Marked = false;
        }
    }
}