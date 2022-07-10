using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PagedLetter : MonoBehaviour
{
    private int currentPage = 0;
    [SerializeField]
    public TMP_Text nextButtonText;

    public void OnDisable()
    {
        currentPage = 0;
    }

    public void OnEnable()
    {
        currentPage = 0;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        transform.GetChild(currentPage).gameObject.SetActive(true);
        ModifyNextButtonText();
    }

    public void NextPage()
    {
        currentPage = Mathf.Min(transform.childCount - 1, currentPage);

        if (currentPage == transform.childCount - 1)
        {
            ScreenObjectManager.Instance.HideCurrentActiveObject();
        }

        transform.GetChild(currentPage++).gameObject.SetActive(false);
        transform.GetChild(currentPage).gameObject.SetActive(true);

        ModifyNextButtonText();
    }

    public void PreviousPage()
    {
        if (currentPage <= 0)
        {
            currentPage = 0;
            return;
        }
        transform.GetChild(currentPage--).gameObject.SetActive(false);
        transform.GetChild(currentPage).gameObject.SetActive(true);

        ModifyNextButtonText();
    }

    private void ModifyNextButtonText()
    {
        if (currentPage == transform.childCount - 1)
        {
            nextButtonText.text = "Close";
        }
        else
        {
            nextButtonText.text = "Next Page";
        }
    }
}
