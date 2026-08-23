using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Implemented by Mohamed Osman on 2026-08-07
//Extended by Cise Nil Köksal on 21/08/2026
// AI assistance: Claude (autocomplete, architecture review)
//ChatGPT: Debugging

public class Guidebook : MonoBehaviour
{
    public static Guidebook Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject panel2;
    [SerializeField] private GameObject arrowLeft;
    [SerializeField] private GameObject arrowRight;


    [SerializeField] private GameObject clickCatcher;
    [SerializeField] private GameObject[] pages;
    [SerializeField] private GuidebookPage[] subpages;
    [SerializeField] private AudioSource turnSound;

    private static readonly string[] Traits = { "classic", "sweet", "spicy", "healthy", "rich" };
    private bool isOpen;
    private int currentPage;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        panel.SetActive(false);
        clickCatcher.SetActive(false);

    }

    public void Open()
    {
        isOpen = true;
        panel.SetActive(true);
        clickCatcher.SetActive(true);
        currentPage = 0;
        ShowPage(0);
    }

    public void Close()
    {
        isOpen = false;
        panel.SetActive(false);
        clickCatcher.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (isOpen) Close(); else Open();
        }

    }

    public void PreviousPage()
    {
        currentPage = currentPage - 1;
        ShowPage(currentPage);
    }
    public void NextPage()
    {
        currentPage = currentPage + 1;
        ShowPage(currentPage);
    }
    private void ShowPage(int index)
    {
        turnSound.Play();
        arrowLeft.SetActive(index > 0);
        arrowRight.SetActive(index < pages.Length - 1);
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }
        pages[index].SetActive(true);
    }

    public void UnlockTrait(string ingredientId, string trait, int score)
    {
        Debug.Log(
        $"[Guidebook] UnlockTrait called: {ingredientId} / {trait} / {score}"
    );

        foreach (GuidebookPage page in subpages)
        {
            if (page.ingredientID == ingredientId)
            {
                page.UnlockTrait(trait, score);
                return;
            }
        }
    }
}
