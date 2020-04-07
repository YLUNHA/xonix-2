using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private float moveSpeed = 3f;
    private Vector3 movement;
    private SwipeDirection direction = SwipeDirection.None;

    public static event Action OnPlayerDamaged = delegate { };
    public static event Action<Vector2> OnTileTrack = delegate { };

    private float playerDead = 0f;
    private TilemapUtil tilemapUtil;

    void Start()
    {
        animator = GetComponent<Animator>();
        tilemapUtil = GameObject.FindGameObjectWithTag("GameController").GetComponent<TilemapUtil>();

        SwipeDetector.OnSwipe += OnSwipe;
    }

    private void OnDestroy()
    {
        SwipeDetector.OnSwipe -= OnSwipe;
    }

    void Update()
    {
        HandleAxisControl();
    }

    private void FixedUpdate()
    {
        if (direction != SwipeDirection.None)
        {
            if (tilemapUtil.CanMove(NextPosition(), CharacterType.Player))
            {
                OnTileTrack(transform.position);
                transform.position = (transform.position + movement * moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                // TODO: Stun player
            }
        }
    }


    private void HandleAxisControl()
    {
        if (playerDead > 0)
        {
            playerDead -= Time.deltaTime;
            return;
        }

        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        if (x != 0 || y != 0)
        {
            if (x > 0)
            {
                UpdateDirection(SwipeDirection.Right);
            }
            else if (x < 0)
            {
                UpdateDirection(SwipeDirection.Left);
            }
            else if (y > 0)
            {
                UpdateDirection(SwipeDirection.Up);
            }
            else if (y < 0)
            {
                UpdateDirection(SwipeDirection.Down);
            }
        }
    }

    private void OnSwipe(SwipeData swipeData)
    {
        if (playerDead > 0)
        {
            playerDead -= Time.deltaTime;
            return;
        }

        UpdateDirection(swipeData.Direction);
    }


    private void UpdateDirection(SwipeDirection swipeDirection)
    {
        if (GameController.isPause)
        {
            return;
        }

        if (IsDirectionConflict(swipeDirection))
        {
            movement.x = 0;
            movement.y = 0;
            direction = SwipeDirection.None;

            // Decrement player health
            OnPlayerDamaged();
        }
        else
        {
            movement.x = 0;
            movement.y = 0;

            direction = swipeDirection;

            switch (direction)
            {
                case SwipeDirection.Up:
                    movement.y = 1;
                    break;
                case SwipeDirection.Down:
                    movement.y = -1;
                    break;
                case SwipeDirection.Left:
                    movement.x = -1;
                    break;
                case SwipeDirection.Right:
                    movement.x = 1;
                    break;
            }
        }

        XonixUtils.UpdateAnimator(animator, direction);
    }

    private Vector2 NextPosition()
    {
        var nextDirection = transform.position;

        switch (direction)
        {
            case SwipeDirection.Up:
                nextDirection.y++;
                break;

            case SwipeDirection.Down:
                nextDirection.y--;
                break;

            case SwipeDirection.Left:
                nextDirection.x--;
                break;

            case SwipeDirection.Right:
                nextDirection.x++;
                break;
        }

        return nextDirection;
    }

    private bool IsDirectionConflict(SwipeDirection swipeDirection)
    {
        switch (swipeDirection)
        {
            case SwipeDirection.None:
                return false;
            case SwipeDirection.Up:
                return direction == SwipeDirection.Down;
            case SwipeDirection.Down:
                return direction == SwipeDirection.Up;
            case SwipeDirection.Left:
                return direction == SwipeDirection.Right;
            case SwipeDirection.Right:
                return direction == SwipeDirection.Left;
        }

        return false;
    }

    public static void DamagePlayer()
    {
        OnPlayerDamaged();
    }

    public void RespawnPlayer()
    {
        playerDead = 0.3f;

        movement.x = 0;
        movement.y = 0;

        direction = SwipeDirection.None;
        transform.position = new Vector3(0, -11, 0);

        XonixUtils.UpdateAnimator(animator, direction);
    }

}
