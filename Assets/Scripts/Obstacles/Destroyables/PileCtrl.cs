using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PileCtrl : MonoBehaviour
{
    private Queue<object> _pickupQueue = new Queue<object>();

    [SerializeField] public ResourceType rsc;
    [SerializeField] public int dropAmnt;

    [SerializeField] private Sprite _woodSprite;
    [SerializeField] private Sprite _metalSprite;

    [SerializeField] private SpriteRenderer sr;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(TryPickup());
        }
    }

    public void Setup(ResourceType rsc, int dropAmnt)
    {
        if (rsc == ResourceType.Wood)
        {
            sr.sprite = _woodSprite;
        } else if (rsc == ResourceType.Metal)
        {
            sr.sprite = _metalSprite;
        }
        this.dropAmnt = dropAmnt;
        this.rsc = rsc;
    }

    private IEnumerator TryPickup()
    {
        object ticket = new object();
        _pickupQueue.Enqueue(ticket);
        while (_pickupQueue.Peek() != ticket)
        {
            yield return null;
        }
        if (PlayerManager.Instance.Inventory_DeltaResourceCheck(rsc, dropAmnt, transform.position))
        {
            GameManager.Instance.pilePool.Release(this);
        }
        _pickupQueue.Dequeue();
    }
}
