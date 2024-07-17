using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    //����ũ ȿ�� �� ī�޶�
    public Transform shakeCamera;

    public bool shakeRotate = false;

    Vector3 originPos;
    Quaternion originRot;

    void Start()
    {
        //�������� ���� ��ġ���� ȸ���� �����صα�
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
    }

    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f, float manitudeRot = 0.1f)
    {
        float passTime = 0f;

        //duration Ÿ�� ���� ���� ���ؼ� While ���
        while (passTime < duration)
        {
            //�������� 1�� ������ ���� �ȿ��� ������ 3���� ��ǥ(x,y,z) ����
            Vector3 shakePos = Random.insideUnitSphere;
            //������ ���� ������ġ�� �Ű��������ؼ� ����
            shakeCamera.localPosition = shakePos * magnitudePos;

            //�ұ�Ģ�� ȸ�� ��� ����
            if (shakeRotate)
            {
                //������ �޸������� �Լ��μ�
                //� �ұ�Ģ�� ������ ���������� ��
                //���� �� ����� ����
                float z = Mathf.PerlinNoise(Time.time * manitudeRot, 0f);
                Vector3 shakeRot = new Vector3(0, 0, z);

                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }
            passTime += Time.deltaTime;
            yield return null;
        }

        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;
    }
 
}
