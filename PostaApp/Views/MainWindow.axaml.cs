using Avalonia.Controls;
using Avalonia.Platform.Storage;
using PostaApp.ViewModels;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using PostaApp.Model;
using System;
using Avalonia.Interactivity;

namespace PostaApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    

    private async void OnSaveAll(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await SaveToFile(null); // Szűrés nélkül
    }

    private async void OnSaveDeleted(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await SaveToFile(PackageStatus.Törölve); // Csak a töröltek
    }

    private async System.Threading.Tasks.Task SaveToFile(PackageStatus? filter)
    {
        var vm = (MainViewModel)DataContext!;

        // Fájlmentő ablak megnyitása
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Csomagok mentése",
            FileTypeChoices = new[]
    {
        new FilePickerFileType("JSON fájlok")
        {
            Patterns = new[] { "*.json" },
            MimeTypes = new[] { "application/json" }
        }
    }
        });

        if (file != null)
        {
            var data = vm.GetFilteredPackages(filter);
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await using var stream = await file.OpenWriteAsync();
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);
        }
    }

    // Add hozzá ezt a metódust a MainWindow.axaml.cs-be:
    private async void OnLoadAll(object? sender, RoutedEventArgs e)
    {
        var vm = (MainViewModel)DataContext!;

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Csomagok betöltése",
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } } }
        });

        if (files.Count > 0)
        {
            await using var stream = await files[0].OpenReadAsync();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var loadedPackages = JsonSerializer.Deserialize<List<Package>>(json);
            if (loadedPackages != null)
            {
                vm.Packages.Clear();
                foreach (var p in loadedPackages) vm.Packages.Add(p);
            }
        }
    }
}