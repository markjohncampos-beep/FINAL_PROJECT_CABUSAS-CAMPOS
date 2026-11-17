using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CampusITAssetTracker.Models;
using CampusITAssetTracker.Interfaces;
using CampusITAssetTracker.Validators;

namespace CampusITAssetTracker
{
    internal class Program
    {
        static int mainMenuTop;
        static List<CampusAsset> assets = new List<CampusAsset>();

        static string filePath = "assets.json";

        static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        static void Main(string[] args) 
        {
            LoadAssets();

            Console.Title = "Campus IT Asset Management System";

            string[] menuOptions =
            {
                "Add New Asset",
                "View All Assets",
                "Search Asset",
                "Filter Assets",
                "Update Asset",
                "Delete Asset",
                "Check Out Asset",
                "Return Asset",
                "Maintenance Report",
                "Exit"
            };

            int selectedIndex = 0;
            bool running = true;


            while (running)
            {
                DisplayDashboard(menuOptions, selectedIndex);



                bool menuActive = true;

                while (menuActive && running)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;

                    int previousIndex = selectedIndex;

                    if (key == ConsoleKey.UpArrow)
                    {
                        selectedIndex--;

                        if (selectedIndex < 0)
                            selectedIndex = menuOptions.Length - 1;
                    }
                    else if (key == ConsoleKey.DownArrow)
                    {
                        selectedIndex++;

                        if (selectedIndex >= menuOptions.Length)
                            selectedIndex = 0;
                    }
                    else if (key == ConsoleKey.Enter)
                    {
                        menuActive = false;

                        switch (selectedIndex)
                        {
                            case 0: AddAsset(); break;
                            case 1: ViewAssets(); break;
                            case 2: SearchAsset(); break;
                            case 3: FilterAssets(); break;
                            case 4: UpdateAsset(); break;
                            case 5: DeleteAsset(); break;
                            case 6: CheckOutAsset(); break;
                            case 7: ReturnAsset(); break;
                            case 8: MaintenanceReport(); break;
                            case 9: running = false; break;
                        }
                    }

                    if (menuActive && previousIndex != selectedIndex)
                    {
                        DrawMainMenuOption(
                            menuOptions[previousIndex],
                            previousIndex,
                            false);

                        DrawMainMenuOption(
                            menuOptions[selectedIndex],
                            selectedIndex,
                            true);
                    }
                }
            }


        }

        

        // ================= DASHBOARD =================

        static void DisplayDashboard(string[] options, int selectedIndex)
        {
            Console.Clear();

            int available = 0;
            int borrowed = 0;
            int repair = 0;

            foreach (CampusAsset asset in assets)
            {
                if (asset.Status == "Available")
                    available++;
                else if (asset.Status == "Borrowed")
                    borrowed++;
                else if (asset.Status == "In Repair")
                    repair++;
            }

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("                   CAMPUS ASSET IT TRACKER                 ");
           
            Console.WriteLine("            DBTC INFORMATION TECHNOLOGY DEPARTMENT");
            Console.ResetColor();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" ┌─────────────────────────────────────────────────────────┐");
            Console.WriteLine(" │                  [ SYSTEM OVERVIEW ]                    │");
            Console.WriteLine(" └─────────────────────────────────────────────────────────┘");
            Console.ResetColor();

            Console.WriteLine();

            Console.Write("   TOTAL ASSETS       : ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(assets.Count);
            Console.ResetColor();

            Console.Write("   AVAILABLE ASSETS   : ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(available);
            Console.ResetColor();

            Console.Write("   BORROWED ASSETS    : ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(borrowed);
            Console.ResetColor();

            Console.Write("   UNDER REPAIR       : ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(repair);
            Console.ResetColor();

            Console.Write("   SYSTEM STATUS      : ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("ONLINE");
            Console.ResetColor();
            
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan; // ─ ┤├┌┐└┘│
            Console.WriteLine(" ┌─────────────────────────────────────────────────────────┐");
            Console.WriteLine(" │                  [ MAIN NAVIGATION ]                    │");
            Console.WriteLine(" └─────────────────────────────────────────────────────────┘");
            Console.ResetColor();

            Console.WriteLine();

            mainMenuTop = Console.CursorTop;
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine("       >>  " + options[i].PadRight(42));

                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("           " + options[i]);
                    Console.ResetColor();
                }
            }

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ──────────────────────────────────────────────────────────");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("       UP / DOWN : Navigate       ENTER : Select");
            Console.ResetColor();

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ==========================================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("                DBTC IT ASSET TRACKER");
            Console.ResetColor();
        }

        static void DrawMainMenuOption(
            string option,
            int index,
            bool selected)
        {
            Console.SetCursorPosition(0, mainMenuTop + index);

            if (selected)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write("       >>  " + option.PadRight(42));
            }
            else
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;

                Console.Write("           " + option.PadRight(42));
            }

            Console.ResetColor();
        }


