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
    public float MaxPatience { get; private set; }

    private SpriteRenderer portraitRenderer;
    private BoxCollider2D boxCollider;
    private TicketBubble bubble;
    private Sprite neutralPortrait;
    private Sprite happyPortrait;
    private Sprite angryPortrait;

    public void Init(Ticket ticket, int seatIndex, Sprite neutralPortrait, Sprite happyPortrait, Sprite angryPortrait, float maxPatience, Vector3 portraitOffset, float portraitScale, GameObject bubblePrefab, Vector3 bubbleOffset)
    {
        Ticket = ticket;
        SeatIndex = seatIndex;
        CurrentState = State.NeedsOrderTaken;
        PatienceTimer = 0f;
        this.neutralPortrait = neutralPortrait;
        this.happyPortrait = happyPortrait;
        this.angryPortrait = angryPortrait;
        MaxPatience = maxPatience;

        GameObject portraitObj = new GameObject("Portrait");
        portraitRenderer = portraitObj.AddComponent<SpriteRenderer>();
        portraitRenderer.sprite = happyPortrait;
        portraitRenderer.material = new Material(Shader.Find("Sprites/Default"));
        portraitObj.transform.SetParent(transform, false);
        portraitObj.transform.localPosition = portraitOffset;
        portraitObj.transform.localScale = Vector3.one * portraitScale;

        // click hitbox sized to match the portrait's actual rendered bounds
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.offset = portraitOffset;
        boxCollider.size = happyPortrait.bounds.size * portraitScale;

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
        if (CurrentState == State.Served)
            return;

        PatienceTimer += Time.deltaTime;
        UpdateMoodPortrait();
    }

    private void UpdateMoodPortrait()
    {
        float fraction = MaxPatience > 0f ? PatienceTimer / MaxPatience : 1f;

        if (fraction >= 2f / 3f)
            portraitRenderer.sprite = angryPortrait;
        else if (fraction >= 1f / 3f)
            portraitRenderer.sprite = neutralPortrait;
        else
            portraitRenderer.sprite = happyPortrait;
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
