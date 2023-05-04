﻿using Dock.Model.Core;
using ExKingEditor.Core.Extensions;
using ExKingEditor.Generators;
using System.Runtime.CompilerServices;

namespace ExKingEditor.Models;

public static class EditorMgr
{
    private static readonly Dictionary<string, string> _editors = ResourceExtension.Parse<App, Dictionary<string, string>>("Editors.json")!;

    public static ReactiveEditor? Current => ShellDockFactory.Current() as ReactiveEditor;

    public static bool TryLoadEditorSafe(string path, out ReactiveEditor? reactiveEditor)
    {
        try {
            return TryLoadEditor(path, out reactiveEditor);
        }
        catch (Exception ex) {
            App.Log(ex);
        }

        reactiveEditor = null;
        return false;
    }

    public static bool TryLoadEditor(string path, out ReactiveEditor? reactiveEditor)
    {
        App.Log($"Processing {Path.GetFileName(path)}");

        if (!CanEdit(path)) {
            throw new NotSupportedException($"Unsupported file type {Path.GetExtension(path)}");
        }

        if (ShellDockFactory.TryFocus(path, out IDockable? dock) && dock is ReactiveEditor exEditor) {
            reactiveEditor = exEditor;
            return true;
        }

        string ext = GetExt(path);
        object? instance = Activator.CreateInstance(Type.GetType($"{nameof(ExKingEditor)}.ViewModels.Editors.{_editors[ext]}")
            ?? throw new InvalidDataException($"Invalid editor type for '{ext}' - '{_editors[ext]}' was not found"), path);

        if (instance is ReactiveEditor editor) {
            reactiveEditor = editor;
            ShellDockFactory.AddDoc(editor);
            StateMgr.Shared.Recent.AddPath(path);

            return true;
        }

        throw new InvalidCastException($"Could not cast the found type of {instance} to {nameof(ReactiveEditor)}");
    }

    public static bool CanEdit(string file, out string? editor)
    {
        return _editors.TryGetValue(GetExt(file), out editor);
    }

    public static bool CanEdit(string file)
    {
        return _editors.ContainsKey(GetExt(file));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetExt(string file)
    {
        if (file.EndsWith(".zs")) {
            file = file[..^3];
        }

        return Path.GetExtension(file);
    }
}
