using System.Reflection;
using AchievementsAPI.API;
using MiraUnleashed.Achievements;
using Reactor.Utilities.Attributes;
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
    private readonly Dictionary<Type, List<(PropertyInfo Prop, BaseAchievement Ach)>> _achievementCache = new();
    private Rect _windowRect = new(0, 0, 520, 560);
    private Vector2 _scroll;
    private string? _error;

    private bool _visible;
    private bool _loggedOpen;
    private bool _positioned;
    private bool _dragging;
    private Vector2 _dragOffset;

    private int _achievementsTabIndex = -1;

    private GUIStyle? _titleStyle;
    private GUIStyle? _descStyle;
    private GUIStyle? _errorStyle;

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
            return;
        }

        var m = Input.mousePosition;
        var gx = m.x;
        var gy = Screen.height - m.y;
        var titleBar = new Rect(_windowRect.x, _windowRect.y, _windowRect.width, 28);

        if (Input.GetMouseButtonDown(0) && titleBar.Contains(new Vector2(gx, gy)))
        {
            _dragging = true;
            _dragOffset = new Vector2(gx - _windowRect.x, gy - _windowRect.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
        }

        if (_dragging && Input.GetMouseButton(0))
        {
            _windowRect.x = Mathf.Clamp(gx - _dragOffset.x, 40 - _windowRect.width, Screen.width - 40);
            _windowRect.y = Mathf.Clamp(gy - _dragOffset.y, 0, Screen.height - 40);
        }
    }

    private void CenterWindow()
    {
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

        if (!_positioned)
        {
            CenterWindow();
        }

        GUILayout.BeginArea(_windowRect, GUI.skin.box);
        try
        {
            GUILayout.BeginHorizontal(GUILayout.Height(28));
            GUILayout.Label("Mira Unleashed Dev Menu (F8) — drag here", _titleStyle);
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
    }

    // --- Achievements ---

    private void DrawAchievementsTab()
    {
        var tabs = AchievementsManager.Tabs;
        if (tabs.Count == 0)
        {
            GUILayout.Label("No achievement tabs registered.");
            return;
        }

        if (_achievementsTabIndex < 0 || _achievementsTabIndex >= tabs.Count)
        {
            _achievementsTabIndex = Math.Max(0, tabs.FindIndex(t => t is MiraUnleashedAchievementsTab));
        }

        GUILayout.BeginHorizontal();
        for (var i = 0; i < tabs.Count; i++)
        {
            if (GUILayout.Toggle(_achievementsTabIndex == i, tabs[i].Name, GUI.skin.button))
            {
                _achievementsTabIndex = i;
            }
        }

        GUILayout.EndHorizontal();

        var tab = tabs[_achievementsTabIndex];
        var achievements = GetAchievements(tab);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Unlock all"))
        {
            foreach (var (_, ach) in achievements)
            {
                ach.Unlock();
            }
        }

        if (GUILayout.Button("Reset all"))
        {
            foreach (var (_, ach) in achievements)
            {
                ResetAchievement(ach);
            }
        }

        GUILayout.EndHorizontal();

        foreach (var (_, ach) in achievements)
        {
            DrawAchievementRow(ach);
        }
    }

    private List<(PropertyInfo Prop, BaseAchievement Ach)> GetAchievements(AchievementsTab tab)
    {
        var type = tab.GetType();
        if (_achievementCache.TryGetValue(type, out var cached))
        {
            return cached;
        }

        var list = new List<(PropertyInfo, BaseAchievement)>();
        foreach (var prop in type.GetProperties()
                     .Where(p => typeof(BaseAchievement).IsAssignableFrom(p.PropertyType)))
        {
            if (prop.GetValue(tab) is BaseAchievement ach)
            {
                list.Add((prop, ach));
            }
        }

        _achievementCache[type] = list;
        return list;
    }

    private void DrawAchievementRow(BaseAchievement ach)
    {
        var name = ach.Name + (ach.Hidden ? " (hidden)" : string.Empty);
        if (ach is CountAchievement count)
        {
            name += $"  [{count.CurrentValue}/{count.RequiredValue}]";
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label(ach.Unlocked ? "[x]" : "[ ]", GUILayout.Width(30));
        GUILayout.Label(name);
        GUILayout.EndHorizontal();
        GUILayout.Label(ach.Description, _descStyle);

        GUILayout.BeginHorizontal();
        GUI.enabled = !ach.Unlocked;
        if (GUILayout.Button("Unlock", GUILayout.Width(70)))
        {
            ach.Unlock();
        }

        GUI.enabled = true;
        if (GUILayout.Button("Reset", GUILayout.Width(60)))
        {
            ResetAchievement(ach);
        }

        if (ach is CountAchievement c)
        {
            if (GUILayout.Button("+1", GUILayout.Width(40)))
            {
                c.Increment(1);
            }

            GUI.enabled = c.CurrentValue > 0;
            if (GUILayout.Button("-1", GUILayout.Width(40)))
            {
                c.SetValue(Math.Max(0, c.CurrentValue - 1));
            }

            GUI.enabled = true;
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(6);
    }

    private static void ResetAchievement(BaseAchievement ach)
    {
        if (ach is CountAchievement count)
        {
            AchievementStorage.AchievementDataReset(count, 0, false);
        }
        else
        {
            AchievementStorage.AchievementDataReset(ach, false);
        }
    }
}
