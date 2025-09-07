using System;

namespace SDW
{
    public static class GameManagerEvents
    {
        /// <summary>
        /// Addressables 다운로드/업데이트가 모두 끝났을 때 1회 발행
        /// </summary>
        public static event Action OnDownloadCompleted;

        /// <summary>
        /// 씬 인덱스가 빌드되었을 때(=ScenePathIndex.Rebuild 후) 발행
        /// </summary>
        public static event Action OnSceneIndexed;

        /// <summary>
        /// 다운로드가 완료되었음을 알리는 이벤트를 트리거하는 메서드
        /// </summary>
        internal static void RaiseDownloadCompleted() => OnDownloadCompleted?.Invoke();

        /// <summary>
        /// 씬이 완전히 인덱싱되고 로드되었을 때 호출되는 이벤트를 트리거하는 메서드
        /// </summary>
        internal static void RaiseSceneIndexed() => OnSceneIndexed?.Invoke();
    }
}