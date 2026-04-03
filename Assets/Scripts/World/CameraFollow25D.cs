using UnityEngine;

public class CameraFollow25D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 14f, -10f);
    [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private float followSmooth = 12f;

    private Quaternion fixedRotation;

    private void Start()
    {
        if (target == null)
            return;

        // 根据 offset 和 lookOffset 计算一个固定朝向
        Vector3 desiredPosition = target.position + offset;
        Vector3 lookPoint = target.position + lookOffset;
        Vector3 lookDirection = (lookPoint - desiredPosition).normalized;

        fixedRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        // 开始时先对齐一次，避免第一帧位置和角度不对
        transform.position = desiredPosition;
        transform.rotation = fixedRotation;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            1f - Mathf.Exp(-followSmooth * Time.deltaTime)
        );

        transform.rotation = fixedRotation;
    }
}