using Global.Scripts;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float moveSpeed = 5f;       // ��ͨ�ƶ��ٶ�
    public float gravity = -9.81f;     // �����ֶΣ���ʱ�������������
    public float startHeight = 1.6f;   // �̶�����߶�
    public float collisionCheckDistance = 0.5f; // ���߼�����

    [SerializeField]
    private CharacterController controller;

    void Start()
    {
        // ��ʼ�߶ȵ���
        Vector3 pos = transform.position;
        pos.y = startHeight;
        transform.position = pos;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
        HandleMovement();
        MaintainHeight();
    }

    void HandleMovement()
    {
        // ��ȡ���뷽�����������ռ䣩
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveDir += transform.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveDir -= transform.forward;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveDir -= transform.right;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveDir += transform.right;

        moveDir.y = 0; // ���Դ�ֱ����
        moveDir.Normalize();

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Vector3 moveStep = moveDir * moveSpeed * Time.deltaTime;

            // ���߼���Ƿ����ϰ�
            if (!Physics.Raycast(transform.position, moveDir, out RaycastHit hit, collisionCheckDistance + moveStep.magnitude))
            {
                // û��ײ�������ƶ�
                transform.position += moveStep;
            }
            else
            {
                // ײ�����壬�����ϰ�ͣ��
                transform.position = hit.point - moveDir * 0.2f;
            }
        }
    }

    void MaintainHeight()
    {
        // �̶�����߶ȣ����ܵ���/��ײ��Ӱ��
        Vector3 pos = transform.position;
        pos.y = startHeight;
        transform.position = pos;
    }
}
