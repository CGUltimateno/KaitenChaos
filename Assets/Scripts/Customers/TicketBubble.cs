
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

    private void Awake()
    {
        bubbleBackground = transform.Find("Background").GetComponent<SpriteRenderer>();
        bubbleText = transform.Find("Text").gameObject;
        target = bubbleBackground.transform.localScale;
        loggedColor = new Color32(247, 254, 255, 87);
        unloggedColor = new Color32(247, 254, 255, 255);
        targetColor = unloggedColor;
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

        }
        else if (isExpanded)
        {
            target = new Vector3(8f, 8f, 1f);

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

        bubbleBackground.transform.localScale = Vector3.Lerp(bubbleBackground.transform.localScale, target, 10f * Time.deltaTime);
        bubbleBackground.color = targetColor;
    }

    public void AutoClose()
    {
        if (isLogged)
        {
            target = new Vector3(8f, 8f, 1f);
            bubbleText.SetActive(false);
            isExpanded = false;
            targetColor = loggedColor;
        }
    }

}