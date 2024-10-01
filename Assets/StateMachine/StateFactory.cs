using System.Collections.Generic;

enum States 
{ 
    None
}

public class StateFactory
{
    ContextManager _contextManager;
    Dictionary<States, AbstractState> _states = new Dictionary<States, AbstractState>();

    public StateFactory(ContextManager currentContextManager)
    {
       _contextManager = currentContextManager;
       _states[States.None] = new ConcreteState(_contextManager, this);
    }

    public AbstractState State()
    {
        return _states[States.None];
    }
}
