using System;
using System.Linq;
using UnityEngine;

public static class XonixUtils
{
    public static void UpdateAnimator(Animator animator, MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.None:
                SetAnimatorParams(animator, 0, 0, 0);
                break;
            case MovementDirection.Up:
                SetAnimatorParams(animator, 0, 1, 1);
                break;
            case MovementDirection.Down:
                SetAnimatorParams(animator, 0, -1, 1);
                break;
            case MovementDirection.Left:
                SetAnimatorParams(animator, -1, 0, 1);
                break;
            case MovementDirection.Right:
                SetAnimatorParams(animator, 1, 0, 1);
                break;
        }
    }

    public static void UpdateAnimator(Animator animator, SwipeDirection direction)
    {
        UpdateAnimator(
            animator
            , (MovementDirection)Enum.Parse(typeof(MovementDirection), direction.ToString()));
    }

    public static void SetAnimatorParams(Animator animator, float horizontal, float vertical, float speed)
    {
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
        animator.SetFloat("Speed", speed);
    }


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
