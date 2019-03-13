using UnityEngine;
using System.Threading;

public class ArtificialIntelligence : MonoBehaviour, ICommandBase
{
    private ICommandBase AIThreadMethod = null;
    private Thread AIThread;

    public void Init(ICommandBase _AIThreadMethod)
    {
        AIThreadMethod = _AIThreadMethod;
        AIThread = new Thread(AIThreadMethod.Excute);
    }

    public void Excute()
    {
        AIThread.Start();
    }
}
