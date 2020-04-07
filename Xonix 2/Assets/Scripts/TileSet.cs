using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileSet", menuName = "Tile set")]
public class TileSet : ScriptableObject
{
    public Tile playerTrack;

    [Space]
    public Tile water;
    [Space]
    public Tile waterLeft;
    public Tile waterUp;
    public Tile waterRight;
    public Tile waterDown;
    [Space]
    public Tile waterLeftUp;
    public Tile waterLeftDown;
    public Tile waterRightUp;
    public Tile waterRightDown;
    [Space]
    public Tile waterOutLeftUp;
    public Tile waterOutLeftDown;
    public Tile waterOutRightUp;
    public Tile waterOutRightDown;
}
