using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class DungeonMapUI : MonoBehaviour
{
    [SerializeField] private Color m_emptyCellColor;
    [SerializeField] private Color m_roomCellColor;

    [Space]
    [SerializeField] private Sprite m_playerSprite;
    [SerializeField] private Sprite m_bossSprite;
    [SerializeField] private Sprite m_chestSprite;

    private const int MapWidth  = 8;
    private const int MapHeight = 8;

    private GridLayoutGroup m_mapGridLayout;
    private Image[,] m_mapGrid = new Image[MapWidth, MapHeight];

    private void Awake()
    {
        m_mapGridLayout = GetComponent<GridLayoutGroup>();
        var gridChildren = GetComponentsInChildren<Image>();

        int currentX = 0;
        int currentY = 0;
        foreach(var child in gridChildren)
        {
            if (child == this)
                continue;

            m_mapGrid[currentX, currentY] = child;

            if(currentX %2 == 0)
                SetMapCell(currentX, currentY, color: Color.white, sprite: m_chestSprite);
            else if(currentY %2 == 0)
                SetMapCell(currentX, currentY, color: Color.white, sprite: m_bossSprite);
            else
                SetMapCell(currentX, currentY, color: m_emptyCellColor, sprite: null);

            currentX++;
            if (currentX >= MapWidth)
            {
                currentX = 0;
                currentY++;

                if (currentY >= MapHeight)
                    break;
            }
        }
    }

    private void SetMapCell(int x, int y, Color color, Sprite sprite)
    {
        m_mapGrid[x, y].color = color;
        m_mapGrid[x, y].sprite = sprite;

        Debug.Log($"Setting Cell [{x},{y}] to [{color}]");
    }
}
