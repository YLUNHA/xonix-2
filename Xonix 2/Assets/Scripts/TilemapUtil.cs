using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapUtil : MonoBehaviour
{
    public SoundController soundController;

    [Space]
    public Tilemap groundTilemap;
    public Tilemap waterTilemap;
    public Tilemap playerTrackTilemap;

    public Tile playerTrackTile;

    [Header("Water tiles")]
    public Tile waterTile;

    [Space]
    public Tile waterUpTile;
    public Tile waterDownTile;

    [Space]
    public Tile waterLeftTile;
    public Tile waterLeftUpTile;
    public Tile waterLeftDownTile;
    public Tile waterOutLeftUpTile;
    public Tile waterOutLeftDownTile;

    [Space]
    public Tile waterRightTile;
    public Tile waterRightUpTile;
    public Tile waterRightDownTile;
    public Tile waterOutRightUpTile;
    public Tile waterOutRightDownTile;


    private int waterCount = 0;
    private int waterCountOrigin;

    private Vector3Int lastPlayerStep;

    private List<Vector3Int> playerTrackList = new List<Vector3Int>();
    private Dictionary<Vector3Int, char> waterTiles = new Dictionary<Vector3Int, char>();

    public static event Action<float> OnCalcLevelProgress = delegate { };
    public static event Action<CharacterType> OnEnemyDestroy = delegate { };

    private void Awake()
    {
        InitiateTileBase();

        PlayerMovement.OnTileTrack += PlayerTileTrack;
    }

    private void OnDestroy()
    {
        PlayerMovement.OnTileTrack -= PlayerTileTrack;
    }


    private void InitiateTileBase()
    {
        BoundsInt boundsInt = waterTilemap.cellBounds;
        TileBase[] tileBase = waterTilemap.GetTilesBlock(boundsInt);

        for (int x = boundsInt.min.x; x < boundsInt.max.x; x++)
        {
            for (int y = boundsInt.min.y; y < boundsInt.max.y; y++)
            {
                var position = new Vector3Int(x, y, 0);

                if (waterTilemap.HasTile(position))
                {
                    waterTiles.Add(position, 'w');
                    waterCount++;
                }
            }
        }

        waterCountOrigin = waterCount;
    }


    private void PlayerTileTrack(Vector2 position)
    {
        Vector3Int intPosition = Vector2ToVector3Int(position, CharacterType.Player);

        if (intPosition != lastPlayerStep)
        {
            lastPlayerStep = intPosition;

            bool hasTrackTile = playerTrackTilemap.HasTile(intPosition);

            // If stap on the track - player dead
            if (hasTrackTile)
            {
                PlayerMovement.DamagePlayer();
                ClearPlayerTrack();
                return;
            }

            bool hasWaterTile = waterTilemap.HasTile(intPosition);

            // If it's water - set player track.
            if (hasWaterTile)
            {
                playerTrackTilemap.SetTile(intPosition, playerTrackTile);
                playerTrackList.Add(intPosition);
            }
            // That means it is ground
            else if (playerTrackList.Count > 0)
            {
                soundController.CutWater();
                CutSelectedWaterArea();
                CalcLevelProgress();
            }
        }
    }

    public void ClearPlayerTrack(bool clearList = true)
    {
        foreach (var item in playerTrackList)
        {
            playerTrackTilemap.SetTile(item, null);
        }

        if (clearList)
        {
            playerTrackList.Clear();
        }
    }

    public bool CanMove(Vector2 position, CharacterType type)
    {
        bool result = false;
        Vector3Int intPosition = Vector2ToVector3Int(position, type);

        switch (type)
        {
            case CharacterType.Player: // Walk on ground and water
                result = groundTilemap.HasTile(intPosition);
                break;

            case CharacterType.EnemyWater: // Walk on water
                result = waterTilemap.HasTile(intPosition);

                break;

            case CharacterType.EnemyGround: // Walk on ground
                result = groundTilemap.HasTile(intPosition) && !waterTilemap.HasTile(intPosition);
                break;
        }

        return result;
    }

    public bool TryAtackPlayerTrack(Vector2 position)
    {
        if (playerTrackTilemap.HasTile(Vector2ToVector3Int(position, CharacterType.Emeny)))
        {
            PlayerMovement.DamagePlayer();
            ClearPlayerTrack();
            return true;
        }

        return false;
    }


    private void CalcLevelProgress()
    {
        float progress = 100 - (((float)waterCount / waterCountOrigin) * 100);
        OnCalcLevelProgress(progress);
    }

    private void CutSelectedWaterArea()
    {
        int selectedWaterCount = 0;

        foreach (var item in playerTrackList)
        {
            waterTiles[item] = 'd';
            waterCount--;
            waterTilemap.SetTile(item, null);
        }

        Vector3Int centerOfSelectedArea = FindCenterOfSelectedWaterArea();

        ClearPlayerTrack(false);
        // -

        Stack<Vector3Int> positions = new Stack<Vector3Int>();
        List<Vector3Int> tilesToFill = new List<Vector3Int>();
        positions.Push(centerOfSelectedArea);

        while (positions.Count > 0)
        {
            var position = positions.Pop();

            if (waterTiles.ContainsKey(position)
                && waterTiles[position] == 'w')
            {
                waterTiles[position] = 's';
                tilesToFill.Add(position);
                selectedWaterCount++;

                positions.Push(new Vector3Int(position.x - 1, position.y, 0));
                positions.Push(new Vector3Int(position.x + 1, position.y, 0));
                positions.Push(new Vector3Int(position.x, position.y - 1, 0));
                positions.Push(new Vector3Int(position.x, position.y + 1, 0));
            }
        }

        char st = selectedWaterCount < (waterCount - selectedWaterCount) ? 's' : 'w';

        MakeCoasts(st == 's' ? 'w' : 's', true);

        if (st == 's')
        {
            foreach (var item in tilesToFill)
            {
                waterTiles[item] = 'd';
                CutWaterTile(item);
            }
        }
        else
        {
            List<Vector3Int> tilesToDelete = new List<Vector3Int>();

            foreach (var item in waterTiles)
            {
                if (item.Value == 'w')
                {
                    tilesToDelete.Add(item.Key);
                    CutWaterTile(item.Key);
                }
            }

            UpdateWaterTilesValue(tilesToDelete, 'd');
            UpdateWaterTilesValue(tilesToFill, 'w');
        }
    }

    private void MakeCoasts(char waterFlag, bool clearTrackList)
    {
        foreach (var item in playerTrackList)
        {
            RiseOutAngleCoast(item, waterFlag);
            RiseCoast(item, waterFlag);
        }

        if (clearTrackList)
        {
            playerTrackList.Clear();
        }
    }

    private void RiseCoast(Vector3Int trackPoint, char waterFlag)
    {
        MovementDirection[] directionsXY = XonixUtils.GetXYDirections();

        foreach (var xyDirection in directionsXY)
        {
            var nextPos = XonixUtils.GetNextPosition(trackPoint, xyDirection);

            if (HasWaterTile(nextPos, waterFlag))
            {
                if (!RiseAngleCoast(nextPos, waterFlag))
                {
                    waterTilemap.SetTile(nextPos, GetCoastTileByDirection(xyDirection));
                }
            }
        }
    }

    private bool RiseAngleCoast(Vector3Int point, char waterFlag)
    {
        MovementDirection[] movementDirections = XonixUtils.GetXYDirections();

        for (int i = 0; i < movementDirections.Length; i++)
        {
            var fDirection = movementDirections[i];
            var fPosition = XonixUtils.GetNextPosition(point, fDirection);

            var sDirection = movementDirections[(i == movementDirections.Length - 1 ? 0 : i + 1)];
            var sPosition = XonixUtils.GetNextPosition(point, sDirection);

            var mDirection = XonixUtils.GetDiagonalDirectionOfXY(fDirection, sDirection);
            var mPosition = XonixUtils.GetNextPosition(point, mDirection);

            if (!HasWaterTile(fPosition, waterFlag)
                && !HasWaterTile(sPosition, waterFlag)
                && !HasWaterTile(mPosition, waterFlag))
            {
                waterTilemap.SetTile(point, GetCoastTileByDirection(mDirection));
                return true;
            }
        }

        return false;
    }

    private void RiseOutAngleCoast(Vector3Int trackPoint, char waterFlag)
    {
        MovementDirection[] movementDirections = XonixUtils.GetXYDirections();

        for (int i = 0; i < movementDirections.Length; i++)
        {
            var fDirection = movementDirections[i];
            var fPosition = XonixUtils.GetNextPosition(trackPoint, fDirection);

            var sDirection = movementDirections[(i == movementDirections.Length - 1 ? 0 : i + 1)];
            var sPosition = XonixUtils.GetNextPosition(trackPoint, sDirection);

            var mDirection = XonixUtils.GetDiagonalDirectionOfXY(fDirection, sDirection);
            var mPosition = XonixUtils.GetNextPosition(trackPoint, mDirection);

            if (HasWaterTile(fPosition, waterFlag)
                && HasWaterTile(sPosition, waterFlag)
                && HasWaterTile(mPosition, waterFlag))
            {
                waterTilemap.SetTile(fPosition, GetCoastTileByDirection(fDirection));
                waterTilemap.SetTile(mPosition, GetCoastTileByDirection(mDirection, true));
                waterTilemap.SetTile(sPosition, GetCoastTileByDirection(sDirection));
                break;
            }
        }
    }


    private TileBase GetCoastTileByDirection(MovementDirection direction, bool isOut = false)
    {
        switch (direction)
        {
            case MovementDirection.Up:
                return waterDownTile;

            case MovementDirection.Down:
                return waterUpTile;

            case MovementDirection.Left:
                return waterRightTile;

            case MovementDirection.LeftUp:
                return isOut ? waterOutLeftUpTile : waterLeftUpTile;

            case MovementDirection.LeftDown:
                return isOut ? waterOutLeftDownTile : waterLeftDownTile;

            case MovementDirection.Right:
                return waterLeftTile;

            case MovementDirection.RightUp:
                return isOut ? waterOutRightUpTile : waterRightUpTile;

            case MovementDirection.RightDown:
                return isOut ? waterOutRightDownTile : waterRightDownTile;
        }

        return waterTile;
    }

    private void CutWaterTile(Vector3Int position)
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(new Vector2(position.x, position.y), new Vector2(1, 1), 0f, new Vector2(), .3f);

        if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject.tag.Equals("Enemy"))
        {
            if (raycastHit2D.collider.gameObject.GetComponent<EnemyMovement>().characterType == CharacterType.EnemyWater)
            {
                GameObject.Destroy(raycastHit2D.collider.gameObject);
                OnEnemyDestroy(CharacterType.EnemyWater);
            }
        }

        waterCount--;
        waterTilemap.SetTile(position, null);
    }

    private void UpdateWaterTilesValue(List<Vector3Int> tilesToUpdate, char v)
    {
        foreach (var item in tilesToUpdate)
        {
            waterTiles[item] = v;
        }
    }

    private bool HasWaterTile(int x, int y)
    {
        var position = new Vector3Int(x, y, 0);

        return (waterTiles.ContainsKey(position) && waterTiles[position] == 'w');
    }

    private bool HasWaterTile(Vector3Int position, char waterFlag = 'w')
    {
        return (waterTiles.ContainsKey(position) && waterTiles[position] == waterFlag);
    }

    private Vector3Int FindCenterOfSelectedWaterArea()
    {
        Vector3Int firstPT = playerTrackList[0];
        Vector3Int lastPT = playerTrackList[playerTrackList.Count - 1];

        int x = (firstPT.x + lastPT.x) / 2;
        int y = (firstPT.y + lastPT.y) / 2;

        for (int i = 1; i < 10; i++)
        {
            if (HasWaterTile(x - i, y))
            {
                return new Vector3Int(x - i, y, 0);
            }
            else if (HasWaterTile(x + i, y))
            {
                return new Vector3Int(x + i, y, 0);
            }
            else if (HasWaterTile(x, y - i))
            {
                return new Vector3Int(x, y - i, 0);
            }
            else if (HasWaterTile(x, y + i))
            {
                return new Vector3Int(x, y + i, 0);
            }
        }

        return new Vector3Int();
    }

    private Vector3Int Vector2ToVector3Int(Vector2 position, CharacterType type)
    {
        int x = (int)Math.Round(position.x - 0.5f);
        int y = (int)Math.Round(position.y);

        if (type != CharacterType.Player)
        {
            y = (int)Math.Round(position.y - 0.5f);
        }

        return new Vector3Int(x, y, 0);
    }



    private class Coast
    {
        private MovementDirection direction;
        private Vector3Int position;

        public Coast()
        {

        }

        public Coast(MovementDirection direction, Vector3Int position)
        {
            this.direction = direction;
            this.position = position;
        }

        public Vector3Int Position { get => position; set => position = value; }
        public MovementDirection Direction { get => direction; set => direction = value; }
    }
}