        // ================= ADD ASSET =================


        static void AddAsset()
        {
            Header("ADD NEW ASSET");

            Console.WriteLine("  Select Asset Type:");
            Console.WriteLine();

            string[] types =
            {
        "Workstation",
        "Networking Hardware",
        "Peripheral",
        "Software License",
        "Cancel"
    };

            int choice = SelectMenu(types);

            if (choice == 4)
                return;

            string tag;

            while (true)
            {
                tag = ReadRequiredText("\nAsset Tag (e.g. WS-001): ");

                if (AssetValidator.IsValidAssetTag(tag))
                    break;

                ErrorMessage(
                    "Invalid Asset Tag! Use format WS-001 (2 uppercase letters, hyphen, 3 digits).");
            }

            if (FindAsset(tag) != null)
            {
                ErrorMessage("An asset with this tag already exists.");
                return;
            }

            decimal price = ReadPositiveDecimal("Purchase Price: ");

            CampusAsset? newAsset = null;

            switch (choice)
            {
                case 0:
                    newAsset = new Workstation(tag, price);
                    break;

                case 1:
                    string ip;

                    while (true)
                    {
                        ip = ReadRequiredText("IP Address: ");

                        if (AssetValidator.IsValidIPAddress(ip))
                            break;

                        ErrorMessage(
                            "Invalid IP! Use a valid IPv4 address (e.g. 192.168.1.1).");
                    }

                    string mac;

                    while (true)
                    {
                        mac = ReadRequiredText("MAC Address: ");

                        if (AssetValidator.IsValidMacAddress(mac))
                            break;

                        ErrorMessage(
                            "Invalid MAC! Use format 00:1A:2B:3C:4D:5E.");
                    }

                    newAsset = new NetworkingHardware(
                        tag, price, ip, mac);
                    break;

                case 2:
                    newAsset = new Peripheral(tag, price);
                    break;

                case 3:
                    int months = ReadNonNegativeInt(
                        "Subscription Months Remaining: ");

                    newAsset = new SoftwareLicense(
                        tag, price, months);
                    break;
            }

            if (newAsset != null)
            {
                assets.Add(newAsset);
                SaveAssets();
                SuccessMessage("Asset successfully registered!");
            }
        }


        // ================= VIEW ASSETS =================

        static void ViewAssets()
        {
            Header("REGISTERED ASSETS");

            if (assets.Count == 0)
            {
                WarningMessage("No assets have been registered yet.");
                return;
            }

            Console.WriteLine(
                "  {0,-15} {1,-25} {2,-15} {3,-15}",
                "ASSET TAG", "ASSET TYPE", "PRICE", "STATUS");

            Console.WriteLine(
                "  ---------------------------------------------------------------");

            foreach (CampusAsset asset in assets)
            {
                Console.WriteLine(
                    "  {0,-15} {1,-25} {2,-15:C} {3,-15}",
                    asset.AssetTag,
                    asset.GetType().Name,
                    asset.PurchasePrice,
                    asset.Status);
            }

            Console.WriteLine();
            Console.WriteLine("  TOTAL REGISTERED ASSETS: " + assets.Count);
            Pause();
        }

        // ================= SEARCH ASSET =================

        static void SearchAsset()
        {
            Header("SEARCH ASSET");

            string tag = ReadRequiredText("Enter Asset Tag: ");

            CampusAsset? asset = FindAsset(tag);

            if (asset == null)
            {
                ErrorMessage("Asset not found.");
                return;
            }

            DisplayAssetDetails(asset);
            Pause();
        }

