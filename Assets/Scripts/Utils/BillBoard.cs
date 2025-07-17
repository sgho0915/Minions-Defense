using UnityEngine;

[ExecuteAlways]    // 편집 모드에서도 회전이 갱신
public class BillBoard : MonoBehaviour
{
    Camera _cam;

    void Awake()
    {
        //씬에 MainCamera 태그 달린 카메라 찾기
        if (Camera.main != null) _cam = Camera.main;
    }

    void LateUpdate()
    {
        if (_cam == null) return;
        // 1) 월드-스페이스 오브젝트의 앞면(+Z축)을 카메라가 있는 방향으로 향하게
        Vector3 dir = (_cam.transform.position - transform.position).normalized;
        // 2) 쳐다보게 회전: forward 가 카메라 방향이 되도록
        transform.rotation = Quaternion.LookRotation(dir);

        // (만약, Y축 회전만 원하면:)
        // var lookPos = _cam.transform.position;
        // lookPos.y = transform.position.y;  
        // transform.LookAt(lookPos);
    }
}