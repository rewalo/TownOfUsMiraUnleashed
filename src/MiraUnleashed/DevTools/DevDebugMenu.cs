using System.Reflection;
using MiraUnleashed.Achievements;
using Reactor.Utilities.Attributes;
using TownOfUs.Achievements;
using UnityEngine;

namespace MiraUnleashed.DebugTools;

/// <summary>
/// In-game IMGUI achievements debug tool (F8), installed only in dev builds.
/// Local-only: it never sends network traffic and never changes game state.
/// </summary>
[RegisterInIl2Cpp]
public sealed class DevDebugMenu(nint cppPtr) : MonoBehaviour(cppPtr)
{
    private static DevDebugMenu? _instance;

    private readonly HashSet<string> _loggedErrors = new();
    private Rect _windowRect = new(0, 0, 820, 820);
    private Vector2 _scroll;
    private string? _error;

    private bool _visible;
    private bool _loggedOpen;
    private bool _positioned;
    private bool _dragging;
    private bool _resizing;
    private Vector2 _dragOffset;
    private const float MinWidth = 360;
    private const float MinHeight = 240;
    private const float ResizeGrip = 22;

    private GUIStyle? _titleStyle;
    private GUIStyle? _descStyle;
    private GUIStyle? _errorStyle;
    private GUIStyle? _gripStyle;

    /// <summary>
    /// Installs the debug menu. No-op in release builds and if already installed.
    /// </summary>
    public static void Install()
    {
        if (!MiraUnleashedPlugin.IsDevBuild || (_instance != null && _instance))
        {
            return;
        }

        var go = new GameObject("MiraUnleashedDevMenu");
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<DevDebugMenu>();
        Info("Dev debug menu installed (F8).");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            _visible = !_visible;
            if (_visible && !_loggedOpen)
            {
                _loggedOpen = true;
                Info("Dev menu opened");
            }
        }

        if (!_visible)
        {
            _dragging = false;
            _resizing = false;
            return;
        }

        var m = Input.mousePosition;
        var gx = m.x;
        var gy = Screen.height - m.y;
        var titleBar = new Rect(_windowRect.x, _windowRect.y, _windowRect.width, 28);
        var grip = new Rect(_windowRect.xMax - ResizeGrip, _windowRect.yMax - ResizeGrip, ResizeGrip, ResizeGrip);

        if (Input.GetMouseButtonDown(0))
        {
            if (grip.Contains(new Vector2(gx, gy)))
            {
                _resizing = true;
            }
            else if (titleBar.Contains(new Vector2(gx, gy)))
            {
                _dragging = true;
                _dragOffset = new Vector2(gx - _windowRect.x, gy - _windowRect.y);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
            _resizing = false;
        }

        if (_dragging && Input.GetMouseButton(0))
        {
            _windowRect.x = Mathf.Clamp(gx - _dragOffset.x, 40 - _windowRect.width, Screen.width - 40);
            _windowRect.y = Mathf.Clamp(gy - _dragOffset.y, 0, Screen.height - 40);
        }

        if (_resizing && Input.GetMouseButton(0))
        {
            _windowRect.width = Mathf.Clamp(gx - _windowRect.x, MinWidth, Screen.width - _windowRect.x);
            _windowRect.height = Mathf.Clamp(gy - _windowRect.y, MinHeight, Screen.height - _windowRect.y);
        }
    }

    private void CenterWindow()
    {
        _windowRect.width = Mathf.Min(_windowRect.width, Screen.width - 40);
        _windowRect.height = Mathf.Min(_windowRect.height, Screen.height - 40);
        _windowRect.x = (Screen.width - _windowRect.width) / 2;
        _windowRect.y = (Screen.height - _windowRect.height) / 2;
        _positioned = true;
    }

