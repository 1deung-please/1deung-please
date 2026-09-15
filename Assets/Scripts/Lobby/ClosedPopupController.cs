using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedPopupController : MonoBehaviour
{
    public GameObject closedPopup;

    public void ShowPopup()
    {
        closedPopup.SetActive(true);
    }

    public void ClosePopup()
    {
        closedPopup.SetActive(false);
    }
}
