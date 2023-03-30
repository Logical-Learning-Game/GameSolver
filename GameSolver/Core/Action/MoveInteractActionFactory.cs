namespace GameSolver.Core.Action;

public class MoveInteractActionFactory : MoveActionFactory
{ 
    public override IGameAction CreateFrom(MoveAction moveAction)
    {
        MoveAction openDoorAction = new OpenDoorAction(moveAction);
        MoveAction collectAction = new CollectAction(openDoorAction);
        IGameAction observeConditionAction = new ObserveConditionAction(collectAction);
        return observeConditionAction;
    }
}