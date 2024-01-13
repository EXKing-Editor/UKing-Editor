﻿using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using NxEditor.Generators;
using NxEditor.Models;
using NxEditor.PluginBase;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.Components;

public class EditorManager : IEditorManager
{
    public static EditorManager Shared { get; } = new();

    public IEditor? Current => ShellDockFactory.Current() as IEditor;

    public async Task<bool> TryLoadEditor(IFileHandle handle)
    {
        try {
            await LoadEditor(handle);
            return true;
        }
        catch (Exception ex) {
            App.Log(ex);
        }

        return false;
    }

    public async Task LoadEditor(IFileHandle handle)
    {
        App.Log($"Processing {handle.Name}");

        if (ShellDockFactory.TryFocus(handle.Id, out IDockable? dock) && dock is IEditor) {
            return;
        }

        IFormatService service = await ServiceLoader.Shared
            .RequestService(handle);

        if (service is IEditor editor) {
            _ = editor.Read();
            ShellDockFactory.AddDoc((Document)editor);
            if (handle.FilePath is not null) {
                RecentFiles.Shared.AddPath(handle.FilePath);
            }

            return;
        }

        throw new InvalidCastException($"Could not cast the found type of {service} to {typeof(Editor<>).Name}");
    }
}
