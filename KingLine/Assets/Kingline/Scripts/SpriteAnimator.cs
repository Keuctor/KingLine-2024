using System;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public enum MoveDirection
    {
        RIGHT,
        LEFT,
        UP,
        DOWN
    }

    [SerializeField]
    private SpriteRenderer m_renderer;

    [SerializeField]
    private int m_idleIndex;

    [SerializeField]
    private CharacterSpriteResource m_characterSpriteResource;

    [SerializeField]
    private bool m_play;

    private Sprite[] m_current;

    private MoveDirection m_currentMoveDirection;

    private int m_frameIndex;

    private MoveDirection m_moveDirection;

    private float timer;

    private readonly float timerInterval = 0.15f;

    private void Start()
    {
        m_current = m_characterSpriteResource.Right;
    }

    public void SetPlay(bool play)
    {
        if (m_play == play)
            return;
        
        m_play = play;
        if (!m_play) m_renderer.sprite = m_current[m_idleIndex];
    }


    public void SetDirection(MoveDirection moveDirection)
    {
        if (m_currentMoveDirection == moveDirection)
            return;

        m_currentMoveDirection = moveDirection;
        m_frameIndex = m_idleIndex;
        switch (moveDirection)
        {
            case MoveDirection.UP:
                m_current = m_characterSpriteResource.Up;
                break;
            case MoveDirection.DOWN:
                m_current = m_characterSpriteResource.Down;
                break;
            case MoveDirection.RIGHT:
                m_current = m_characterSpriteResource.Right;
                break;
            case MoveDirection.LEFT:
                m_current = m_characterSpriteResource.Left;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(moveDirection), moveDirection, null);
        }

        m_renderer.sprite = m_current[m_idleIndex];
    }

    public void Animate()
    {
        if (m_play)
        {
            timer += Time.deltaTime;
            if (timer >= timerInterval)
            {
                m_renderer.sprite = m_current[m_frameIndex++];
                if (m_frameIndex >= m_current.Length)
                    m_frameIndex = 0;
                timer = 0f;
            }
        }
        else
        {
            m_frameIndex = 0;
            m_renderer.sprite = m_current[m_idleIndex];
        }
    }
}