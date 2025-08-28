using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 싱글톤 매니저  
public class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;
    static bool _isShutDown = false;

    [Header("Singleton Settings")]
    [SerializeField] bool _dontDestroyOnLoad = true;

    public static T Instance
    {
        get
        {
            // 앱이 종료 중이면 null 반환
            if (_isShutDown) return null; 

            if (_instance == null)
            {
                // 최초 1회 씬에 이미 존재하고 있는 인스턴스를 찾아서 대입
                _instance = FindFirstObjectByType<T>();

                // 찾았는데 없으면 새로 생성
                if (_instance == null)
                {
                    // 빈 오브젝트를 만들고 싱글톤 매니저 컴포넌트를 붙임
                    var singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();

                    // 생성된 오브젝트가 싱글톤매니저 타입인지 확인 후 맞으면 singletonComponent변수에 저장하고 오브젝트가 씬 이동시 파괴가 되지 않길 원하면
                    if (_instance is SingletonManager<T> singletonComponent && singletonComponent._dontDestroyOnLoad)
                    {
                        DontDestroyOnLoad(singletonComponent);
                    }
                }
            }
            return _instance;
        }
    }

    // 인스턴스 초기화
    protected virtual void Awake()
    {
        // 오브젝트가 존재하지 않으면
        if (_instance == null)
        {
            // 자기 자신을 저장
            _instance = this as T;

            // 오브젝트 미파괴 적용
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (_instance != null) // 오브젝트가 있으면 중복 관련 삭제
        {
            Destroy(gameObject);
        }
    }

    // 게임 종료 시 싱글톤 재생성을 방지하기 위해 종료 중으로 전환
    protected virtual void OnApplicationQuit()
    {
        _isShutDown = true;
    }

    // 파괴 시 싱글톤 재생성을 방지하기 위해 종료 중으로 전환
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _isShutDown = true;
        }
    }
}
