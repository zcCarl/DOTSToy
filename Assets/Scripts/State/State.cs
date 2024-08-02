namespace State
{
    public abstract class State<T> where T : IContext
    {

        public virtual void Enter(T context) { }
        public virtual void Exit(T context) { }
        public virtual void Update(T context) { }
    }
}