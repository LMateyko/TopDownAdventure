
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonData", menuName = "Adventure Game Data/DungeonData")]
public class DungeonData : ScriptableObject
{
    public static int MapWidth = 8;
    public static int MapHeight = 8;

    public enum MapCellType
    {
        Empty       = 0,
        Room        = 1,
        Player      = 2,
        Boss        = 3
    }

    public string DungeonName = "Lost Caverns";
    [TextArea(minLines: 7, maxLines: 7)]
    public string BasicDungeonMap = "";

    public Vector2Int PlayerStart => GetPlayerStart();

    [Range(0, 7)] public int testCellX = 0;
    [Range(0, 7)] public int testCellY = 0;

    public string testSubString;
    public MapCellType testCellDisplay = MapCellType.Empty;

    public int playerStartX = 0;
    public int playerStartY = 0;

    public MapCellType GetCellDisplay(int x, int y)
    {
        testSubString = BasicDungeonMap.Substring(x + (y * (MapWidth + 1)), 1);

        int testSubStringInt = 0;
        int.TryParse(testSubString, out testSubStringInt);

        return (MapCellType)testSubStringInt;
    }

    private void OnValidate()
    {
        testCellDisplay = GetCellDisplay(testCellX, testCellY);

        var playerStart = GetPlayerStart();
        playerStartX = (int)playerStart.x;
        playerStartY = (int)playerStart.y;
    }

    private Vector2Int GetPlayerStart()
    {
        var index = BasicDungeonMap.IndexOf(((int)MapCellType.Player).ToString());

        if(index > 0)
        {
            var posX = index % (MapWidth + 1);
            var posY = index / (MapWidth + 1);

            return new Vector2Int(posX, posY);
        }

       return Vector2Int.zero;
    }
}
