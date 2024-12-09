using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotateSpeed;
    


    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // �L�[���͂��擾
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        // �ړ��������v�Z
        var movement = transform.forward * vertical; // ���ʂɑ΂��đO��̓���

        // ��]�������v�Z
        var rotation = Vector3.up * horizontal; // ������ɑ΂��č��E�̓���

        // �X�s�[�h�ƃt���[���̌o�ߎ��Ԃ�������
        movement *= moveSpeed * Time.deltaTime;
        rotation *= rotateSpeed * Time.deltaTime;

        // �ړ��Ɖ�]
        _rb.position += movement;
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(rotation));

        // �X�y�[�X�L�[����������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;
            // �����̈ʒu���牺������2��Ray���΂�
            if (Physics.Raycast(transform.position, -transform.up, out hit, 2))
            {
                // Ray�����������I�u�W�F�N�g���擾
                var hitObject = hit.collider.gameObject;
                // �J�[�h�R���g���[���[���A�^�b�`����Ă�����
                if (hitObject.TryGetComponent<CardController>(out var cardController))
                {
                    // �J�[�h����]������
                    cardController.FlipCard();
                }
            }
        }
    }

    public void Impact(Vector3 direction)
    {
        _rb.AddForce(direction * 10f, ForceMode.Impulse);
        Vector3 torqueAxis = Vector3.Cross(direction, Vector3.up);
        _rb.AddTorque(torqueAxis * 10f, ForceMode.Impulse);
    }
}
