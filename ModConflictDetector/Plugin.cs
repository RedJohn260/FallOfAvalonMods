using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Awaken.TG.Main.UI.TitleScreen;

namespace ModConflictDetector
{
    [BepInPlugin(PluginConsts.PLUGIN_GUID, PluginConsts.PLUGIN_NAME, PluginConsts.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Log;
        internal static PluginConfig PluginConfig;
        internal static Plugin Instance;

        public Harmony HarmonyInstance { get; set; }

        // UI State
        private bool _showUi = false;
        private string _logContent = "";
        private Vector2 _scrollPosition = Vector2.zero;

        // Window Texture Cache
        private Texture2D _windowTexture;

        // Runtime Detection State
        private bool _hasScanned = false;

        // Window ID
        private int _windowId = 9876;

        public void Awake()
        {
            Log = Logger;
            Instance = this;

            Log.LogInfo($"Plugin {PluginConsts.PLUGIN_GUID} is loading...");

            // Initialize Configuration
            PluginConfig = new PluginConfig(Config);

            // Create window background texture ONCE
            _windowTexture = CreateWindowTexture();

            // Apply Harmony Patches
            HarmonyInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            // START RUNTIME SEARCH (UnityExplorer style)
            StartCoroutine(WaitForTitleScreenRoutine());

            Log.LogInfo($"Plugin {PluginConsts.PLUGIN_GUID} is loaded! Searching for Title Screen...");
        }

        // Helper to create a dark semi-transparent texture
        private Texture2D CreateWindowTexture()
        {
            var tex = new Texture2D(1, 1);
            // Dark grey with slight transparency
            tex.SetPixel(0, 0, new Color(0f, 0f, 0f, 1f));
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            return tex;
        }

        public void Update()
        {
            // Handle Manual Toggle via Keybind
            if (PluginConfig != null && PluginConfig.ToggleKey != null)
            {
                if (Input.GetKeyDown(PluginConfig.ToggleKey.Value))
                {
                    _showUi = !_showUi;
                }
            }
        }

        public void OnDestroy()
        {
            Log.LogInfo($"Plugin {PluginConsts.PLUGIN_GUID} is unloading...");
            HarmonyInstance?.UnpatchSelf();
            Log.LogInfo($"Plugin {PluginConsts.PLUGIN_GUID} is unloaded!");
        }

        // ========================================================================
        // RUNTIME SEARCH (UnityExplorer Style)
        // ========================================================================

        private IEnumerator WaitForTitleScreenRoutine()
        {
            float logTimer = 0f;

            while (!_hasScanned)
            {
                yield return new WaitForSeconds(1.0f);

                logTimer += 1f;
                if (logTimer >= 5f)
                {
                    Log.LogInfo("Scanning scene for VTitleScreenUI...");
                    logTimer = 0f;
                }

                var titleScreenUI = GameObject.FindObjectOfType<VTitleScreenUI>(true);

                if (titleScreenUI != null)
                {
                    Log.LogInfo("TITLE SCREEN DETECTED! Running conflict scan...");
                    ExecuteScan();
                    _hasScanned = true;
                    yield break;
                }
            }
        }

        // ========================================================================
        // SCANNING LOGIC (FIXED FOR MANUAL TOGGLE)
        // ========================================================================

        private void ExecuteScan()
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("=== SCANNING FOR MOD CONFLICTS ===");
                sb.AppendLine();

                // 1. Check GUID Conflicts
                var guidConflicts = CheckGuidConflicts();
                if (guidConflicts.Count > 0)
                {
                    sb.AppendLine($"[CRITICAL] Found {guidConflicts.Count} Duplicate GUID(s):");
                    foreach (var guid in guidConflicts.Keys)
                    {
                        sb.AppendLine($" - GUID: {guid}");
                        foreach (var plugin in guidConflicts[guid])
                        {
                            sb.AppendLine($"   -> {plugin.Name} ({plugin.Assembly})");
                        }
                    }
                    sb.AppendLine();
                }

                // 2. Check Patch Conflicts
                var patchConflicts = CheckPatchConflicts();
                if (patchConflicts.Count > 0)
                {
                    sb.AppendLine($"[WARNING] Found {patchConflicts.Count} Potential Patch Conflict(s):");
                    sb.AppendLine();

                    for (int i = 0; i < patchConflicts.Count; i++)
                    {
                        sb.Append(GenerateGenericConflictReport(patchConflicts[i], i));
                        sb.AppendLine();
                    }
                }

                if (guidConflicts.Count == 0 && patchConflicts.Count == 0)
                {
                    sb.AppendLine("No conflicts detected!");
                }

                // --- CRITICAL FIX: ALWAYS SAVE REPORT ---
                // We save the content here so the manual toggle (Key) has something to show.
                // Even if we don't auto-open the window, we want the text ready.
                _logContent = sb.ToString();

                // Log to Console
                if (PluginConfig.EnableDetailedLogging.Value)
                {
                    Log.LogWarning(_logContent);
                }

                // --- SEVERITY LOGIC ---
                bool hasSeriousIssues = false;

                // GUID conflicts are always serious
                if (guidConflicts.Count > 0) hasSeriousIssues = true;

                // Check for Transpilers or Hero conflicts
                foreach (var c in patchConflicts)
                {
                    string methodName = c.Method.Name.ToUpper();
                    string className = c.Method.DeclaringType?.Name.ToUpper() ?? "";

                    // A. Transpilers
                    if (c.TranspilerCount > 0)
                    {
                        hasSeriousIssues = true;
                        continue;
                    }

                    // B. Critical Method Names
                    if (methodName.Contains("UPDATE") ||
                        methodName.Contains("FIXEDUPDATE") ||
                        methodName.Contains("LATEUPDATE") ||
                        methodName.Contains("LOAD") ||
                        methodName.Contains("SAVE") ||
                        methodName.Contains("AWAKE") ||
                        methodName.Contains("START") ||
                        methodName.Contains("ONDESTROY") ||
                        methodName.Contains("DIE") ||
                        methodName.Contains("TAKEDAMAGE") ||
                        methodName.Contains("GETINPUT"))
                    {
                        hasSeriousIssues = true;
                        continue;
                    }

                    // C. Critical Classes
                    if (className.Contains("HERO") ||
                        className.Contains("SAVE") ||
                        className.Contains("LOAD") ||
                        className.Contains("INPUT") ||
                        className.Contains("CAMERA") ||
                        className.Contains("GAMEPLAY"))
                    {
                        hasSeriousIssues = true;
                        continue;
                    }
                }

                // --- DECIDE AUTO-OPEN ---
                // Show if: (Config says Yes) OR (We found Serious Issues)
                bool shouldShow = PluginConfig.EnablePopup.Value; //|| hasSeriousIssues;

                // ONLY set _showUi to true automatically if conditions are met.
                // If false, user must press Key (Update method) to see it.
                if (shouldShow && (guidConflicts.Count > 0 || patchConflicts.Count > 0))
                {
                    _showUi = true;
                }
            }
            catch (Exception ex)
            {
                Log.LogError($"Error during conflict scan: {ex.Message}");
            }
        }

