using System.Collections.Generic;
using UnityEngine;

namespace FoW
{
    public enum FogOfWarPhysics
    {
        None,
        Physics2D,
        Physics3D
    }

    public enum FogOfWarPlane
    {
        XY, // 2D
        YZ,
        XZ // 3D
    }

    class FogOfWarDrawThreadTask : FogOfWarThreadTask
    {
        public FogOfWarShape shape;
        public FogOfWarDrawer drawer;

        public override void Run()
        {
            drawer.Draw(shape, true);
        }
    }

    [AddComponentMenu("FogOfWar/FogOfWarTeam")]
    public class FogOfWarTeam : MonoBehaviour
    {
        public int team = 0;

        [Header("Map")]
        public Vector2Int mapResolution = new Vector2Int(128, 128);
        public float mapSize = 128;
        public Vector2 mapOffset = Vector2.zero;

        public FogOfWarPlane plane = FogOfWarPlane.XZ;
        public FogOfWarPhysics physics = FogOfWarPhysics.Physics3D;

        [Header("Visuals")]
        public bool pointFiltering = false;
        public FilterMode filterMode { get { return pointFiltering ? FilterMode.Point : FilterMode.Bilinear; } }
        public int blurAmount = 0;
        public int blurIterations = 0;
        public FogOfWarBlurType blurType = FogOfWarBlurType.Gaussian3;

        [Header("Behaviour")]
        public bool updateUnits = true;
        public bool updateAutomatically = true;
        bool _isPerformingManualUpdate = false;
        [Range(0.0f, 1.0f)]
        public float partialFogAmount = 0.5f;
        float _fadeAmount = 0;
        public float fadeDuration = 0.1f;
        bool _hasFogChanged = false;

        [Header("Multithreading")]
        public bool multithreaded = false;
        [Range(2, 8)]
        public int threads = 2;
        public double maxMillisecondsPerFrame = 5;
        FogOfWarThreadPool _threadPool = null;
        int _currentUnitProcessing = 0;
        float _timeSinceLastUpdate = 0;
        System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        // core stuff
        public Texture2D fogTexture { get; private set; }
        public Texture finalFogTexture { get; private set; }
        byte[] _fogValuesCurrent = null; // how much the scene has been cleared for this frame only
        byte[] _fogValuesTotal = null; // how much the entire scene has been cleared (in the last finished fog frame)
        FogOfWarDrawerSoftware _drawer = null;
        int _drawThreadTaskPoolCount = 0;
        List<FogOfWarDrawThreadTask> _drawThreadTaskPool = new List<FogOfWarDrawThreadTask>();
        FogOfWarBlur _blur = new FogOfWarBlur();
        public UnityEngine.Events.UnityEvent onRenderFogTexture { get; private set; } = new UnityEngine.Events.UnityEvent(); // only call SetTotalFogValues() with multithreading when this is invoked!

        static List<FogOfWarTeam> _instances = new List<FogOfWarTeam>();
        public static List<FogOfWarTeam> instances { get { return _instances; } }

        public static FogOfWarTeam GetTeam(int team)
        {
            for (int i = 0; i < instances.Count; ++i)
            {
                if (instances[i].team == team)
                    return instances[i];
            }
            return null;
        }

        void Awake()
        {
            Reinitialize();
        }

        void OnEnable()
        {
            _instances.Add(this);
        }

        void OnDisable()
        {
            _instances.Remove(this);
        }

        // Call this whenever you change any of the size values of the map
        public void Reinitialize()
        {
            if (_drawer == null)
                _drawer = new FogOfWarDrawerSoftware();
            _drawer.Initialise(new FogOfWarMap(this));
            _drawer.Clear(255);

            if (_fogValuesCurrent == null || _fogValuesCurrent.Length != mapResolution.x * mapResolution.y)
            {
                _fogValuesCurrent = new byte[mapResolution.x * mapResolution.y];
                _fogValuesTotal = new byte[mapResolution.x * mapResolution.y];
            }

            for (int i = 0; i < _fogValuesCurrent.Length; ++i)
            {
                _fogValuesCurrent[i] = 255;
                _fogValuesTotal[i] = 255;
            }

            _drawThreadTaskPool.Clear();
        }

        public void GetCurrentFogValues(ref byte[] values)
        {
            if (values == null || values.Length != _fogValuesCurrent.Length)
                Debug.LogError("GetCurrentFogValues cannot take null as parameter or arrays of different sizes");
            else
                System.Array.Copy(_fogValuesCurrent, values, _fogValuesCurrent.Length);
        }

        public void SetCurrentFogValues(byte[] currentvalues)
        {
            if (currentvalues == null || currentvalues.Length != _fogValuesCurrent.Length)
            {
                Debug.LogError("SetCurrentFogValues cannot take null as parameter or arrays of different sizes");
                return;
            }

            System.Array.Copy(currentvalues, _fogValuesCurrent, _fogValuesCurrent.Length);
            _drawer.SetValues(_fogValuesCurrent);
        }

