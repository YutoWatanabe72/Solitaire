/// <summary>
/// カードのスプライト変更の記録を保存
/// </summary>
public class CardFlipCommand : IAction
{
    private CanSelect canSelect;

    public CardFlipCommand(CanSelect _canSelect)
    {
        canSelect = _canSelect;
    }

    public void ExecuteCommand()
    {
        canSelect.turn = true;

    }
    public void UndoCommand()
    {
        canSelect.turn = false;
    }
}
