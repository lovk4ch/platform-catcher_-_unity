using UnityEngine;

public class Gimbal : Manager<Gimbal>
{
    private float minSmoothScale = -1.25f;
    private float maxSmoothScale = -3;
    private float correctSmoothScale = 0;

    private static Vector3 smoothScale;

    public Vector3 LookPosition { get; set; }

    [SerializeField]
    private float m_speed = 10f;
    [SerializeField]
    private Transform cameraHolder = null;

    private void Awake()
    {
        correctSmoothScale = (Screen.height / (float)Screen.width - 1920f / 1080f) * 0.3f;
        minSmoothScale -= correctSmoothScale;
        maxSmoothScale -= correctSmoothScale;
        smoothScale = cameraHolder.localPosition - Vector3.forward * correctSmoothScale;

        cameraHolder.localPosition = smoothScale;
    }

    public void LookToTower()
    {
        smoothScale = new Vector3(smoothScale.x, smoothScale.y, maxSmoothScale);
    }

    public void LookReset()
    {
        smoothScale = new Vector3(smoothScale.x, smoothScale.y, minSmoothScale);
    }

    private void LateUpdate()
    {
        float delta = Time.deltaTime;

        float change = InputManager.Instance.GetWheel();
        if (change != 0 && smoothScale.z + change <= -5 && smoothScale.z + change >= -60)
        {
            smoothScale += new Vector3(0, 0, change);
        }
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, smoothScale, m_speed * delta);

        transform.position = Vector3.Lerp(transform.position, LookPosition, m_speed * delta);
    }
}