        public void GetTotalFogValues(ref byte[] totalvalues)
        {
            if (totalvalues == null || totalvalues.Length != _fogValuesTotal.Length)
                Debug.LogError("GetTotalFogValues cannot take null as parameter or arrays of different sizes");
            else
                System.Array.Copy(_fogValuesTotal, totalvalues, _fogValuesTotal.Length);
        }

        public void SetTotalFogValues(byte[] totalvalues)
        {
            if (totalvalues == null || totalvalues.Length != _fogValuesTotal.Length)
            {
                Debug.LogError("SetFogValues cannot take null as parameter or arrays of different sizes");
                return;
            }

            System.Array.Copy(totalvalues, _fogValuesTotal, _fogValuesTotal.Length);
        }

        // Increase skip to improve performance but sacrifice accuracy
        public float ExploredArea(int skip = 1)
        {
            skip = Mathf.Max(skip, 1);
            int total = 0;
            for (int i = 0; i < _fogValuesTotal.Length; i += skip)
                total += _fogValuesTotal[i];
            return (1.0f - total / (_fogValuesTotal.Length * 255.0f / skip)) * 2;
        }

        // Converts a world position to a fog pixel position. Values will be between 0 and mapResolution.
        public Vector2Int WorldPositionToFogPosition(Vector3 position)
        {
            Vector2 fogplanepos = FogOfWarConversion.WorldToFogPlane(position, plane);
            Vector2 mappos = FogOfWarConversion.WorldToFog(fogplanepos, mapOffset, mapResolution, mapSize);
            return mappos.ToInt();
        }

        // Returns the fog amount at a particular world position. 0 is fully unfogged and 255 if fully fogged.
        public byte GetFogValue(Vector3 position)
        {
            Vector2Int mappos = WorldPositionToFogPosition(position);
            mappos.x = Mathf.Clamp(mappos.x, 0, mapResolution.x - 1);
            mappos.y = Mathf.Clamp(mappos.y, 0, mapResolution.y - 1);
            return _fogValuesTotal[mappos.y * mapResolution.x + mappos.x];
        }

        // Set the fog for a square area of the map. 0 is fully unfogged and 255 if fully fogged.
        public void SetFog(Bounds bounds, byte value)
        {
            Rect rect = new Rect();
            rect.min = FogOfWarConversion.WorldToFog(bounds.min, plane, mapOffset, mapResolution, mapSize);
            rect.max = FogOfWarConversion.WorldToFog(bounds.max, plane, mapOffset, mapResolution, mapSize);

            int xmin = (int)Mathf.Max(rect.xMin, 0);
            int xmax = (int)Mathf.Min(rect.xMax, mapResolution.x);
            int ymin = (int)Mathf.Max(rect.yMin, 0);
            int ymax = (int)Mathf.Min(rect.yMax, mapResolution.y);

            for (int y = ymin; y < ymax; ++y)
            {
                for (int x = xmin; x < xmax; ++x)
                    _fogValuesTotal[y * mapResolution.x + x] = value;
            }
        }

        // Sets the fog value for the entire map. Set to 0 for completely unfogged, to 255 for completely fogged.
        public void SetAll(byte value = 255)
        {
            for (int i = 0; i < _fogValuesTotal.Length; ++i)
                _fogValuesTotal[i] = value;
        }

        // Checks the average visibility of an area. 0 is fully unfogged and 1 if fully fogged.
        public float VisibilityOfArea(Bounds worldbounds)
        {
            Vector2 min = FogOfWarConversion.WorldToFog(worldbounds.min, plane, mapOffset, mapResolution, mapSize);
            Vector2 max = FogOfWarConversion.WorldToFog(worldbounds.max, plane, mapOffset, mapResolution, mapSize);

            int xmin = Mathf.Clamp(Mathf.RoundToInt(min.x), 0, mapResolution.x);
            int xmax = Mathf.Clamp(Mathf.RoundToInt(max.x), 0, mapResolution.x);
            int ymin = Mathf.Clamp(Mathf.RoundToInt(min.y), 0, mapResolution.y);
            int ymax = Mathf.Clamp(Mathf.RoundToInt(max.y), 0, mapResolution.y);

            float total = 0;
            int count = 0;
            for (int y = ymin; y < ymax; ++y)
            {
                for (int x = xmin; x < xmax; ++x)
                {
                    ++count;
                    total += _fogValuesTotal[y * mapResolution.x + x] / 255.0f;
                }
            }

            return total / count;
        }

