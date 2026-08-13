
using System.Collections.Generic;
using UnityEngine;

//implemented by Cise Nil Köksal on 05.08.2026
//AI Assistance: ChatGPT(debugging support on three-state switching)

public class TicketBubble : MonoBehaviour
{
    public bool isLogged = false;
    private bool isExpanded = false;
    private SpriteRenderer bubbleBackground;
    private GameObject bubbleText;
    private Vector3 target;
    private Color32 unloggedColor;
    private Color32 loggedColor;
    private Color32 targetColor;
    private Customer owner;
    private TMPro.TextMeshPro textMesh;

    private void Awake()
    {
        bubbleBackground = transform.Find("Background").GetComponent<SpriteRenderer>();
        bubbleText = transform.Find("Text").gameObject;
        textMesh = bubbleText.GetComponent<TMPro.TextMeshPro>();

        bubbleBackground.transform.localPosition = Vector3.zero;
        RectTransform textRect = bubbleText.GetComponent<RectTransform>();
        if (textRect != null)
            textRect.anchoredPosition = Vector2.zero;
        bubbleBackground.transform.localScale = new Vector3(4f, 4f, 1f);
        target = bubbleBackground.transform.localScale;
        bubbleBackground.sortingOrder = 10; 

        Renderer textRenderer = bubbleText.GetComponent<Renderer>();
        if (textRenderer != null)
            textRenderer.sortingOrder = 11; 

        bubbleText.SetActive(false);

        loggedColor = new Color32(247, 254, 255, 87);
        unloggedColor = new Color32(247, 254, 255, 255);
        targetColor = unloggedColor;

        if (GetComponent<BoxCollider2D>() == null)
        {
            BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(5f, 5f);
        }
    }

    public void Init(Customer customer)
    {
        owner = customer;

        if (textMesh != null && owner != null && owner.Ticket != null)
            textMesh.text = owner.Ticket.phrase;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void OnMouseDown()
    {
        if (!isLogged)
        {
            isLogged = true;
            isExpanded = true;
            bubbleText.SetActive(true);
            target = new Vector3(24.51f, 10.47f, 1f);
            targetColor = unloggedColor;
            owner?.LogTicket();

        }
        else if (isExpanded)
        {
            target = new Vector3(4f, 4f, 1f);

            bubbleText.SetActive(false);
            isExpanded = false;
            targetColor = loggedColor;
        }
        else
        {
            target = new Vector3(24.51f, 10.47f, 1f);
            bubbleText.SetActive(true);
            isExpanded = true;
            targetColor = loggedColor;
        }
    }

    void Update()
    {
        bubbleBackground.transform.localScale = Vector3.Lerp(bubbleBackground.transform.localScale, target, 4f * Time.deltaTime);
        bubbleBackground.color = targetColor;

        if (isExpanded && Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld);
            bool clickedInsideBubble = hit != null && hit.transform.IsChildOf(transform);
            if (!clickedInsideBubble)
                AutoClose();
        }
    }

    public void AutoClose()
    {
        if (isLogged)
        {
            target = new Vector3(4f, 4f, 1f);
            bubbleText.SetActive(false);
            isExpanded = false;
            targetColor = loggedColor;
        }
    }

}