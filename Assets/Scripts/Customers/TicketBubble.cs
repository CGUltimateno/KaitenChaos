
using System.Collections.Generic;
using UnityEngine;

//implemented by Cise Nil Köksal on 05.08.2026

public class TicketBubble : MonoBehaviour
{
    public bool isLogged = false;
    private Transform bubbleBackground;
    private GameObject bubbleText;
    private Vector3 target;

    private void Awake()
    {
        bubbleBackground = transform.Find("Background");
        bubbleText = transform.Find("Text").gameObject;
        target = bubbleBackground.localScale;
    }

    public void OnMouseDown()
    {
        isLogged = !isLogged;
        if (!isLogged)
        {
            bubbleText.SetActive(false);
            target = new Vector3(8f, 8f, 1f);

        }
        else
        {
            target = new Vector3(24.51f, 10.47f, 1f);
            bubbleText.SetActive(true);
        }
    }

    void Update()
    {
        bubbleBackground.localScale = Vector3.Lerp(bubbleBackground.localScale, target, 10f * Time.deltaTime);
    }

}