        // FILTHER ASSETSEQEQWEQWE


        static void FilterAssets()
        {
            Header("ASSET FILTERING");

            string[] filters =
            {
        "Show All Assets",
        "Available Assets",
        "Borrowed Assets",
        "Assets Under Repair",
        "Borrowable Assets",
        "Borrowed Items Only (IBorrowable)",
        "Networkable Assets (INetworkable)",
        "Cancel"
    };

            int choice = SelectMenu(filters);

            if (choice == 7)
                return;

            List<CampusAsset> filteredAssets = new List<CampusAsset>();

            switch (choice)
            {
                case 0:
                    filteredAssets = assets.ToList();
                    break;

                case 1:
                    filteredAssets = assets
                        .Where(a => a.Status == "Available")
                        .ToList();
                    break;

                case 2:
                    filteredAssets = assets
                        .Where(a => a.Status == "Borrowed")
                        .ToList();
                    break;

                case 3:
                    filteredAssets = assets
                        .Where(a => a.Status == "In Repair")
                        .ToList();
                    break;

                case 4:
                    filteredAssets = assets
                        .Where(a => a is IBorrowable)
                        .ToList();
                    break;

                case 5:
                    filteredAssets = assets
                        .Where(a => a is IBorrowable &&
                                    a.Status == "Borrowed")
                        .ToList();
                    break;

                case 6:
                    filteredAssets = assets
                        .Where(a => a is INetworkable)
                        .ToList();
                    break;
            }

            Console.Clear();
            Header("FILTER RESULTS");

            if (filteredAssets.Count == 0)
            {
                WarningMessage("No matching assets were found.");
                return;
            }

            Console.WriteLine(
                "  {0,-15} {1,-25} {2,-15} {3,-15}",
                "ASSET TAG", "ASSET TYPE", "PRICE", "STATUS");

            Console.WriteLine(
                "  ---------------------------------------------------------------");

            foreach (CampusAsset asset in filteredAssets)
            {
                Console.WriteLine(
                    "  {0,-15} {1,-25} {2,-15:C} {3,-15}",
                    asset.AssetTag,
                    asset.GetType().Name,
                    asset.PurchasePrice,
                    asset.Status);
            }

            Console.WriteLine();
            Console.WriteLine("  MATCHING ASSETS: " + filteredAssets.Count);

            Pause();
        }


        // ================= UPDATE ASSET =================

        static void UpdateAsset()
        {
            Header("UPDATE ASSET");

            string tag = ReadRequiredText("Enter Asset Tag: ");

            CampusAsset? asset = FindAsset(tag);
            SaveAssets();

            if (asset == null)
            {
                ErrorMessage("Asset not found.");
                return;
            }

            if (asset.Status == "Borrowed")
            {
                WarningMessage(
                    "Return this asset before updating its condition.");
                return;
            }

            string[] options =
            {
                "Update Purchase Price",
                "Mark as In Repair",
                "Mark as Available",
                "Cancel"
            };

            int choice = SelectMenu(options);

            try
            {
                switch (choice)
                {
                    case 0:
                        decimal newPrice =
                            ReadPositiveDecimal("Enter New Price: ");

                        asset.UpdatePurchasePrice(newPrice);
                        SaveAssets();

                        SuccessMessage("Purchase price updated!");
                        SaveAssets();
                        break;

                    case 1:
                        asset.MarkForRepair();

                        SuccessMessage("Asset marked as In Repair.");
                        SaveAssets();
                        break;

                    case 2:
                        asset.MarkAvailable();

                        SuccessMessage("Asset marked as Available.");
                        SaveAssets();
                        break;

                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex.Message);
            }
        }

        // ================= DELETE ASSET =================