        void ProcessUnits(System.Diagnostics.Stopwatch stopwatch)
        {
            // if we are not updating units and all units have finished processing
            if (!updateUnits && _currentUnitProcessing >= FogOfWarUnit.registeredUnits.Count)
                return;

            // remove any invalid units
            FogOfWarUnit.registeredUnits.RemoveAll(u => u == null);

            double millisecondfrequency = 1000.0 / System.Diagnostics.Stopwatch.Frequency;
            for (; _currentUnitProcessing < FogOfWarUnit.registeredUnits.Count; ++_currentUnitProcessing)
            {
                if (!FogOfWarUnit.registeredUnits[_currentUnitProcessing].isActiveAndEnabled || FogOfWarUnit.registeredUnits[_currentUnitProcessing].team != team)
                    continue;

                FogOfWarShape shape = FogOfWarUnit.registeredUnits[_currentUnitProcessing].GetShape(this, physics, plane);
                if (multithreaded)
                {
                    ++_drawThreadTaskPoolCount;
                    while (_drawThreadTaskPoolCount > _drawThreadTaskPool.Count)
                        _drawThreadTaskPool.Add(new FogOfWarDrawThreadTask());

                    FogOfWarDrawThreadTask task = _drawThreadTaskPool[_drawThreadTaskPoolCount - 1];
                    task.drawer = _drawer;
                    task.shape = shape;
                    _threadPool.Run(task);
                }
                else
                    _drawer.Draw(shape, false);

                // do the timer check here so that at least one unit will be processed
                if (stopwatch != null && _stopwatch.ElapsedTicks * millisecondfrequency >= maxMillisecondsPerFrame)
                {
                    ++_currentUnitProcessing;
                    break;
                }
            }
        }

        [ContextMenu("Manual Update")]
        public void ManualUpdate()
        {
            ManualUpdate(1);
        }

        public void ManualUpdate(float timesincelastupdate)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Cannot do manual update when not playing!", this);
                return;
            }

            if (updateAutomatically && updateUnits)
            {
                Debug.LogWarning("Cannot do manual update when both updateAutomatically and updateMode are true!", this);
                return;
            }

            if (_isPerformingManualUpdate)
                return;

            _currentUnitProcessing = 0;
            _isPerformingManualUpdate = true; // flag for only one draw
            if (!updateUnits)
                _drawer.Clear(255);

            // multithreading will update on the next Update(), but single thread can do it now
            if (!multithreaded)
            {
                ProcessUnits(null);
                CompileFinalTexture(ref timesincelastupdate, false);
            }
        }

        void Update()
        {
            if (!updateAutomatically && !_isPerformingManualUpdate)
                return;

            // prepare threads
            if (multithreaded)
            {
                if (_threadPool == null)
                    _threadPool = new FogOfWarThreadPool();

                // do some thread maintenance
                threads = Mathf.Clamp(threads, 2, 8);
                _threadPool.maxThreads = threads;
                _threadPool.Clean();
            }
            else if (_threadPool != null)
            {
                _threadPool.StopAllThreads();
                _threadPool = null;
            }

            _stopwatch.Reset();
            _stopwatch.Start();

            // draw unit shapes
            ProcessUnits(_stopwatch);

            // compile final texture
            _timeSinceLastUpdate += Time.deltaTime;
            CompileFinalTexture(ref _timeSinceLastUpdate, true);

            _stopwatch.Stop();
        }

        void CompileFinalTexture(ref float timesincelastupdate, bool checkstopwatch)
        {
            // don't compile until all units have been processed
            if (_currentUnitProcessing < FogOfWarUnit.registeredUnits.Count || (checkstopwatch && multithreaded && !_threadPool.hasAllFinished))
                return;

            onRenderFogTexture.Invoke();

            // get the fog values from the drawer
            // get current values from units (if updateUnits is false, this will retain what it have since the last time updateUnits was true)
            _drawer.GetValues(_fogValuesCurrent);

            // fade in fog
            if (fadeDuration > 0.0001f)
                _fadeAmount += timesincelastupdate / fadeDuration;
            if (_fadeAmount > 1)
                _fadeAmount = 1;
            byte fadebytes = (byte)(_fadeAmount * 255);
            if (updateUnits || _hasFogChanged)
                _hasFogChanged = _drawer.Fade(_fogValuesCurrent, _fogValuesTotal, partialFogAmount, fadebytes);
            _fadeAmount -= fadebytes / 255.0f;

            if (updateUnits)
                _drawer.Clear(255);

            // prepare texture
            if (fogTexture == null)
            {
                fogTexture = new Texture2D(mapResolution.x, mapResolution.y, TextureFormat.Alpha8, false);
                fogTexture.wrapMode = TextureWrapMode.Clamp;
                fogTexture.filterMode = filterMode;
            }
            else if (fogTexture.width != mapResolution.x || fogTexture.height != mapResolution.y)
                fogTexture.Resize(mapResolution.x, mapResolution.y, TextureFormat.Alpha8, false);
            else
                fogTexture.filterMode = filterMode;
            fogTexture.LoadRawTextureData(_fogValuesTotal);
            fogTexture.Apply();

            // apply blur
            finalFogTexture = _blur.Apply(fogTexture, mapResolution, blurAmount, blurIterations, blurType);

            if (updateUnits)
                _currentUnitProcessing = 0;
            timesincelastupdate = 0;
            _drawThreadTaskPoolCount = 0;
            _isPerformingManualUpdate = false; // manual update has finished
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 offset = FogOfWarConversion.FogPlaneToWorld(mapOffset.x, mapOffset.y, 0, plane);
            Vector3 size = FogOfWarConversion.FogPlaneToWorld(mapSize, mapSize, 0, plane);
            Gizmos.DrawWireCube(offset, size);

            Gizmos.color = new Color(1, 0, 0, 0.2f);
            Gizmos.DrawCube(offset, size);
        }
    }
}
