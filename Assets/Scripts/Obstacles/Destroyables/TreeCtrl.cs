using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TreeCtrl : Destroyable
{
    public override float maxHealth => 50f;
    public int woodAmnt = 4;

    [SerializeField] WoodPileCtrl _woodDropPrefab; // might be temp

    public static ObjectPool<WoodPileCtrl> woodDropPool = null;

    protected override void Awake()
    {
        base.Awake();
        if (woodDropPool == null)
        {
            woodDropPool = new ObjectPool<WoodPileCtrl>(() =>
            {
                return Instantiate(_woodDropPrefab);
            },
            woodDrop =>
            {
                woodDrop.gameObject.SetActive(true);
            },
            woodDrop =>
            {
                woodDrop.gameObject.SetActive(false);
            },
            null, true, 10);
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log($"I'm a tree, I'm now at {currentHealth} health!!!");
        if (currentHealth == 0)
        {
            Debug.Log("Oh no! I'm dead!! :(");
            GiveWood(); // Give wood to player
            ObstacleManager.Instance.RemoveObstacleAt(pos); // Destroy self
            return;
        }

        Vector3 hitDirection = (transform.position - new Vector3(PlayerManager.Instance.PlayerWorldPosition.x, PlayerManager.Instance.PlayerWorldPosition.y, transform.position.z)).normalized;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + hitDirection * 0.05f;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(endPos, 0.1f));
        sequence.Append(transform.DOMove(startPos, 0.1f));

        sequence.Play();
    }

    private void GiveWood()
    {
        if (!PlayerManager.Instance.Inventory_DeltaResourceCheck(ResourceType.Wood, woodAmnt))
        {
            int amntCanAdd = PlayerManager.Instance.carryCapacity - PlayerManager.Instance.Inventory_GetTotalCarried();
            int remaining = woodAmnt - amntCanAdd;
            if (!PlayerManager.Instance.Inventory_DeltaResourceCheck(ResourceType.Wood, amntCanAdd))
            {
                throw new System.Exception("Tree wood give mechanism fail");
            }
            for (int i = 0; i < remaining; ++i)
            {
                DropWood();
            }
        }
    }

    private void DropWood()
    {
        WoodPileCtrl woodDrop = woodDropPool.Get();
        Vector2 gridWorldPos = GridManager.Instance.GridToWorldPos(pos);
        float gridSize = GridManager.Instance.gridConfig.gridSquareSize;
        woodDrop.transform.position = new Vector3(UnityEngine.Random.Range(gridWorldPos.x, gridWorldPos.x + gridSize), UnityEngine.Random.Range(gridWorldPos.y, gridWorldPos.y + gridSize), 0);
        woodDrop.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-35f, 35f));
    }
}
