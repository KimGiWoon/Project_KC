namespace SDW
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MySceneManager : MonoBehaviour
    {
        private AsyncOperation _levelSceneOperation;
        private ISceneLoadable _sceneLoadUI;

        private bool _isLoading = false;
        public bool IsLoading => _isLoading;
        private SceneName _prevSceneName = SceneName.SDW_SignInScene;
        private int _prevSceneIndex = 0;

        /// <summary>
        /// Scene Loading UI 요소에 대한 컴포넌트 연결
        /// </summary>
        private void Start()
        {
            _sceneLoadUI = GameManager.Instance.UI.GetComponent<ISceneLoadable>();
        }

        /// <summary>
        /// 비동기로 지정된 Scene을 로드하는 메서드
        /// </summary>
        /// <param name="sceneName">로드할 Scene의 이름을 나타내는 <see cref="SceneName"/> 열거형 값</param>
        public void LoadSceneAsync(SceneName sceneName)
        {
            if (!_isLoading)
            {
                _prevSceneName = sceneName;
                StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
            }
        }

        /// <summary>
        /// 지정된 Scene을 비동기로 로드하기 위한 메서드 호출
        /// </summary>
        /// <param name="sceneIndex">로드할 Scene의 index 값</param>
        public void LoadSceneAsync(int sceneIndex)
        {
            if (!_isLoading)
            {
                _prevSceneIndex = sceneIndex;
                StartCoroutine(LoadSceneAsyncCoroutine(sceneIndex));
            }
        }

        /// <summary>
        /// 비동기로 Scene 로드 작업을 수행하는 코루틴 메서드
        /// </summary>
        /// <param name="sceneName">로드할 Scene의 이름을 나타내는 <see cref="SceneName"/> 열거형 값</param>
        private IEnumerator LoadSceneAsyncCoroutine(SceneName sceneName)
        {
            yield return StartCoroutine(PrepareSceneLoading());

            LoadScene(sceneName);

            yield return StartCoroutine(UpdateLoadingProgress());
        }

        /// <summary>
        /// 지정된 Scene을 비동기로 로드하는 코루틴 메서드
        /// </summary>
        /// <param name="sceneIndex">로드할 Scene의 index 값</param>
        private IEnumerator LoadSceneAsyncCoroutine(int sceneIndex)
        {
            yield return StartCoroutine(PrepareSceneLoading());

            LoadScene(sceneIndex);

            yield return StartCoroutine(UpdateLoadingProgress());
        }

        /// <summary>
        /// 지정된 이름 또는 인덱스의 Scene을 비동기로 로드하는 메서드 호출
        /// </summary>
        /// <param name="sceneName">로드할 Scene의 이름을 나타내는 <see cref="SceneName"/> 열거형 값</param>
        private void LoadScene(SceneName sceneName)
        {
            _levelSceneOperation = SceneManager.LoadSceneAsync(sceneName.ToString());
            _levelSceneOperation.allowSceneActivation = false;
        }

        /// <summary>
        /// 지정된 이름 또는 인덱스의 Scene을 비동기로 로드하는 메서드 호출
        /// </summary>
        /// <param name="sceneIndex">로드할 Scene의 index 값</param>
        private void LoadScene(int sceneIndex)
        {
            _levelSceneOperation = SceneManager.LoadSceneAsync(sceneIndex);
            _levelSceneOperation.allowSceneActivation = false;
        }

        /// <summary>
        /// Scene 로딩 준비 코루틴을 시작하여 UI 요소를 초기화하고 로딩 상태를 표시
        /// </summary>
        private IEnumerator PrepareSceneLoading()
        {
            _isLoading = true;
            _sceneLoadUI.InitSceneLoadingUI();

            yield return new WaitForEndOfFrame();
        }

        /// <summary>
        /// 지정된 Scene 로드 작업의 진행 상황을 업데이트하는 비동기 코루틴 메서드
        /// </summary>
        /// <returns>비동기 코루틴을 나타냄</returns>
        private IEnumerator UpdateLoadingProgress()
        {
            while (!_levelSceneOperation.isDone)
            {
                //# 씬 활성화 및 씬이 완전히 로드될 때까지 대기
                if (_levelSceneOperation.progress >= 0.88f)
                {
                    _levelSceneOperation.allowSceneActivation = true;
                    yield return new WaitUntil(() => _levelSceneOperation.isDone);
                }

                //# 로딩 진행률 (0.0 ~ 0.9)
                float totalProgress = _levelSceneOperation.progress;

                if (UpdateLoadingUI(totalProgress)) break;

                yield return null;
            }

            yield return StartCoroutine(CompleteSceneLoading());
        }

        /// <summary>
        /// 로드 중인 Scene UI 업데이트를 처리하는 메서드
        /// </summary>
        /// <param name="totalProgress">Scene 로드 진행률 (0.0f부터 1.0f 사이의 값)</param>
        /// <returns>UI 업데이트가 완료되면 <c>true</c>, 그렇지 않으면 <c>false</c></returns>
        private bool UpdateLoadingUI(float totalProgress)
        {
            //# 0.9에서 1.0까지는 수동으로 제어
            float displayProgress = totalProgress;

            if (displayProgress >= 0.9f)
            {
                displayProgress = Mathf.Lerp(displayProgress, 1.0f, Time.deltaTime);

                if (displayProgress >= 1f)
                {
                    _sceneLoadUI.UpdateLoadingUI(1.0f);
                    return true;
                }
            }

            _sceneLoadUI.UpdateLoadingUI(displayProgress);
            return false;
        }

        /// <summary>
        /// 지정된 비동기 로딩 프로세스를 완료하고 관련 로딩 UI 요소를 처리하는 메서드
        /// </summary>
        private IEnumerator CompleteSceneLoading()
        {
            _sceneLoadUI.CompleteSceneLoading();

            yield return new WaitForSeconds(1f);

            _isLoading = false;
            _levelSceneOperation = null;
        }

        /// <summary>
        /// 현재 활성화된 Scene의 이름을 반환
        /// </summary>
        /// <returns>현재 활성화된 Scene의 이름 문자열</returns>
        public string GetActiveScene() => SceneManager.GetActiveScene().name;
    }
}