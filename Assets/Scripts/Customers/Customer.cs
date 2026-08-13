using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-03
// AI assistance: Claude (autocomplete, architecture review)

public class Customer : MonoBehaviour
{
    public enum State { NeedsOrderTaken, WaitingForFood, Served }

    public Ticket Ticket { get; private set; }
    public State CurrentState { get; private set; }
    public float PatienceTimer { get; private set; }
    public int SeatIndex { get; private set; }

    private SpriteRenderer portraitRenderer;
    private BoxCollider2D boxCollider;
    private TicketBubble bubble;

    public void Init(Ticket ticket, int seatIndex, Sprite portraitSprite, Vector3 portraitOffset, float portraitScale, GameObject bubblePrefab, Vector3 bubbleOffset)
    {
        Ticket = ticket;
        SeatIndex = seatIndex;
        CurrentState = State.NeedsOrderTaken;
        PatienceTimer = 0f;

        GameObject portraitObj = new GameObject("Portrait");
        portraitRenderer = portraitObj.AddComponent<SpriteRenderer>();
        portraitRenderer.sprite = portraitSprite;
        portraitRenderer.material = new Material(Shader.Find("Sprites/Default"));
        portraitObj.transform.SetParent(transform, false);
        portraitObj.transform.localPosition = portraitOffset;
        portraitObj.transform.localScale = Vector3.one * portraitScale;

        // click hitbox sized to match the portrait's actual rendered bounds
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.offset = portraitOffset;
        boxCollider.size = portraitSprite.bounds.size * portraitScale;

        if (bubblePrefab != null)
        {
            GameObject bubbleObj = Instantiate(bubblePrefab, transform);
            bubbleObj.transform.localPosition = bubbleOffset;
            bubble = bubbleObj.AddComponent<TicketBubble>();
            bubble.Init(this);
        }
    }

    private void Update()
    {
        if (CurrentState != State.Served)
        {
            PatienceTimer += Time.deltaTime;
        }
    }

    private void OnMouseDown()
    {
        LogTicket();
    }

    public void LogTicket()
    {
        if (CurrentState != State.NeedsOrderTaken)
            return;

        Ticket.loggedToTray = true;
        TicketTray.Instance.AddTicket(Ticket);
        SetState(State.WaitingForFood);
    }

    public void SetVisible(bool visible)
    {
        portraitRenderer.enabled = visible;
        boxCollider.enabled = visible;
        bubble?.SetVisible(visible);
    }

    public void SetState(State newState)
    {
        CurrentState = newState;
    }
}
