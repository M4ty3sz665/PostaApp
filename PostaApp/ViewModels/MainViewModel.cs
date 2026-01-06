using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PostaApp.Model;


namespace PostaApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ObservableCollection<Package> Packages { get; } = new();

    // Segédlista a ComboBox-hoz
    public IEnumerable<PackageStatus> StatusOptions => Enum.GetValues<PackageStatus>();

    [ObservableProperty]
    private Package? selectedPackage;

    // Új csomag mezői (a formhoz)
    [ObservableProperty] private string newName = "";
    [ObservableProperty] private string newOrigin = "";
    [ObservableProperty] private string newDestination = "";
    [ObservableProperty] private double newPrice = 100;
    [ObservableProperty] private int newDays = 1;

    [RelayCommand]
    public void AddPackage()
    {
        // Validáció: Név ne legyen üres, ár ne legyen 0 vagy negatív
        if (string.IsNullOrWhiteSpace(NewName) || NewPrice <= 0) return;

        Packages.Add(new Package
        {
            Name = NewName,
            OriginCity = NewOrigin,
            DestinationCity = NewDestination,
            Price = NewPrice,
            DaysUntilDelivery = NewDays
        });

        // Form alaphelyzetbe állítása
        NewName = ""; NewOrigin = ""; NewDestination = ""; NewPrice = 100; NewDays = 1;
    }

    [RelayCommand]
    public void SaveEdit()
    {
        if (SelectedPackage == null) return;

        // Üzleti szabály: Ha kiszállítva/törölve -> napok száma fix 0
        if (SelectedPackage.Status == PackageStatus.Kiszállítva ||
            SelectedPackage.Status == PackageStatus.Törölve)
        {
            SelectedPackage.DaysUntilDelivery = 0;
        }

        // Ár validáció javítás közben is
        if (SelectedPackage.Price <= 0) SelectedPackage.Price = 1;

        // Értesítjük a listát, hogy változás történt a kijelzőn
        var temp = SelectedPackage;
        SelectedPackage = null;
        SelectedPackage = temp;
    }

    // Mentés szűréssel
    public List<Package> GetFilteredPackages(PackageStatus? filter)
    {
        if (filter == null) return Packages.ToList();
        return Packages.Where(p => p.Status == filter).ToList();
    }
}