        static void DeleteAsset()
        {
            Header("DELETE ASSET");

            string tag = ReadRequiredText("Enter Asset Tag: ");

            CampusAsset? asset = FindAsset(tag);

            if (asset == null)
            {
                ErrorMessage("Asset not found.");
                return;
            }

            if (asset.Status == "Borrowed")
            {
                WarningMessage("Cannot delete an asset that is borrowed.");
                return;
            }

            Console.WriteLine();
            DisplayAssetDetails(asset);


            Console.WriteLine();
            Console.WriteLine("  Are you sure you want to delete this asset?:");
            

            string[] confirmationOptions = { "YES", "NO" };

            int confirmation = SelectMenu(confirmationOptions);

            if (confirmation == 0)
            {
                assets.Remove(asset);
                SaveAssets();
                SuccessMessage("Asset successfully deleted.");
            }
            else
            {
                WarningMessage("Delete operation cancelled.");
            }

        }

        // ================= CHECK OUT =================

        static void CheckOutAsset()
        {
            Header("CHECK OUT ASSET");

            string tag = ReadRequiredText("Enter Asset Tag: ");

            CampusAsset? asset = FindAsset(tag);

            if (asset == null)
            {
                ErrorMessage("Asset not found.");
                return;
            }

            if (!(asset is IBorrowable borrowable))
            {
                WarningMessage("This asset cannot be borrowed.");
                return;
            }

            string borrowerId = ReadRequiredText("Borrower ID (e.g. 2026-12345): ");

            try
            {
                borrowable.CheckOut(borrowerId);
                SaveAssets();

                SuccessMessage("Asset successfully checked out!");
            }
            catch (Exception ex)
            {
                ErrorMessage(ex.Message);
            }
        }

        // ================= RETURN ASSET =================

        static void ReturnAsset()
        {
            Header("RETURN ASSET");

            string tag = ReadRequiredText("Enter Asset Tag: ");

            CampusAsset? asset = FindAsset(tag);

            if (asset == null)
            {
                ErrorMessage("Asset not found.");
                return;
            }

            if (!(asset is IBorrowable borrowable))
            {
                WarningMessage("This asset does not support borrowing.");
                return;
            }

            try
            {
                borrowable.ReturnAsset();
                SaveAssets();

                SuccessMessage("Asset successfully returned!");
            }
            catch (Exception ex)
            {
                ErrorMessage(ex.Message);
            }
        }

        // ================= ASSET DETAILS =================

        static void DisplayAssetDetails(CampusAsset asset)
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  [ ASSET INFORMATION ]");
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("  Asset Type       : " + asset.GetType().Name);
            Console.WriteLine("  Asset Tag        : " + asset.AssetTag);
            Console.WriteLine("  Purchase Price   : " + asset.PurchasePrice.ToString("C"));
            Console.WriteLine("  Current Status   : " + asset.Status);
            Console.WriteLine("  Maintenance      : " + asset.GetMaintenanceStatus());

            if (asset is NetworkingHardware network)
            {
                Console.WriteLine("  IP Address       : " + network.IPAddress);
                Console.WriteLine("  MAC Address      : " + network.MACAddress);
            }

            if (asset is SoftwareLicense software)
            {
                Console.WriteLine(
                    "  Months Remaining : " +
                    software.SubscriptionMonthsRemaining);
            }

