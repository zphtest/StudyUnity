using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public PlayerCharacter player;
    public Button rollButton;
    public TextMeshProUGUI diceText;
    public TextMeshProUGUI infoText;
    
    void Start()
    {
        rollButton.onClick.AddListener(OnRollDice);
        infoText.text = "点击掷骰子开始游戏";
        diceText.text = "?";
    }
    
    void OnRollDice()
    {
        if (player == null)
        {
            Debug.LogError("Player未分配！");
            return;
        }
        
        int dice = Random.Range(1, 7);
        diceText.text = dice.ToString();
        infoText.text = $"骰子点数: {dice}，移动中...";
        
        rollButton.interactable = false;
        
        Debug.Log($"===== 掷骰子：{dice} =====");
        player.MoveSteps(dice);
        
        // 延迟恢复按钮
        float delay = dice * 0.5f + 0.5f;
        Invoke("EnableButton", delay);
    }
    
    void EnableButton()
    {
        rollButton.interactable = true;
        infoText.text = "移动完成！点击继续";
    }
}