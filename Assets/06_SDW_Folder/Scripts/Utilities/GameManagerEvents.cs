using System;

namespace SDW
{
    public static class GameManagerEvents
    {
        /// <summary>Addressables 다운로드/업데이트가 모두 끝났을 때 1회 발행</summary>
        public static event Action OnDownloadCompleted;

        /// <summary>씬 인덱스가 빌드되었을 때(=ScenePathIndex.Rebuild 후) 발행</summary>
        public static event Action OnSceneIndexed;

        internal static void RaiseDownloadCompleted() => OnDownloadCompleted?.Invoke();
        internal static void RaiseSceneIndexed() => OnSceneIndexed?.Invoke();
    }
}