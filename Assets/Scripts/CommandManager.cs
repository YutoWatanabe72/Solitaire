using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 変更記録の保存
/// </summary>
public class CommandManager : MonoBehaviour
{
    public GameObject flipedCards;
    private Stack<IAction> historyStack = new Stack<IAction>();

    /// <summary>
    /// 変更を記録
    /// </summary>
    /// <param name="action"></param>
    public void ExecuteCommand(IAction action)
    {
        action.ExecuteCommand();
        historyStack.Push(action);
    }

    /// <summary>
    /// 記録した状態に戻す
    /// </summary>
    public void UndoCommand()
    {
        if(historyStack.Count > 0)
        {
            historyStack.Pop().UndoCommand();
        }
    }

    /// <summary>
    /// 一時保管していたカードオブジェクトを削除
    /// 記録の削除
    /// </summary>
    public void ResetCommand()
    {
        foreach (Transform child in flipedCards.transform)
        {
            if (child.CompareTag("Card"))
            {
                Destroy(child.gameObject);
            }
        }
        if (historyStack.Count > 0)
        {
            historyStack.Clear();
        }
    }
}
