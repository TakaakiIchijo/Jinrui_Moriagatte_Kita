using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class TreeBase : MonoBehaviour {

    public List<ItemBase> itemBaseList = new List<ItemBase>();

    public BoxCollider BoxCollider;

    public bool isTouchable = false;

    public void SetPlayerItemTakeCallback(UnityAction callback)
    {
        itemBaseList.ForEach(item => item.SetTakeItemCallback(callback));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTouchable = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTouchable = false;
        }
    }

    private void Update()
    {
        if (isTouchable && Input.GetButtonDown("Fire1"))
        {
            OnDropItems();
        }
    }

    public void OnDropItems()
    {
        itemBaseList.ForEach(item => item.ChangeStateCanTake());
    }
}
