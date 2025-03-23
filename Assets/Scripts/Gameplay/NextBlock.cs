using UnityEngine;

public class NextBlock : MonoBehaviour
{
    private GameObject[] blocks;

    private GameObject lastBlock;

    private void Start()
    {
        blocks = BlockSpawner.Instance.blocks;

        EventManager.Instance.OnNextBlockChosen += OnNextBlockChosen;
    }

    private void OnNextBlockChosen(int nextBlockId)
    {
        Destroy(lastBlock);
        GameObject block = Instantiate(blocks[nextBlockId], transform.position, Quaternion.identity);
        block.name = "NextBlockModel";
        block.transform.SetParent(transform);

        Block blockComponent = block.GetComponent<Block>();
        blockComponent.blockModel.transform.localPosition += Vector3.left;

        block.GetComponent<Block>().enabled = false;

        lastBlock = block;
    }
}
