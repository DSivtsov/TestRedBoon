using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private Canvas _canvasInventory;

    public void Show()
    {
        _canvasInventory.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _canvasInventory.gameObject.SetActive(false);
    }
}
