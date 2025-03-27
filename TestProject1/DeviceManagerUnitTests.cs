namespace TestProject1;

using Xunit;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public class DeviceManagerUnitTests
    {
        private string _testFilePath = "test_devices_xunit.txt"; // Temporary file for tests

    // Helper method to clean up the test file after each test
    private void CleanupTestFile()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    // Runs after each test method
    public DeviceManagerUnitTests()
    {
        CleanupTestFile();
    }

    // Runs before each test method (optional in this case as cleanup is enough)
    // ~DeviceManagerUnitTests()
    // {
    //     CleanupTestFile();
    // }


    [Fact]
    public void AddDevice_DeviceAddedSuccessfully()
    {
        // Arrange
        File.WriteAllText(_testFilePath, ""); // Start with an empty file
        DeviceManager manager = new DeviceManager(_testFilePath);
        Smartwatch smartwatch = new Smartwatch("SW-1", "TestWatch", false, 80); // Changed ID to string

        // Act
        manager.AddDevice(smartwatch);

        // Assert
        Assert.Equal(1, manager.GetDeviceCount());
    }

    [Fact]
    public void AddDevice_StorageIsFull_ThrowsException()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "");
        DeviceManager manager = new DeviceManager(_testFilePath);
        for (int i = 1; i <= 15; i++)
        {
            manager.AddDevice(new Smartwatch($"SW-{i}", $"TestWatch{i}", false, 80)); // Changed ID to string
        }

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => manager.AddDevice(new Smartwatch("SW-16", "TooMany", false, 80))); // Changed ID to string
    }

    [Fact]
    public void RemoveDevice_ExistingDeviceRemoved()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,false,80%"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.RemoveDevice("SW-1"); // Changed ID to string

        // Assert
        Assert.Equal(0, manager.GetDeviceCount());
    }

    [Fact]
    public void RemoveDevice_NonExistingDevice_DoesNothing()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,false,80%"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);
        int initialCount = manager.GetDeviceCount();

        // Act
        manager.RemoveDevice("SW-999"); // Non-existent ID as string

        // Assert
        Assert.Equal(initialCount, manager.GetDeviceCount()); // Count should remain the same
    }

    [Fact]
    public void TurnOnDevice_ExistingDevice_Smartwatch_SufficientBattery()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,false,25%"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.TurnOnDevice("SW-1"); // Changed ID to string

        // Assert
        var device = manager.GetDeviceById("SW-1") as Smartwatch; // Changed ID to string, using extension method
        Assert.NotNull(device);
        Assert.True(device.IsTurnedOn);
        Assert.Equal(15, device.BatteryPercentage); // Battery should decrease by 10
    }


    [Fact]
    public void TurnOnDevice_ExistingDevice_PC_WithOS()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "P-1,TestPC,false,Windows"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.TurnOnDevice("P-1"); // Changed ID to string

        // Assert
        var device = manager.GetDeviceById("P-1") as PersonalComputer; // Changed ID to string, using extension method
        Assert.NotNull(device);
        Assert.True(device.IsTurnedOn);
    }


    [Fact]
    public void TurnOnDevice_ExistingDevice_EmbeddedDevice_ConnectsSuccessfully()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "ED-1,TestED,False,192.168.1.10,MD Ltd.Wifi"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.TurnOnDevice("ED-1"); // Changed ID to string

        // Assert
        var device = manager.GetDeviceById("ED-1") as EmbeddedDevice; // Changed ID to string, using extension method
        Assert.NotNull(device);
        Assert.True(device.IsTurnedOn);
    }

    [Fact]
    public void TurnOnDevice_ExistingDevice_EmbeddedDevice_ConnectionFails()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "ED-1,TestED,False,192.168.1.10,HomeWifi"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.TurnOnDevice("ED-1"); // Changed ID to string // ConnectionException is caught and message printed, but no exception propagates

        // Assert
        var device = manager.GetDeviceById("ED-1") as EmbeddedDevice; // Changed ID to string, using extension method
        Assert.NotNull(device);
        Assert.False(device.IsTurnedOn); // Should NOT be turned on because of connection failure logic in TurnOn method
        // In current implementation, TurnOn catches ConnectionException and prints to console, but doesn't prevent IsTurnedOn to be set to true. Fix in code if needed for stricter behaviour.
    }

    [Fact]
    public void TurnOnDevice_NonExistingDevice_DoesNothing()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "");
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.TurnOnDevice("SW-999"); // Non-existent ID as string

        // Assert
        // No exception should be thrown, nothing should happen
    }

    [Fact]
    public void TurnOffDevice_ExistingDevice_TurnsOff()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,true,80%"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.TurnOffDevice("SW-1"); // Changed ID to string

        // Assert
        var device = manager.GetDeviceById("SW-1") as Smartwatch; // Changed ID to string, using extension method
        Assert.NotNull(device);
        Assert.False(device.IsTurnedOn);
    }

    [Fact]
    public void TurnOffDevice_NonExistingDevice_DoesNothing()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "");
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.TurnOffDevice("SW-999"); // Non-existent ID as string

        // Assert
        // No exception should be thrown, nothing should happen
    }

    [Fact]
    public void GetDeviceCount_ReturnsCorrectCount()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "SW-1,Watch1,false,80%\nP-1,PC1,false,Windows"); // IDs in file are string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act & Assert
        Assert.Equal(2, manager.GetDeviceCount());
    }





    [Fact]
    public void LoadFromFile_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        string nonExistentFilePath = "non_existent_file.txt";

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => new DeviceManager(nonExistentFilePath));
    }

    [Fact]
    public void LoadFromFile_ExceedsMaxDevices_StopsLoading()
    {
        // Arrange
        string fileContent = "";
        for (int i = 1; i <= 20; i++) // Create file with 20 devices
        {
            fileContent += $"SW-{i},Watch{i},false,80%\n"; // IDs in file are string already
        }
        File.WriteAllText(_testFilePath, fileContent);

        // Act
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Assert
        Assert.Equal(15, manager.GetDeviceCount()); // Should load only up to MaxDevices (15)
    }


    [Fact]
    public void EditDevice_EditSmartwatchName()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "SW-1,OldName,false,80%"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.EditDevice("SW-1", "Name", "NewSmartwatchName"); // Changed ID to string

        // Assert
        var device = manager.GetDeviceById("SW-1") as Smartwatch; // Changed ID to string, using extension method
        Assert.NotNull(device);
        Assert.Equal("NewSmartwatchName", device.Name);
    }

    [Fact]
    public void EditDevice_EditPC_OS()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "P-1,TestPC,false,null"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.EditDevice("P-1", "OperatingSystem", "MacOS"); // Changed ID to string

        // Assert
        var device = manager.GetDeviceById("P-1") as PersonalComputer; // Changed ID to string, using extension method
        Assert.NotNull(device);
        Assert.Equal("MacOS", device.OperatingSystem);
    }

    [Fact]
    public void EditDevice_EditEmbeddedDevice_NetworkName()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "ED-1,TestED,False,192.168.1.1,OldNetwork"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act
        manager.EditDevice("ED-1", "NetworkName", "NewNetworkName"); // Changed ID to string

        // Assert
        var device = manager.GetDeviceById("ED-1") as EmbeddedDevice; // Changed ID to string, using extension method
        Assert.NotNull(device);
        Assert.Equal("NewNetworkName", device.NetworkName);
    }

    [Fact]
    public void EditDevice_NonExistingDevice_ThrowsArgumentException()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "");
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => manager.EditDevice("SW-999", "Name", "NewName")); // Changed ID to string
    }

    [Fact]
    public void EditDevice_NonExistingProperty_ThrowsArgumentException()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,false,80%"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => manager.EditDevice("SW-1", "NonExistentProperty", "newValue")); // Changed ID to string
    }

    [Fact]
    public void EditDevice_InvalidPropertyValueType_ThrowsArgumentException()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,false,80%"); // ID in file is string already
        DeviceManager manager = new DeviceManager(_testFilePath);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => manager.EditDevice("SW-1", "BatteryPercentage", "invalid_value")); // Changed ID to string // BatteryPercentage is int, but passing string
    }
}