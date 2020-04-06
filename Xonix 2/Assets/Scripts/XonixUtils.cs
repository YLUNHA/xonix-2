﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class XonixUtils
{
    public static MovementDirection[] GetDiagonalDirections(MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.Up:
                return new MovementDirection[] { MovementDirection.LeftUp, MovementDirection.RightUp };

            case MovementDirection.Down:
                return new MovementDirection[] { MovementDirection.LeftDown, MovementDirection.RightDown };

            case MovementDirection.Left:
                return new MovementDirection[] { MovementDirection.LeftUp, MovementDirection.LeftDown };

            case MovementDirection.Right:
                return new MovementDirection[] { MovementDirection.RightUp, MovementDirection.RightDown };
        }

        return new MovementDirection[] { };
    }

    public static MovementDirection[] GetXYDirections()
    {
        return new MovementDirection[] { MovementDirection.Left, MovementDirection.Up, MovementDirection.Right, MovementDirection.Down };
    }

    public static MovementDirection GetDiagonalDirectionOfXY(MovementDirection x, MovementDirection y)
    {
        if (Enum.GetNames(typeof(MovementDirection)).Contains(x.ToString() + y.ToString()))
        {
            return (MovementDirection)Enum.Parse(typeof(MovementDirection), x.ToString() + y.ToString());
        }
        else
        {
            return (MovementDirection)Enum.Parse(typeof(MovementDirection), y.ToString() + x.ToString());
        }
    }

    public static Vector3Int GetNextPosition(Vector3Int curentPosition, MovementDirection direction)
    {
        var pos = curentPosition;

        switch (direction)
        {
            case MovementDirection.Left:
                pos.x--;
                break;

            case MovementDirection.Up:
                pos.y++;
                break;

            case MovementDirection.Right:
                pos.x++;
                break;

            case MovementDirection.Down:
                pos.y--;
                break;

            case MovementDirection.LeftUp:
                pos.x--;
                pos.y++;
                break;

            case MovementDirection.LeftDown:
                pos.x--;
                pos.y--;
                break;

            case MovementDirection.RightUp:
                pos.x++;
                pos.y++;
                break;

            case MovementDirection.RightDown:
                pos.x++;
                pos.y--;
                break;
        }

        return pos;
    }

}
