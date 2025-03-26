using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance;

    public GameObject[] blocks;

    private int blockId;
    private int nextBlockId;

    [SerializeField] private GameObject playerBoard;

    public readonly Vector3 spawnPosition = new Vector3(5f, 17f, 0);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        nextBlockId = Random.Range(0, 6);
    }

    // spawn new block, and position it properly
    public void SpawnBlock()
    {
        blockId = nextBlockId;
        nextBlockId = Random.Range(0, 6);

        EventManager.Instance.OnNextBlockChosen?.Invoke(nextBlockId);

        GameObject block = Instantiate(blocks[blockId], spawnPosition, Quaternion.identity);
        block.SetActive(false);

        GameObject blockModel = block.GetComponent<Block>().blockModel;
        Vector3 blockRotationPointVector = block.GetComponent<Block>().rotationPointVector;

        block.transform.position += blockRotationPointVector;
        blockModel.transform.localPosition -= blockRotationPointVector;

        block.name = "Block_" + Random.Range(1000, 9999).ToString();

        block.SetActive(true);
    }
}
