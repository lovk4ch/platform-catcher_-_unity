using UnityEngine;
using UnityEngine.EventSystems;

public class BlockController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    [Range(0, 50)]
    private float speed = 5f;

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    private Vector3 axisVector;
    public float direction { get; set; }

    private void Awake()
    {
        GetComponent<Renderer>().material.color = LevelManager.Instance.BlockColor;
        if (LevelManager.Instance.CurrentAxis == LevelManager.Axis.X)
            axisVector = Vector3.right;
        else
            axisVector = Vector3.forward;
    }

    private void Update()
    {
        transform.Translate(axisVector * speed * direction * Time.deltaTime);
        CheckBounds();
    }

    public void EnableGravity()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void GameOver()
    {
        EnableGravity();
        LevelManager.Instance.GameOver();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Speed != 0)
        {
            BlockController lastBlock = LevelManager.Instance.LastBlock;

            if (LevelManager.Instance.CurrentAxis == LevelManager.Axis.X)
            {
                float dx = LevelManager.Instance.LastBlock.transform.position.x
                    - transform.position.x;

                if (transform.localScale.x - Mathf.Abs(dx) > 0)
                {
                    SplitBlock(dx, transform.localScale.x);
                }
                else
                {
                    GameOver();
                }
            }
            else
            {
                float dz = lastBlock.transform.position.z - transform.position.z;
                if (transform.localScale.z - Mathf.Abs(dz) > 0)
                {
                    SplitBlock(dz, transform.localScale.z);
                }
                else
                {
                    GameOver();
                }
            }

            Speed = 0;
        }
    }

    private void SplitBlock(float delta, float blockScale)
    {
        transform.localScale -= axisVector * Mathf.Abs(delta);
        transform.position += axisVector * delta / 2;

        Vector3 splitPosition =     // позиция отрезанного блока
            transform.position      // позиция текущего блока
            - axisVector            // ось среза
            * Mathf.Abs(blockScale) // размер текущего блока
            * Mathf.Sign(delta)     // промах
            / 2;                    // половина слева или справа

        enabled = false;
        LevelManager.Instance.Split(this, splitPosition, Mathf.Abs(delta));
    }

    private void CheckBounds()
    {
        if (LevelManager.Instance.CurrentAxis == LevelManager.Axis.X)
        {
            if (transform.position.x > LevelManager.Instance.PlatformMovingInterval)
                direction = -1;
            else if (transform.position.x < -LevelManager.Instance.PlatformMovingInterval)
                direction = 1;

        }
        else
        {
            if (transform.position.z > LevelManager.Instance.PlatformMovingInterval)
                direction = -1;
            else if (transform.position.z < -LevelManager.Instance.PlatformMovingInterval)
                direction = 1;
        }

        if (transform.position.y < -100)
            Destroy(gameObject);
    }
}