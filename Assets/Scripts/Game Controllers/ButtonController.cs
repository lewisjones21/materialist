using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonController : MonoBehaviour
{
    [SerializeField]
    private AudioClip _soundMouseEnter, _soundMouseExit;

    public void OnPointerEnter()
    {
        if (enabled && _soundMouseEnter != null)
        {
            AudioSource.PlayClipAtPoint(_soundMouseEnter, Vector3.zero, 1f);
        }
    }

    public void OnPointerExit()
    {
        if (enabled && _soundMouseExit != null)
        {
            AudioSource.PlayClipAtPoint(_soundMouseExit, Vector3.zero, 1f);
        }
    }
}
