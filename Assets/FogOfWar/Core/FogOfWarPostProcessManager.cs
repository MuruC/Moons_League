using UnityEngine;

namespace FoW
{
    class FoWIDs
    {
        public int mainTex;
        public int skyboxTex;
        public int clearFogTex;
        public int fogColorTex;
        public int fogColorTexScale;
        public int inverseView;
        public int fogColor;
        public int mapOffset;
        public int mapSize;
        public int fogTextureSize;
        public int fogTex;
        public int outsideFogStrength;
        public int cameraWorldPosition;
        public int stereoSeparation;

        internal FoWIDs()
        {
            mainTex = Shader.PropertyToID("_MainTex");
            skyboxTex = Shader.PropertyToID("_SkyboxTex");
            clearFogTex = Shader.PropertyToID("_ClearFogTex");
            fogColorTex = Shader.PropertyToID("_FogColorTex");
            fogColorTexScale = Shader.PropertyToID("_FogColorTexScale");
            inverseView = Shader.PropertyToID("_InverseView");
            fogColor = Shader.PropertyToID("_FogColor");
            mapOffset = Shader.PropertyToID("_MapOffset");
            mapSize = Shader.PropertyToID("_MapSize");
            fogTextureSize = Shader.PropertyToID("_FogTextureSize");
            fogTex = Shader.PropertyToID("_FogTex");
            outsideFogStrength = Shader.PropertyToID("_OutsideFogStrength");
            cameraWorldPosition = Shader.PropertyToID("_CameraWorldPosition");
            stereoSeparation = Shader.PropertyToID("_StereoSeparation");
        }
    }

    public abstract class FogOfWarPostProcessManager
    {
        public int team { get; set; }
        public Camera camera { get; set; }
        public bool fogFarPlane { get; set; }
        public float outsideFogStrength { get; set; }
        public bool pointFiltering { get; set; }
        public Color fogColor { get; set; }
        public Texture fogColorTexture { get; set; }
        public bool fogTextureScreenSpace { get; set; }
        public float fogColorTextureScale { get; set; }
        public float fogColorTextureHeight { get; set; }
        public bool clearFog { get; set; }
        public int clearFogMask { get; set; }

        static FoWIDs _ids = null;
        static Camera _clearFogCamera = null;

        protected abstract void SetTexture(int id, Texture value);
        protected abstract void SetVector(int id, Vector4 value);
        protected abstract void SetColor(int id, Color value);
        protected abstract void SetFloat(int id, float value);
        protected abstract void SetMatrix(int id, Matrix4x4 value);
        protected abstract void SetKeyword(string keyword, bool enabled);
        protected abstract void GetTargetSize(out int width, out int height, out int depth);
        protected abstract void BlitToScreen();

        public void Render()
        {
            RenderTexture clearfogtexture = null;
            if (clearFog)
            {
                GetTargetSize(out int width, out int height, out int depth);
                clearfogtexture = RenderTexture.GetTemporary(width, height, depth);
                RenderClearFog(clearfogtexture);
                fogColorTexture = clearfogtexture;
                fogTextureScreenSpace = true;
            }

            RenderFogFull();

            if (clearfogtexture != null)
            {
                fogColorTexture = null;
                RenderTexture.ReleaseTemporary(clearfogtexture);
            }
        }

        void RenderFogFull()
        {
            FogOfWarTeam fow = FogOfWarTeam.GetTeam(team);
            if (fow == null)
            {
                Debug.LogWarning("No FogOfWar team found: " + team.ToString());
                return;
            }

            if (fow.finalFogTexture == null)
                return;

            if (_ids == null)
                _ids = new FoWIDs();

            if ((camera.depthTextureMode & DepthTextureMode.Depth) == 0)
                camera.depthTextureMode |= DepthTextureMode.Depth;

            fow.finalFogTexture.filterMode = pointFiltering ? FilterMode.Point : FilterMode.Bilinear;
            SetTexture(_ids.fogTex, fow.finalFogTexture);
            SetVector(_ids.fogTextureSize, fow.mapResolution.ToFloat());
            SetFloat(_ids.mapSize, fow.mapSize);
            SetVector(_ids.mapOffset, fow.mapOffset);
            SetColor(_ids.fogColor, fogColor);
            SetMatrix(_ids.inverseView, camera.cameraToWorldMatrix);
            SetFloat(_ids.outsideFogStrength, outsideFogStrength);
            SetVector(_ids.cameraWorldPosition, camera.transform.position);
            SetFloat(_ids.stereoSeparation, camera.stereoSeparation);

            // orthographic is treated very differently in the shader, so we have to make sure it executes the right code
            SetKeyword("CAMERA_PERSPECTIVE", !camera.orthographic);
            SetKeyword("CAMERA_ORTHOGRAPHIC", camera.orthographic);

            // which plane will the fog be rendered to?
            SetKeyword("PLANE_XY", fow.plane == FogOfWarPlane.XY);
            SetKeyword("PLANE_YZ", fow.plane == FogOfWarPlane.YZ);
            SetKeyword("PLANE_XZ", fow.plane == FogOfWarPlane.XZ);

            SetKeyword("FOG_COLORED", fogColorTexture == null);
            SetKeyword("FOG_TEXTURED_WORLD", fogColorTexture != null && !fogTextureScreenSpace);
            SetKeyword("FOG_TEXTURED_SCREEN", fogColorTexture != null && fogTextureScreenSpace);
            if (fogColorTexture != null)
            {
                SetTexture(_ids.fogColorTex, fogColorTexture);
                SetVector(_ids.fogColorTexScale, new Vector2(fogColorTextureScale, fogColorTextureHeight));
            }

            SetKeyword("FOGFARPLANE", fogFarPlane);

            BlitToScreen();
        }

        void RenderClearFog(RenderTexture clearfogtex)
        {
            // create clear fog camera
            if (_clearFogCamera == null)
            {
                _clearFogCamera = new GameObject("ClearFogCamera").AddComponent<Camera>();
                _clearFogCamera.enabled = false;
                _clearFogCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            // set clear fog camera values
            Transform camtfm = camera.transform;
            _clearFogCamera.transform.SetPositionAndRotation(camtfm.position, camtfm.rotation);
            _clearFogCamera.fieldOfView = camera.fieldOfView;
            _clearFogCamera.clearFlags = CameraClearFlags.Skybox;
            _clearFogCamera.targetTexture = clearfogtex;
            _clearFogCamera.cullingMask = clearFogMask;
            _clearFogCamera.orthographic = camera.orthographic;
            _clearFogCamera.orthographicSize = camera.orthographicSize;
            _clearFogCamera.rect = camera.rect;

            // render skyboxcamera to texture
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = _clearFogCamera.targetTexture;
            _clearFogCamera.Render();
            RenderTexture.active = currentRT;
            _clearFogCamera.targetTexture = null;
        }
    }
}
