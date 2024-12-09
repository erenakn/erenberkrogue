using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private int m_FoodAmount = 100;
    public static GameManager Instance { get; private set; }

    public BoardManager BoardManager;
    public PlayerCharacter PlayerController;
    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;
    public UIDocument UIDoc;
    private Label m_FoodLabel;
    private int m_CurrentLevel = 1;
    public TurnManager TurnManager { get; private set; }
    public int foodPoints = 100; // Oyuncunun yiyecek puan�
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");

        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        StartNewGame();
    }
    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));

        m_CurrentLevel++;
    }
    void OnTurnHappen()
    {
        m_FoodAmount -= 1;
        m_FoodLabel.text = "Food : " + m_FoodAmount;
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        if (m_FoodAmount <= 0)
        {
            PlayerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = "Game Over!\n\nSurvived " + m_CurrentLevel + " days";
        }
    }
    public void StartNewGame()
    {
        m_GameOverPanel.style.visibility = Visibility.Hidden;

        m_CurrentLevel = 1;
        m_FoodAmount = 20;
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        BoardManager.Clean();
        BoardManager.Init();

        PlayerController.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
    }
    // Etiketi g�ncelleyen bir yard�mc� y�ntem
    private void UpdateFoodLabel()
    {
        if (m_FoodLabel != null)
        {
            m_FoodLabel.text = "Food: " + foodPoints;
        }
    }
}
