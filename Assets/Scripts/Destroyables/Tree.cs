using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Destroyable
{
    public override float maxHealth => 50f;

    [SerializeField] GameObject _woodDropPrefab; // might be temp

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log($"I'm a tree, I'm now at {currentHealth} health!!!");
        if (currentHealth == 0)
        {
            Debug.Log("Oh no! I'm dead!! :(");
        }

        Vector3 hitDirection = (transform.position - new Vector3(PlayerManager.Instance.PlayerWorldPosition.x, PlayerManager.Instance.PlayerWorldPosition.y, transform.position.z)).normalized;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + hitDirection * 0.05f;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(endPos, 0.1f));
        sequence.Append(transform.DOMove(startPos, 0.1f));

        sequence.Play();
    }

    public override void DestroyObject()
    {
        base.DestroyObject();
        GameObject woodDrop = Instantiate(_woodDropPrefab);
        woodDrop.transform.position = transform.position;
    }
}
