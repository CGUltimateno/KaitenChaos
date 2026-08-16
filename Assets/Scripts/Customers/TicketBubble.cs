
using System.Collections.Generic;
using UnityEngine;

//implemented by Cise Nil Köksal on 05.08.2026
//AI Assistance: ChatGPT(debugging support on three-state switching)

public class TicketBubble : MonoBehaviour
{
    private const float BubblePaddingX = 1.2f;
    private const float BubblePaddingY = 0.8f;
    private const float MaxBubbleWidth = 5.9925f;

    public bool isLogged = false;
    private bool isExpanded = false;
    private Vector3 expandedTargetScale;
    private Vector3 collapsedScale;
    private float expansion;       // 0 = collapsed, 1 = expanded — drives background scale + text fade/scale together
    private float targetExpansion;
    private SpriteRenderer bubbleBackground;
    private GameObject bubbleText;
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
        textMesh.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;

        bubbleBackground.transform.localPosition = Vector3.zero;
        RectTransform textRect = bubbleText.GetComponent<RectTransform>();
        if (textRect != null)
            textRect.anchoredPosition = Vector2.zero;
        bubbleBackground.transform.localScale = new Vector3(4f, 4f, 1f);
        collapsedScale = bubbleBackground.transform.localScale;
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
        {
            textMesh.text = owner.Ticket.phrase;
            expandedTargetScale = ComputeExpandedScale();
        }
    }

    // fits the wrap box + background to the actual phrase (capped at MaxBubbleWidth) instead of always filling a fixed-size box
    private Vector3 ComputeExpandedScale()
    {
        Vector2 preferred = textMesh.GetPreferredValues(textMesh.text, MaxBubbleWidth, 0f);
        float width = Mathf.Clamp(preferred.x, 2f, MaxBubbleWidth);
        float height = Mathf.Clamp(preferred.y, 1.2f, 8f);

        RectTransform textRect = textMesh.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(width, height);

        Vector2 nativeSize = bubbleBackground.sprite.bounds.size;
        float scaleX = (width + BubblePaddingX) / nativeSize.x;
        float scaleY = (height + BubblePaddingY) / nativeSize.y;
        return new Vector3(scaleX, scaleY, 1f);
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
            targetExpansion = 1f;
            targetColor = unloggedColor;
            owner?.LogTicket();
        }
        else if (isExpanded)
        {
            targetExpansion = 0f;
            isExpanded = false;
            targetColor = loggedColor;
        }
        else
        {
            targetExpansion = 1f;
            isExpanded = true;
            targetColor = loggedColor;
        }
    }

    void Update()
    {
        expansion = Mathf.Lerp(expansion, targetExpansion, 4f * Time.deltaTime);
        bubbleBackground.transform.localScale = Vector3.Lerp(collapsedScale, expandedTargetScale, expansion);
        bubbleBackground.color = targetColor;

        bool shouldShowText = expansion > 0.02f;
        if (bubbleText.activeSelf != shouldShowText)
            bubbleText.SetActive(shouldShowText);
        if (shouldShowText)
        {
            textMesh.alpha = expansion;
            bubbleText.transform.localScale = Vector3.one * expansion;
        }

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
            targetExpansion = 0f;
            isExpanded = false;
            targetColor = loggedColor;
        }
    }

}