using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Manager<LevelManager>
{
    #region Variables

    private Vector3 blocksTopPosition = Vector3.zero;

    [SerializeField]
    private Color startColor = Color.red;
    [SerializeField]
    private Color midColor = Color.green;
    [SerializeField]
    private Color endColor = Color.blue;
    [SerializeField]
    [Range(15, 150)]
    private int gradientCycleCount = 30;
    [SerializeField]
    [Range(10, 30)]
    private int platformMovingInterval = 11;

    [SerializeField]
    private BlockController blockPrefab = null;

    public enum Axis { X, Z };
    public Axis CurrentAxis;

    public int PlatformMovingInterval => platformMovingInterval;

    public BlockController LastBlock
    {
        get
        {
            if (Blocks.Count == 0)
            {
                BlockController block = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
                block.enabled = false;
                block.Speed = 0;
                Blocks.Add(block);

                return block;
            }
            else
            {
                return Blocks[Blocks.Count - 1];
            }
        }
    }

    public Color BlockColor
    {
        get
        {
            float number = Blocks.Count % gradientCycleCount;
            float colorStep = (gradientCycleCount / 3f);
            float lerp = number % colorStep / colorStep;

            if (number < colorStep)
            {
                return Color.Lerp(startColor, midColor, lerp);
            }
            else if (number < colorStep * 2)
            {
                return Color.Lerp(midColor, endColor, lerp);
            }
            else
            {
                return Color.Lerp(endColor, startColor, lerp);
            }
        }
    }
    public List<BlockController> Blocks { get; set; }

    #endregion





    public delegate void Event();
    public event Event OnChange;

    public void AddListener(Event e)
    {
        OnChange += e;
    }

    private void Awake()
    {
        Gimbal.Instance.LookPosition = blocksTopPosition;
        Blocks = new List<BlockController>();
    }

    public void Restart()
    {
        BlockController[] blocks = FindObjectsOfType<BlockController>();
        for (int i = 0; i < blocks.Length; i++)
        {
            Destroy(blocks[i].gameObject);
        }
        Blocks.Clear();

        blocksTopPosition = Vector3.zero;
        Gimbal.Instance.LookReset();

        GetBlock();
    }

    public void GameOver()
    {
        UIManager.Instance.RestartEnabled = true;
        Gimbal.Instance.LookToTower();
    }

    private void GetBlock()
    {
        blocksTopPosition += Vector3.up * blockPrefab.transform.localScale.y;

        float direction = Random.Range(1, 3);
        if (direction != 1)
            direction = -1;

        float axisRandom = Random.Range(1, 3);

        Vector3 shift;
        if (axisRandom == 1)
        {
            CurrentAxis = Axis.X;
            shift = Vector3.right * direction * PlatformMovingInterval;
        }
        else
        {
            CurrentAxis = Axis.Z;
            shift = Vector3.forward * direction * PlatformMovingInterval;
        }

        Vector3 pos = blocksTopPosition + shift + new Vector3(
            LastBlock.transform.position.x,
            0,
            LastBlock.transform.position.z
        );

        Vector3 scale = LastBlock.transform.localScale;

        BlockController block = Instantiate(blockPrefab, pos, Quaternion.identity);
        block.transform.localScale = scale;
        block.direction = -direction;
        Gimbal.Instance.LookPosition = blocksTopPosition;
    }

    public void Split(BlockController block, Vector3 position, float scale)
    {
        BlockController splitBlock = Instantiate(blockPrefab, position, Quaternion.identity);
        splitBlock.EnableGravity();
        splitBlock.Speed = 0;
        Vector3 blockScale = block.transform.localScale;

        if (CurrentAxis == Axis.X)
            splitBlock.transform.localScale = new Vector3(scale, blockScale.y, blockScale.z);
        else
            splitBlock.transform.localScale = new Vector3(blockScale.x, blockScale.y, scale);

        AudioManager.Instance.Play("Set");
        Blocks.Add(block);

        GetBlock();
        OnChange.Invoke();
    }
}