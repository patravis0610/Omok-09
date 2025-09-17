using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void OnClickMultiPlayer()
    {
        AuthPopup.Instance?.Open();
    }
}
