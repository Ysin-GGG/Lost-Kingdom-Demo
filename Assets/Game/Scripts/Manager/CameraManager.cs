using UnityEngine;

namespace LostKingdom.Manager
{
    [DisallowMultipleComponent]
    public sealed class CameraManager : LKBehaviour
    {
        public static Camera MainCamera { get; private set; }
        public static Camera GridCamera { get; private set; }
        public static Vector2 GridCameraOffset { get; private set; }

        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private Camera gridCamera;

        protected override void Awake()
        {
            CameraManager.MainCamera = this.mainCamera;
            CameraManager.GridCamera = this.gridCamera;
            LK.OnScreenSizeChanged += this.UpdateSize;
        }

        private void UpdateSize()
        {
            float size = LK.ScreenHeight / 2f / GridManager.GridSize;
            CameraManager.GridCameraOffset = new Vector2(size * LK.ScreenWidth / LK.ScreenHeight, size);
            CameraManager.GridCamera.orthographicSize = size;
        }

        public void GridCameraPosition()
        {

        }
    }
}