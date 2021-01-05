using UnityEngine;

public class LimitFPS : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
}
