namespace State
{
    public abstract class StateMachine<T> where T : IContext
    {
        protected T Context;

        private State<T> currentState;

        protected StateMachine(T context)
        {
            Context = context;
        }

        public void ChangeState(State<T> newState)
        {
            if (currentState != null)
            {
                currentState.Exit(Context);
            }

            currentState = newState;
            currentState.Enter(Context);
        }

        public void Update()
        {
            if (currentState != null)
            {
                currentState.Update(Context);
            }
        }
    }
}