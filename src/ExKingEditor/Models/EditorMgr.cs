﻿using ExKingEditor.Core.Extensions;

namespace ExKingEditor.Models;

public static class EditorMgr
{
    private static readonly Dictionary<string, string> _editors = EmbedExtension.Parse<App, Dictionary<string, string>>("Editors.json")!;

    public static ReactiveEditor GetEditor(string file)
    {
        string ext = GetExt(file);
        object? instance = Activator.CreateInstance(Type.GetType($"{nameof(ExKingEditor)}.ViewModels.Editors.{_editors[ext]}")
            ?? throw new InvalidDataException($"Invalid editor type for '{ext}' - '{_editors[ext]}' was not found"), file);

        if (instance is ReactiveEditor editor) {
            return editor;
        }

        throw new InvalidCastException($"Could not cast the found type of {instance} to {nameof(ReactiveEditor)}");
    }

    public static bool CanEdit(string file)
    {
        return _editors.ContainsKey(GetExt(file));
    }

    private static string GetExt(string file)
    {
        if (file.EndsWith(".zs")) {
            file = file[..^3];
        }

        return Path.GetExtension(file);
    }
}