            Console.WriteLine();
            Console.WriteLine("  " + asset.GetDetails());
        }

        // ================= FIND ASSET =================

        static CampusAsset? FindAsset(string tag)
        {
            foreach (CampusAsset asset in assets)
            {
                if (asset.AssetTag.Equals(
                    tag, StringComparison.OrdinalIgnoreCase))
                {
                    return asset;
                }
            }

            return null;
        }

        // ============= MAINTENANCE REPORTZZZ =============


        static void MaintenanceReport()
        {
            Header("MAINTENANCE REPORT");

            if (assets.Count == 0)
            {
                WarningMessage("No registered assets found.");
                return;
            }

            foreach (CampusAsset asset in assets)
            {
                Console.WriteLine("  Asset Tag : " + asset.AssetTag);
                Console.WriteLine("  Type      : " + asset.GetType().Name);

                Console.Write("  Condition : ");

                string maintenance = asset.GetMaintenanceStatus();

                if (maintenance.Contains("repair") ||
                    maintenance.Contains("expired"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (maintenance.Contains("soon"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.WriteLine(maintenance);
                Console.ResetColor();

                Console.WriteLine(
                    "  --------------------------------------------------");
            }

            Pause();
        }


        // ================= MENU SELECTOR =================


        static int SelectMenu(string[] options)
        {
            int selectedIndex = 0;
            int menuTop = Console.CursorTop;

            while (true)
            {
                // Redraw each option in the same position
                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition(0, menuTop + i);

                    // Clear the current line
                    Console.Write(new string(' ', Console.WindowWidth - 1));

                    Console.SetCursorPosition(0, menuTop + i);

                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;

                        Console.Write("  >> " + options[i]);

                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;

                        Console.Write("     " + options[i]);

                        Console.ResetColor();
                    }
                }

                // Read keyboard input
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                {
                    selectedIndex--;

                    if (selectedIndex < 0)
                        selectedIndex = options.Length - 1;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    selectedIndex++;

                    if (selectedIndex >= options.Length)
                        selectedIndex = 0;
                }
                else if (key == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(0, menuTop + options.Length);

                    return selectedIndex;
                }
            }
        }


        // ================= INPUT VALIDATION =================

        static string ReadRequiredText(string prompt)
        {
            while (true)
            {
                Console.Write("  " + prompt);

                string? value = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(value))  
                    return value.Trim();

                ErrorMessage("This field cannot be empty.");
            }
        }

        static decimal ReadPositiveDecimal(string prompt)
        {
            while (true)
            {
                Console.Write("  " + prompt);

                if (decimal.TryParse(Console.ReadLine(), out decimal value)
                    && value > 0)
                {
                    return value;
                }

                ErrorMessage("Enter a valid amount greater than zero.");
            }
        }

        static int ReadNonNegativeInt(string prompt)
        {
            while (true)
            {
                Console.Write("  " + prompt);

                if (int.TryParse(Console.ReadLine(), out int value)
                    && value >= 0)
                {
                    return value;
                }

                ErrorMessage("Enter a valid number (zero or higher).");
            }
        }

        // ================= SCREEN NGANIII =================

        static void Header(string title)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine(" ┌─────────────────────────────────────────────────────────┐");
            Console.WriteLine(" │            CAMPUS IT ASSET MANAGEMENT SYSTEM            │");
            Console.WriteLine(" └─────────────────────────────────────────────────────────┘");
            Console.ResetColor();

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                      [ " + title + " ]");
            Console.ResetColor();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" ───────────────────────────────────────────────────────────");
            Console.WriteLine();
        }

        static void SuccessMessage(string message)
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [ SUCCESS ] " + message);
            Console.ResetColor();

            Pause();
        }

        static void ErrorMessage(string message)
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  [ ERROR ] " + message);
            Console.ResetColor();

            Pause();
        }

        static void WarningMessage(string message)
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [ NOTICE ] " + message);
            Console.ResetColor();

            Pause();
        }

        static void Pause()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  Press any key to continue...");
            Console.ResetColor();

            Console.ReadKey(true);
        }

        // ================= EXIT SCREEN =================

        static void DisplayExitScreen()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine(" ┌─────────────────────────────────────────────────────────┐");
            Console.WriteLine(" │            CAMPUS IT ASSET MANAGEMENT SYSTEM            │");
            Console.WriteLine(" └─────────────────────────────────────────────────────────┘");
            Console.ResetColor();

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 SYSTEM SHUTDOWN SUCCESSFUL");
            Console.WriteLine("                    Thank you for using");
            Console.WriteLine("                  Campus IT Asset Tracker!");
            Console.ResetColor();

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("                    Application Closed.");
            Console.ResetColor();

            Console.WriteLine();
        }
        static void SaveAssets()
        {
            try
            {
                string json = JsonSerializer.Serialize(assets, jsonOptions);

                File.WriteAllText(filePath, json);

                Console.WriteLine("Assets saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving assets: {ex.Message}");
            }
        }
        static void LoadAssets()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);

                    assets = JsonSerializer.Deserialize<List<CampusAsset>>(
                        json, jsonOptions) ?? new List<CampusAsset>();

                    Console.WriteLine("Assets loaded successfully!");
                }
                else
                {
                    assets = new List<CampusAsset>();

                    Console.WriteLine("No saved asset file found. Starting fresh.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading assets: {ex.Message}");
                assets = new List<CampusAsset>();
            }
        }
    }

}
