using System.Collections.Generic;
using UnityEngine;


public class EnemyMovement : MonoBehaviour
{
    public CharacterType characterType;

    private TilemapUtil tilemapUtil;
    private Animator animator;

    private MovementDirection direction = MovementDirection.None;
    private MovementDirection diagonalDirection = MovementDirection.None;

    private Vector3 movement;
    private float moveSpeed = 4f;


    void Start()
    {
        tilemapUtil = GameObject.FindGameObjectWithTag("GameController").GetComponent<TilemapUtil>();
        animator = GetComponent<Animator>();

        GetNextDirection();
        UpdateMovement();
    }

    private void FixedUpdate()
    {
        if (CanMove())
        {
            transform.position = (transform.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            UpdateMovement();
            XonixUtils.UpdateAnimator(animator, direction);
        }

        if (!TryAtackPlayer())
        {
            TryAtackPlayerTrack();
        }
    }


    private bool TryAtackPlayer()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 0f, new Vector2(), .6f);

        if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject.tag.Equals("Player"))
        {
            AtackAnimation();
            PlayerMovement.DamagePlayer();
            return true;
        }

        return false;
    }

    private bool TryAtackPlayerTrack()
    {
        if (tilemapUtil.TryAtackPlayerTrack(transform.position))
        {
            AtackAnimation();
            return true;
        }

        return false;
    }

    private void AtackAnimation()
    {
        // TODO
    }

    private bool CanMove()
    {
        return tilemapUtil.CanMove(NextDiagonalMovement(diagonalDirection, transform.position), characterType);
    }

    private void UpdateMovement()
    {
        var nextMovement = GetNextMovement(direction);

        if (nextMovement == new Vector3())
        {
            GetNextDirection();
        }
        else
        {
            movement = nextMovement;
        }
    }

    private void GetNextDirection()
    {
        int v = (UnityEngine.Random.Range(123, 54321) + (int)(transform.position.x + transform.position.y)) % 2;

        List<MovementDirection> directionX = new List<MovementDirection>();
        directionX.Add(MovementDirection.Left);
        directionX.Add(MovementDirection.Right);

        List<MovementDirection> directionY = new List<MovementDirection>();
        directionY.Add(MovementDirection.Up);
        directionY.Add(MovementDirection.Down);

        direction = directionX.Contains(direction)
            ? directionY[v]
            : directionX[v];

    }

    private Vector3 GetNextMovement(MovementDirection di)
    {
        foreach (var diagonalD in XonixUtils.GetDiagonalDirections(di))
        {
            if (tilemapUtil.CanMove(NextDiagonalMovement(diagonalD, transform.position), characterType))
            {
                diagonalDirection = diagonalD;
                return NextDiagonalMovement(diagonalD, new Vector3());
            }
        }
        return new Vector3();
    }

    private Vector3 NextDiagonalMovement(MovementDirection dd, Vector3 position)
    {
        var pos = position;

        switch (dd)
        {
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