        // --- Helper: Check Duplicate GUIDs ---
        private Dictionary<string, List<PluginInfo>> CheckGuidConflicts()
        {
            var conflicts = new Dictionary<string, List<PluginInfo>>();
            var allPlugins = GetAllPlugins();

            foreach (var plugin in allPlugins)
            {
                if (!conflicts.ContainsKey(plugin.GUID))
                {
                    conflicts[plugin.GUID] = new List<PluginInfo>();
                }
                conflicts[plugin.GUID].Add(plugin);
            }

            var duplicates = conflicts.Where(kvp => kvp.Value.Count > 1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return duplicates;
        }

        // --- Helper: Check Patch Conflicts ---
        private List<MethodConflictInfo> CheckPatchConflicts()
        {
            var conflicts = new List<MethodConflictInfo>();
            var pluginMap = GetAllPlugins().ToDictionary(p => p.Assembly, p => p);

            foreach (var method in Harmony.GetAllPatchedMethods())
            {
                var info = Harmony.GetPatchInfo(method);
                if (info == null) continue;

                var combinedPatches = new List<Patch>();
                foreach (var p in info.Prefixes) combinedPatches.Add(p);
                foreach (var p in info.Postfixes) combinedPatches.Add(p);
                foreach (var p in info.Transpilers) combinedPatches.Add(p);

                var distinctOwners = combinedPatches.Select(p => GetFriendlyOwnerName(p, pluginMap)).Distinct().ToList();

                if (distinctOwners.Count <= 1 && info.Transpilers.Count == 0) continue;

                var details = new List<PatchDetail>();
                foreach (var p in info.Prefixes) details.Add(new PatchDetail { OwnerName = GetFriendlyOwnerName(p, pluginMap), Type = "PREFIX", Order = p.index });
                foreach (var p in info.Postfixes) details.Add(new PatchDetail { OwnerName = GetFriendlyOwnerName(p, pluginMap), Type = "POSTFIX", Order = p.index });
                foreach (var p in info.Transpilers) details.Add(new PatchDetail { OwnerName = GetFriendlyOwnerName(p, pluginMap), Type = "TRANSPILER", Order = p.index });

                details.Sort((a, b) => a.Order.CompareTo(b.Order));

                conflicts.Add(new MethodConflictInfo
                {
                    Method = method,
                    Owners = distinctOwners,
                    TranspilerCount = info.Transpilers.Count,
                    Details = details
                });
            }
            return conflicts;
        }

        // --- Helper: Generate GENERIC Conflict Report ---
        private string GenerateGenericConflictReport(MethodConflictInfo conflict, int index)
        {
            var sb = new StringBuilder();

            // 1. Header
            sb.AppendLine($"{index + 1}. {conflict.Method.DeclaringType?.Name}.{conflict.Method.Name}");

            // 2. Mods List
            string transpilerTag = conflict.TranspilerCount > 0 ? " [Transpiler Active]" : "";
            string modsList = string.Join(", ", conflict.Owners);
            sb.AppendLine($"   Mods Involved: {modsList}{transpilerTag}");

            // 3. GENERIC REPORTING LOGIC

            if (conflict.TranspilerCount > 0)
            {
                // SCENARIO: TRANSPILER (CODE REWRITING)
                sb.AppendLine($"   Status: High Risk (Code Rewriting).");
                sb.AppendLine($"   Explanation: This mod uses a Transpiler, which directly modifies the game's underlying C# code instructions. This is the most invasive patching method.");
                sb.AppendLine($"   What it means: The game logic for '{conflict.Method.Name}' has been fundamentally changed by {modsList}.");
                sb.AppendLine($"   Priority Status: N/A (Code Rewriting).");
                sb.AppendLine($"   Verdict: If you experience crashes or instability in this area, disable this mod immediately.");
            }
            else
            {
                // SCENARIO: EXECUTION ORDER (MULTIPLE MODS)
                // Determine winner (Last in list)
                var winner = conflict.Details.LastOrDefault().OwnerName;

                sb.AppendLine($"   Status: Potential Value Conflict (Low Risk).");
                sb.AppendLine($"   Explanation: Multiple mods are patching the same method. Harmony executes them in a chain. The last mod to run will set the final state.");
                sb.AppendLine($"   What it means: '{winner}' runs last. It likely overwrites or finalizes values set by the other mod(s).");
                sb.AppendLine($"   Priority Status: '{winner}' has final say.");
                sb.AppendLine($"   Verdict: Monitor for bugs where features of the earlier mods seem inactive. Generally safe to run together.");
            }

            return sb.ToString();
        }

        // --- Helper: Identify Mod/Assembly ---
        private string GetFriendlyOwnerName(Patch patch, Dictionary<string, PluginInfo> pluginMap)
        {
            try
            {
                var declaringAssembly = patch.PatchMethod.DeclaringType.Assembly.GetName().Name;
                if (pluginMap.TryGetValue(declaringAssembly, out var plugin))
                {
                    return $"{plugin.Name} (v{plugin.Version})";
                }
                return declaringAssembly;
            }
            catch
            {
                return patch.owner;
            }
        }

        // --- Helper: Get All Plugins ---
        private List<PluginInfo> GetAllPlugins()
        {
            var plugins = new List<PluginInfo>();
            var basePluginType = typeof(BaseUnityPlugin);

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (asm.IsDynamic) continue;
                    var pluginTypes = asm.GetTypes().Where(t => basePluginType.IsAssignableFrom(t) && !t.IsAbstract);
                    foreach (var t in pluginTypes)
                    {
                        var attr = t.GetCustomAttribute<BepInPlugin>();
                        if (attr != null)
                        {
                            plugins.Add(new PluginInfo { Name = attr.Name, GUID = attr.GUID, Version = attr.Version.ToString(), Assembly = asm.GetName().Name });
                        }
                    }
                }
                catch { }
            }
            return plugins;
        }

        // ========================================================================
        // UNITY UI (FIXED COLORS & CLICKABILITY)
        // ========================================================================

        private void OnGUI()
        {
            if (!_showUi) return;

            // Ensure GUI is enabled for interaction
            GUI.enabled = true;

            // 1. Define Sizes
            float width = 850;
            float height = 650;
            float x = (Screen.width - width) / 2;
            float y = (Screen.height - height) / 2;
            Rect windowRect = new Rect(x, y, width, height);

            // 2. Create "Game Styled" Window Style
            var windowStyle = new GUIStyle(GUI.skin.window);
            // FORCE DARK BACKGROUND FOR ALL STATES (Fixes the color flash)
            windowStyle.normal.background = _windowTexture;
            windowStyle.onFocused.background = _windowTexture;
            windowStyle.focused.background = _windowTexture;
            windowStyle.onNormal.background = _windowTexture;

            // Optional: Remove header padding for cleaner look
            windowStyle.padding = new RectOffset(5, 5, 5, 5);

            // 3. Draw Window and Store the returned Rect (Fixes button coordinates)
            windowRect = GUILayout.Window(_windowId, windowRect, DrawWindowContent, "", windowStyle);
        }

        // Content inside window
        private void DrawWindowContent(int id)
        {
            // 1. Define Styles

            // Button Style: Add hover effects so user knows it's clickable
            var btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.fontSize = 14;
            btnStyle.fontStyle = FontStyle.Bold;
            btnStyle.fixedHeight = 35;
            btnStyle.padding = new RectOffset(10, 10, 10, 10);

            // Add distinct colors for interactions
            btnStyle.normal.textColor = Color.white;
            btnStyle.hover.textColor = Color.cyan; // Cyan on hover (Game-ish)
            btnStyle.active.textColor = Color.gray;  // Gray when clicked

            // Text Styles
            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
                padding = new RectOffset(0, 0, 10, 10)
            };

            var bodyStyle = new GUIStyle(GUI.skin.textArea)
            {
                fontSize = 13,
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f) },
                padding = new RectOffset(10, 10, 10, 10),
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                richText = true
            };

            // 2. Layout
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            // Header
            GUILayout.Label("Detected Conflicts:", titleStyle);

            // Scroll Area
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
            GUILayout.Label(_logContent, bodyStyle);
            GUILayout.EndScrollView();

            // Footer / Buttons
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Close Button
            if (GUILayout.Button("Close (ESC)", btnStyle, GUILayout.Width(220)))
            {
                _showUi = false;
            }

            // Open Log Folder Button
            if (GUILayout.Button("Open Log Folder", btnStyle, GUILayout.Width(220)))
            {
                Application.OpenURL(BepInEx.Paths.BepInExRootPath);
            }

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        // ========================================================================
        // DATA STRUCTURES
        // ========================================================================

        private struct PluginInfo
        {
            public string Name;
            public string GUID;
            public string Version;
            public string Assembly;
        }

        private struct MethodConflictInfo
        {
            public MethodBase Method;
            public List<string> Owners;
            public int TranspilerCount;
            public List<PatchDetail> Details;
        }

        private struct PatchDetail
        {
            public string OwnerName;
            public string Type;
            public int Order;
        }
    }
}
