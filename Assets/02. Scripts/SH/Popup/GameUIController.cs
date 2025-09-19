using TMPro;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject player_BalckTurnPanel;
    [SerializeField] private GameObject player_WhiteTurnPanel;

    [SerializeField] private TMP_Text gameType_Txt;

    public enum GameTurnPanelType { None, Balck_Turn, White_Turn }

    public void OnClickBackButton()
    {
        GameManager.Instance.OpenConfirmPanel("Close Game?", () =>
             {
                 GameManager.Instance.ChangeToMainScene();
             });
    }

    public void SetGameTitle(Constants.GameType gameType)
    {
        gameType_Txt.text = gameType.ToString();
        
    }

    public void SetGameTurnPanel(GameTurnPanelType gameTurnPanelType)
    {
        switch (gameTurnPanelType)
        {
            case GameTurnPanelType.None:
                player_BalckTurnPanel.SetActive(false);
                player_WhiteTurnPanel.SetActive(false);
                break;
            case GameTurnPanelType.Balck_Turn:
                player_BalckTurnPanel.SetActive(true);
                player_WhiteTurnPanel.SetActive(false);
                break;
            case GameTurnPanelType.White_Turn:
                player_BalckTurnPanel.SetActive(false);
                player_WhiteTurnPanel.SetActive(true);
                break;
        }
    }
}
