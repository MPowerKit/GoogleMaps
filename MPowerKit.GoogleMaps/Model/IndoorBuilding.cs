namespace MPowerKit.GoogleMaps;

public class IndoorBuilding
{
    public int DefaultLevelIndex { get; set; }
    public bool IsUnderground { get; set; }
    public required IList<IndoorLevel> Levels { get; set; }
}

public class IndoorLevel
{
#if IOS
    public required Google.Maps.MapView NativeMap { get; set; }
#endif
    public required object NativeIndoorLevel { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }

    public void Activate()
    {
#if ANDROID
        (NativeIndoorLevel as Android.Gms.Maps.Model.IndoorLevel)?.Activate();
#elif IOS
        //NativeMap.IndoorDisplay.ActiveLevel = (NativeIndoorLevel as Google.Maps.IndoorLevel);
        //ToDo: This is workaround. remove when property is added
        var level = NativeIndoorLevel as Google.Maps.IndoorLevel;
        void_objc_msgSend_IntPtr(NativeMap.IndoorDisplay.Handle, ObjCRuntime.Selector.GetHandle("setActiveLevel:"), level.Handle);
#endif
    }

#if IOS
    [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    static extern void void_objc_msgSend_IntPtr(System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg0);
#endif
}