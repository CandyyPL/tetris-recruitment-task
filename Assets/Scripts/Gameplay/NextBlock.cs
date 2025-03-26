using UnityEngine;

public class NextBlock : MonoBehaviour
{
    [SerializeField] private GameObject[] blocks;

    private GameObject lastBlock;

    private void Start()
    {
        EventManager.Instance.OnNextBlockChosen += OnNextBlockChosen;
        BlockSpawner.Instance.SpawnBlock();
    }

    // change display of next block on event invoke
    private void OnNextBlockChosen(int nextBlockId)
    {
        Destroy(lastBlock);
        GameObject block = Instantiate(blocks[nextBlockId], transform.position, Quaternion.identity);
        block.name = "NextBlockModel";
        block.transform.SetParent(transform);

        lastBlock = block;
    }
}
