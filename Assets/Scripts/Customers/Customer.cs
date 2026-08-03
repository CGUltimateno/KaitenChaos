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

    private SpriteRenderer spriteRenderer;

    public void Init(Ticket ticket, int seatIndex)
    {
        Ticket = ticket;
        SeatIndex = seatIndex;
        CurrentState = State.NeedsOrderTaken;
        PatienceTimer = 0f;

        // colored square placeholder
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = Resources.Load<Sprite>("unity_builtin_extra/Sprites/Square");
        if (spriteRenderer.sprite == null)
        {
            // fallback: create via Sprite.Create if built-in load fails
            Texture2D tex = Texture2D.whiteTexture;
            spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        spriteRenderer.color = Random.ColorHSV(0f, 1f, 0.5f, 0.8f, 0.7f, 1f);
        // size is temporary to test click collision, will replace with actual art later
        transform.localScale = Vector3.one * 50f; 
        
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();
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
        if (CurrentState != State.NeedsOrderTaken)
            return;

        Ticket.loggedToTray = true;
        TicketTray.Instance.AddTicket(Ticket);
        SetState(State.WaitingForFood);
    }

    public void SetState(State newState)
    {
        CurrentState = newState;
    }
}
