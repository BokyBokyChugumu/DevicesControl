namespace TestProject1;

using Xunit;
using System.IO;
using System;
using DevicesControl;
using System.Collections.Generic;
using System.Linq;

public class DeviceManagerUnitTests
    {
        private string _testFilePath = "test_devices_xunit.txt";

   
    private void CleanupTestFile()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    
    public DeviceManagerUnitTests()
    {
        CleanupTestFile();
    }

    


    [Fact]
    public void AddDevice_DeviceAddedSuccessfully()
    {
        
        File.WriteAllText(_testFilePath, ""); 
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());
        Smartwatch smartwatch = new Smartwatch("SW-1", "TestWatch", false, 80);

        
        manager.AddDevice(smartwatch);

        
        Assert.Equal(1, manager.GetDeviceCount());
    }

    [Fact]
    public void AddDevice_StorageIsFull_ThrowsException()
    {
        
        File.WriteAllText(_testFilePath, "");
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());
        for (int i = 1; i <= 15; i++)
        {
            manager.AddDevice(new Smartwatch($"SW-{i}", $"TestWatch{i}", false, 80));
        }

        
        Assert.Throws<InvalidOperationException>(() => manager.AddDevice(new Smartwatch("SW-16", "TooMany", false, 80))); 
    }

    [Fact]
    public void RemoveDevice_ExistingDeviceRemoved()
    {
        
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,false,80%"); 
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        manager.RemoveDevice("SW-1");

        
        Assert.Equal(0, manager.GetDeviceCount());
    }

    [Fact]
    public void RemoveDevice_NonExistingDevice_DoesNothing()
    {
       
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,false,80%"); 
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());
        int initialCount = manager.GetDeviceCount();

      
        manager.RemoveDevice("SW-999");
       
        Assert.Equal(initialCount, manager.GetDeviceCount()); 
    }

    [Fact]
    public void TurnOnDevice_ExistingDevice_Smartwatch_SufficientBattery()
    {
       
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,false,25%"); 
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        manager.TurnOnDevice("SW-1"); 

       
        var device = manager.GetDeviceById("SW-1") as Smartwatch;
        Assert.NotNull(device);
        Assert.True(device.IsTurnedOn);
        Assert.Equal(15, device.BatteryPercentage); 
    }


    [Fact]
    public void TurnOnDevice_ExistingDevice_PC_WithOS()
    {
        
        File.WriteAllText(_testFilePath, "P-1,TestPC,false,Windows"); 
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

       
        manager.TurnOnDevice("P-1");

       
        var device = manager.GetDeviceById("P-1") as PersonalComputer; 
        Assert.NotNull(device);
        Assert.True(device.IsTurnedOn);
    }


    [Fact]
    public void TurnOnDevice_ExistingDevice_EmbeddedDevice_ConnectsSuccessfully()
    {
      
        File.WriteAllText(_testFilePath, "ED-1,TestED,False,192.168.1.10,MD Ltd.Wifi");
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

       
        manager.TurnOnDevice("ED-1");

       
        var device = manager.GetDeviceById("ED-1") as EmbeddedDevice;
        Assert.NotNull(device);
        Assert.True(device.IsTurnedOn);
    }

    [Fact]
    public void TurnOnDevice_ExistingDevice_EmbeddedDevice_ConnectionFails()
    {
        
        File.WriteAllText(_testFilePath, "ED-1,TestED,False,192.168.1.10,HomeWifi"); 
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        manager.TurnOnDevice("ED-1");

        
        var device = manager.GetDeviceById("ED-1") as EmbeddedDevice; 
        Assert.NotNull(device);
        Assert.False(device.IsTurnedOn); 
        
    }

    [Fact]
    public void TurnOnDevice_NonExistingDevice_DoesNothing()
    {
        
        File.WriteAllText(_testFilePath, "");
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        manager.TurnOnDevice("SW-999");

        
    }

    [Fact]
    public void TurnOffDevice_ExistingDevice_TurnsOff()
    {
        
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,true,80%"); 
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        manager.TurnOffDevice("SW-1"); 

        
        var device = manager.GetDeviceById("SW-1") as Smartwatch; 
        Assert.NotNull(device);
        Assert.False(device.IsTurnedOn);
    }

    [Fact]
    public void TurnOffDevice_NonExistingDevice_DoesNothing()
    {
        
        File.WriteAllText(_testFilePath, "");
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        manager.TurnOffDevice("SW-999"); 

        
    }

    [Fact]
    public void GetDeviceCount_ReturnsCorrectCount()
    {
        
        File.WriteAllText(_testFilePath, "SW-1,Watch1,false,80%\nP-1,PC1,false,Windows"); // IDs in file are string already
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        Assert.Equal(2, manager.GetDeviceCount());
    }





    [Fact]
    public void LoadFromFile_NonExistentFile_ThrowsFileNotFoundException()
    {
        
        string nonExistentFilePath = "non_existent_file.txt";

        
        Assert.Throws<FileNotFoundException>(() => DeviceManager.LoadFromFile(nonExistentFilePath, new FileDeviceFactory()));
    }

    [Fact]
    public void LoadFromFile_ExceedsMaxDevices_StopsLoading()
    {
        
        string fileContent = "";
        for (int i = 1; i <= 20; i++) 
        {
            fileContent += $"SW-{i},Watch{i},false,80%\n"; 
        }
        File.WriteAllText(_testFilePath, fileContent);

        
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        Assert.Equal(15, manager.GetDeviceCount()); 
    }


    [Fact]
    public void EditDevice_EditSmartwatchName()
    {
        
        File.WriteAllText(_testFilePath, "SW-1,OldName,false,80%");
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        manager.EditDevice("SW-1", "Name", "NewSmartwatchName");

        
        var device = manager.GetDeviceById("SW-1") as Smartwatch;
        Assert.NotNull(device);
        Assert.Equal("NewSmartwatchName", device.Name);
    }

    [Fact]
    public void EditDevice_EditPC_OS()
    {
        
        File.WriteAllText(_testFilePath, "P-1,TestPC,false,null");
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        manager.EditDevice("P-1", "OperatingSystem", "MacOS"); 

       
        var device = manager.GetDeviceById("P-1") as PersonalComputer; 
        Assert.NotNull(device);
        Assert.Equal("MacOS", device.OperatingSystem);
    }

    [Fact]
    public void EditDevice_EditEmbeddedDevice_NetworkName()
    {
        
        File.WriteAllText(_testFilePath, "ED-1,TestED,False,192.168.1.1,OldNetwork");
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        manager.EditDevice("ED-1", "NetworkName", "NewNetworkName"); 

        
        var device = manager.GetDeviceById("ED-1") as EmbeddedDevice; 
        Assert.NotNull(device);
        Assert.Equal("NewNetworkName", device.NetworkName);
    }

    [Fact]
    public void EditDevice_NonExistingDevice_ThrowsArgumentException()
    {
       
        File.WriteAllText(_testFilePath, "");
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        Assert.Throws<ArgumentException>(() => manager.EditDevice("SW-999", "Name", "NewName")); 
    }

    [Fact]
    public void EditDevice_NonExistingProperty_ThrowsArgumentException()
    {
        
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,false,80%"); 
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        Assert.Throws<ArgumentException>(() => manager.EditDevice("SW-1", "NonExistentProperty", "newValue"));
    }

    [Fact]
    public void EditDevice_InvalidPropertyValueType_ThrowsArgumentException()
    {
        
        File.WriteAllText(_testFilePath, "SW-1,TestWatch,false,80%"); 
        DeviceManager manager = DeviceManager.LoadFromFile(_testFilePath, new FileDeviceFactory());

        
        Assert.Throws<ArgumentException>(() => manager.EditDevice("SW-1", "BatteryPercentage", "invalid_value")); 
    }
}