/*
┌──────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)             │
│  Repository: GitHub (https://github.com/IvanMurzak/Unity-MCP)    │
│  Copyright (c) 2025 Ivan Murzak                                  │
│  Licensed under the Apache License, Version 2.0.                 │
│  See the LICENSE file in the project root for more information.  │
└──────────────────────────────────────────────────────────────────┘
*/

#nullable enable
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace com.IvanMurzak.Unity.MCP.RuntimeAddon.Editor
{
    /// <summary>
    /// Adds UNITY_MCP_RUNTIME scripting define symbol when the optional runtime add-on package is installed.
    /// This allows runtime assemblies and runtime plugin dependencies from the main package to be included in player builds.
    /// </summary>
    [InitializeOnLoad]
    public static class RuntimeDefineSetup
    {
        private const string DefineSymbol = "UNITY_MCP_RUNTIME";

        private static readonly NamedBuildTarget[] BuildTargets =
        {
            NamedBuildTarget.Standalone,
            NamedBuildTarget.Android,
            NamedBuildTarget.iOS,
            NamedBuildTarget.WebGL,
            NamedBuildTarget.Server,
        };

        static RuntimeDefineSetup()
        {
            AddDefineSymbol();
        }

        private static void AddDefineSymbol()
        {
            foreach (var target in BuildTargets)
            {
                try
                {
                    PlayerSettings.GetScriptingDefineSymbols(target, out var defines);
                    if (defines.Contains(DefineSymbol))
                        continue;

                    PlayerSettings.SetScriptingDefineSymbols(target, defines.Append(DefineSymbol).ToArray());
                }
                catch (Exception)
                {
                    // Build target module may not be installed in this Unity editor.
                }
            }
        }

        /// <summary>
        /// Removes UNITY_MCP_RUNTIME from known targets.
        /// Useful as a manual cleanup helper after removing the runtime add-on package.
        /// </summary>
        public static void RemoveDefineSymbol()
        {
            foreach (var target in BuildTargets)
            {
                try
                {
                    PlayerSettings.GetScriptingDefineSymbols(target, out var defines);
                    var filtered = defines.Where(d => d != DefineSymbol).ToArray();
                    if (filtered.Length == defines.Length)
                        continue;

                    PlayerSettings.SetScriptingDefineSymbols(target, filtered);
                }
                catch (Exception)
                {
                    // Build target module may not be installed in this Unity editor.
                }
            }
        }
    }
}
