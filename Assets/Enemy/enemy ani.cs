using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyani : MonoBehaviour
{
    public Transform player; // ���a����
    private Vector3 originalScale;

    private void Start()
    {
        // �O�s�ĤH����l�Y���
        originalScale = transform.localScale;
        // �۰ʴM��a�� "Player" ���Ҫ�����
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // �p�G���a�b�ĤH�k��A�O����l��V�F�b����A½�� X �b
            if (player != null)
            {
                // �p�G���a�b�k���A½�� X �b�F�b�����A�O����l��V
                if (player.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
                }
            }
        }
    }
}