    private void OnGUI()
    {
        if (!_visible)
        {
            return;
        }

        _titleStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
        _descStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 10, wordWrap = true };
        _errorStyle ??= new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            wordWrap = true,
            normal = { textColor = Color.red },
        };
        _gripStyle ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerRight, fontSize = 14 };

        if (!_positioned)
        {
            CenterWindow();
        }

        GUILayout.BeginArea(_windowRect, GUI.skin.box);
        try
        {
            GUILayout.BeginHorizontal(GUILayout.Height(28));
            GUILayout.Label("Mira Unleashed Dev Menu (F8) — drag here, resize at corner", _titleStyle);
            if (GUILayout.Button("Reset position", GUILayout.Width(100)))
            {
                CenterWindow();
            }

            if (GUILayout.Button("Close", GUILayout.Width(60)))
            {
                _visible = false;
            }

            GUILayout.EndHorizontal();

            var scrollOpen = false;
            try
            {
                _scroll = GUILayout.BeginScrollView(_scroll);
                scrollOpen = true;
                DrawAchievementsTab();
                _error = null;
            }
            finally
            {
                if (scrollOpen)
                {
                    GUILayout.EndScrollView();
                }
            }

            if (_error != null)
            {
                GUILayout.Label(_error, _errorStyle);
            }
        }
        catch (Exception ex)
        {
            _error = ex.Message;
            if (_loggedErrors.Add(ex.Message))
            {
                Error(ex.ToString());
            }
        }
        finally
        {
            GUILayout.EndArea();
        }

        if (_visible)
        {
            GUI.Label(new Rect(_windowRect.xMax - ResizeGrip, _windowRect.yMax - ResizeGrip, ResizeGrip, ResizeGrip), "◢",
                _gripStyle);
        }
    }

    // --- Achievements ---

    private void DrawAchievementsTab()
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            GUILayout.Label("AchievementsAPI is not loaded.");
            return;
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Unlock all"))
        {
            foreach (var spec in MuAchievements.All)
            {
                AchievementsAPIHelper.Unlock(spec);
            }
        }

        if (GUILayout.Button("Reset all"))
        {
            foreach (var spec in MuAchievements.All)
            {
                AchievementsAPIHelper.Reset(spec);
            }
        }

        GUILayout.EndHorizontal();

        foreach (var spec in MuAchievements.All)
        {
            DrawAchievementRow(spec);
        }
    }

    private void DrawAchievementRow(AchievementSpec spec)
    {
        var name = spec.Name + (spec.Hidden ? " (hidden)" : string.Empty);
        if (spec.ProgressGoal > 0)
        {
            var current = AchievementsAPIHelper.GetCurrentValue(spec);
            name += $"  [{current}/{spec.ProgressGoal}]";
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label(AchievementsAPIHelper.IsUnlocked(spec) ? "[x]" : "[ ]", GUILayout.Width(30));
        GUILayout.Label(name);
        GUILayout.EndHorizontal();
        GUILayout.Label(spec.Description, _descStyle);

        GUILayout.BeginHorizontal();
        GUI.enabled = !AchievementsAPIHelper.IsUnlocked(spec);
        if (GUILayout.Button("Unlock", GUILayout.Width(70)))
        {
            AchievementsAPIHelper.Unlock(spec);
        }

        GUI.enabled = true;
        if (GUILayout.Button("Reset", GUILayout.Width(60)))
        {
            AchievementsAPIHelper.Reset(spec);
        }

        if (spec.ProgressGoal > 0)
        {
            if (GUILayout.Button("+1", GUILayout.Width(40)))
            {
                AchievementsAPIHelper.Increment(spec, 1);
            }

            GUI.enabled = AchievementsAPIHelper.GetCurrentValue(spec) > 0;
            if (GUILayout.Button("-1", GUILayout.Width(40)))
            {
                AchievementsAPIHelper.SetValue(spec, Mathf.Max(0, AchievementsAPIHelper.GetCurrentValue(spec) - 1));
            }

            GUI.enabled = true;
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(6);
    }
}
