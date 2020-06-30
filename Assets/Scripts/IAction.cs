/// <summary>
/// 記録保存のインターフェイス
/// </summary>
public interface IAction
{
    //記録保存と変更
    void ExecuteCommand();

    //保存した記録を呼び戻す
    void UndoCommand();
}
