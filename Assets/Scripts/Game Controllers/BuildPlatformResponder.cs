using UnityEngine;
using UnityEngine.Events;

public class BuildPlatformResponder : MonoBehaviour
{
    [SerializeField]
    private RuntimePlatform _buildPlatform;

    [SerializeField]
    private UnityEvent _actionOnBuildPlatform;

    [SerializeField]
    private UnityEvent _actionOnOtherBuildPlatforms;

    private void Awake()
    {
        if (Application.platform == _buildPlatform)
        {
            if (_actionOnBuildPlatform != null)
            {
                _actionOnBuildPlatform.Invoke();
            }
        }
        else
        {
            if (_actionOnOtherBuildPlatforms != null)
            {
                _actionOnOtherBuildPlatforms.Invoke();
            }
        }
    }
}
