using UnityEngine;
using Unity.Profiling;

public class RuntimeMetrics : MonoBehaviour
{
    ProfilerRecorder mainThreadTime;
    ProfilerRecorder gpuTime;
    ProfilerRecorder gcAlloc;

    float fps;
    float smoothedFps;

    GUIStyle textStyle;

    void OnEnable()
    {
        mainThreadTime = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        gpuTime = ProfilerRecorder.StartNew(ProfilerCategory.Render, "GPU Frame Time", 15);
        gcAlloc = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", 15);

        // Crear estilo sin usar GUI.skin (seguro fuera de OnGUI)
        CreateBaseStyle();
    }

    void OnDisable()
    {
        mainThreadTime.Dispose();
        gpuTime.Dispose();
        gcAlloc.Dispose();
    }

    void Update()
    {
        fps = 1f / Time.deltaTime;
        smoothedFps = Mathf.Lerp(smoothedFps, fps, 0.1f); // suavizado
    }

    void CreateBaseStyle()
    {
        textStyle = new GUIStyle();
        textStyle.fontSize = 16;
        textStyle.normal.textColor = Color.green;
    }

    void OnGUI()
    {
        // Asegurar que exista
        if (textStyle == null)
            CreateBaseStyle();

        GUI.Label(new Rect(10, 10, 300, 20), $"FPS: {smoothedFps:F1}", textStyle);
        GUI.Label(new Rect(10, 30, 300, 20), $"CPU Frame Time: {GetRecorderMs(mainThreadTime)} ms", textStyle);
        GUI.Label(new Rect(10, 50, 300, 20), $"GPU Frame Time: {GetRecorderMs(gpuTime)} ms", textStyle);
        GUI.Label(new Rect(10, 70, 300, 20), $"GC Alloc (frame): {gcAlloc.LastValue} bytes", textStyle);
    }

    static double GetRecorderMs(ProfilerRecorder recorder)
    {
        if (!recorder.Valid || recorder.Count == 0)
            return 0;

        return recorder.LastValue / 1_000_000.0; // ns → ms
    }
}
