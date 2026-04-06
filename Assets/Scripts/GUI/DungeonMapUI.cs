using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class DungeonMapUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_titleText;

    [Header("Cell Settings")]
    [SerializeField] private Color m_emptyCellColor;
    [SerializeField] private Color m_roomCellColor;

    [Space]
    [SerializeField] private Sprite m_playerSprite;
    [SerializeField] private Sprite m_bossSprite;
    [SerializeField] private Sprite m_chestSprite;

    private GridLayoutGroup m_mapGridLayout;
    private Image[,] m_mapGrid = new Image[DungeonData.MapWidth, DungeonData.MapHeight];

    public void ConfigureMapDisplay(DungeonData dungeonData)
    {
        m_titleText.text = dungeonData.DungeonName;

        int currentX = 0;
        int currentY = 0;
        var gridChildren = GetComponentsInChildren<Image>();

        foreach (var child in gridChildren)
        {
            if (child == this)
                continue;

            m_mapGrid[currentX, currentY] = child;
            SetMapCell(currentX, currentY, dungeonData.GetCellDisplay(currentX, currentY));

            currentX++;
            if (currentX >= DungeonData.MapWidth)
            {
                currentX = 0;
                currentY++;

                if (currentY >= DungeonData.MapHeight)
                    break;
            }
        }
    }

    public void SetMapCell(int x, int y, DungeonData.MapCellType cellType)
    {
        switch (cellType)
        {
            case DungeonData.MapCellType.Room:
                SetMapCell(x, y, color: m_roomCellColor, sprite: null);
                break;

            case DungeonData.MapCellType.Player:
                SetMapCell(x, y, color: Color.white, sprite: m_playerSprite);
                break;

            case DungeonData.MapCellType.Boss:
                SetMapCell(x, y, color: Color.white, sprite: m_bossSprite);
                break;

            default:
                SetMapCell(x, y, color: m_emptyCellColor, sprite: null);
                break;
        }

    }

    private void SetMapCell(int x, int y, Color color, Sprite sprite)
    {
        m_mapGrid[x, y].color = color;
        m_mapGrid[x, y].sprite = sprite;
    }
}
