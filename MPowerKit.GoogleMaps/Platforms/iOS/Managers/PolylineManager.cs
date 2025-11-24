using Foundation;

using Google.Maps;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using UIKit;

using NPolyline = Google.Maps.Polyline;
using VPolyline = Microsoft.Maui.Controls.Shapes.Polyline;

namespace MPowerKit.GoogleMaps;

public class PolylineManager : ItemsMapFeatureManager<VPolyline, NPolyline, MapView>
{
    protected override IEnumerable<VPolyline>? VirtualViewItems => VirtualView!.Polylines;
    protected override string VirtualViewItemsPropertyName => GoogleMap.PolylinesProperty.PropertyName;

    protected override void SubscribeToEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        platformView.OverlayTapped += PlatformView_OverlayTapped;
        platformView.CameraPositionChanged += PlatformView_CameraPositionChanged;
    }

    protected override void UnsubscribeFromEvents(GoogleMap virtualView, MapView platformView, GoogleMapHandler handler)
    {
        platformView.OverlayTapped -= PlatformView_OverlayTapped;
        platformView.CameraPositionChanged -= PlatformView_CameraPositionChanged;

        base.UnsubscribeFromEvents(virtualView, platformView, handler);
    }

    protected override void RemoveItemFromPlatformView(NPolyline? nItem)
    {
        if (nItem is not null)
        {
            nItem.Map = null;
        }
    }

    protected override NPolyline AddItemToPlatformView(VPolyline vItem)
    {
        return vItem.ToNative(PlatformView!, PolylineAttached.GetiOSPixelDependentDashedPattern(vItem));
    }

    protected override void ItemPropertyChanged(VPolyline vItem, NPolyline nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (propertyName == Shape.IsEnabledProperty.PropertyName)
        {
            OnIsEnabledChanged(vItem, nItem);
        }
        else if (propertyName == Shape.IsVisibleProperty.PropertyName)
        {
            OnIsVisibleChanged(vItem, nItem);
        }
        else if (propertyName == Shape.ZIndexProperty.PropertyName)
        {
            OnZIndexChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeThicknessProperty.PropertyName)
        {
            OnStrokeThicknessChanged(vItem, nItem);
        }
        else if (propertyName == VPolyline.PointsProperty.PropertyName)
        {
            OnPointsChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeDashArrayProperty.PropertyName)
        {
            OnStrokeDashArrayChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeProperty.PropertyName)
        {
            OnStrokeChanged(vItem, nItem);
        }
        else if (propertyName == PolylineAttached.iOSPixelDependentDashedPatternProperty.PropertyName)
        {
            OnPixelDependentDashedPatternChanged(vItem, nItem);
        }
        else if (propertyName == PolylineAttached.TextureStampProperty.PropertyName)
        {
            OnTextureStampChanged(vItem, nItem);
        }
    }

    protected virtual void OnIsEnabledChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.Tappable = vPolyline.IsEnabled;
    }

    protected virtual void OnIsVisibleChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.Map = vPolyline.IsVisible ? PlatformView! : null;
    }

    protected virtual void OnZIndexChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.ZIndex = vPolyline.ZIndex;
    }

    protected virtual void OnStrokeThicknessChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.StrokeWidth = (float)vPolyline.StrokeThickness;
    }

    protected virtual void OnPointsChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.Path = vPolyline.Points.ToPath();
        OnStrokeChanged(vPolyline, nPolyline);
    }

    protected virtual void OnStrokeDashArrayChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        OnStrokeChanged(vPolyline, nPolyline);
    }

    protected virtual void OnStrokeChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.Spans = vPolyline.ToSpans(PlatformView!, PolylineAttached.GetiOSPixelDependentDashedPattern(vPolyline));
    }

    protected virtual void OnPixelDependentDashedPatternChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        OnStrokeChanged(vPolyline, nPolyline);
    }

    protected virtual void OnTextureStampChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        OnStrokeChanged(vPolyline, nPolyline);
    }

    protected virtual void PlatformView_OverlayTapped(object? sender, GMSOverlayEventEventArgs e)
    {
        var polyline = Items.SingleOrDefault(p => NativeObjectAttachedProperty.GetNativeObject(p) == e.Overlay);
        if (polyline is null) return;

        VirtualView!.SendPolylineClick(polyline);
    }

    protected virtual void PlatformView_CameraPositionChanged(object? sender, GMSCameraEventArgs e)
    {
        foreach (var polyline in Items.Where(p => p.StrokeDashPattern.Length != 0))
        {
            if (!PolylineAttached.GetiOSPixelDependentDashedPattern(polyline)) continue;

            if (NativeObjectAttachedProperty.GetNativeObject(polyline) is not NPolyline native) continue;

            native.Spans = polyline.ToSpans(PlatformView!, true);
        }
    }
}

public static class PolylineExtensions
{
    public static NPolyline ToNative(this VPolyline polyline, MapView map, bool pixelDependentDashedPattern)
    {
        var path = polyline.Points.ToPath();
        var native = NPolyline.FromPath(path);
        native.ZIndex = polyline.ZIndex;
        native.StrokeWidth = (float)polyline.StrokeThickness;
        native.Tappable = polyline.IsEnabled;
        native.Spans = polyline.ToSpans(map, pixelDependentDashedPattern);

        if (polyline.IsVisible)
        {
            native.Map = map;
        }

        return native;
    }

    public static StyleSpan[] ToSpans(this VPolyline polyline, MapView map, bool pixelDependentDashedPattern)
    {
        var path = polyline.Points.ToPath();
        var texture = PolylineAttached.GetTextureStamp(polyline);
        StyleSpan[] spans;
        if (polyline.StrokeDashPattern.Length != 0)
        {
            var colorStyle = polyline.Stroke.ToStrokeStyle(texture);
            var clearStyle = StrokeStyle.GetSolidColor(UIColor.Clear);
            var pattern = polyline.StrokeDashPattern;
            List<StrokeStyle> styles = [];
            for (int i = 0; i < pattern.Length; i++)
            {
                var value = pattern[i];

                styles.Add(i % 2 == 0
                    ? colorStyle
                    : clearStyle);
            }
            var metersPerPixel = pixelDependentDashedPattern ? Distance.MetersPerDevicePixel((polyline.Points[0].X + polyline.Points[^1].X) / 2d, map.Camera.Zoom) : 1d;
            spans = GeometryUtils.StyleSpans(path, [.. styles], [.. pattern.Select(v => new NSNumber(v * metersPerPixel))], LengthKind.Rhumb);
        }
        else
        {
            spans = [polyline.Stroke.ToSpan(texture)];
        }

        return spans;
    }

    public static StrokeStyle ToStrokeStyle(this Brush? brush, string? texture)
    {
        var style = brush switch
        {
            SolidColorBrush solidBrush => StrokeStyle.GetSolidColor(solidBrush.Color.ToPlatform()),
            LinearGradientBrush gradientBrush => gradientBrush.GradientStops.Count switch
            {
                0 => StrokeStyle.GetSolidColor(UIColor.Black),
                1 => StrokeStyle.GetSolidColor(gradientBrush.GradientStops[0].Color.ToPlatform()),
                _ => StrokeStyle.GetGradient(gradientBrush.GradientStops[0].Color.ToPlatform(),
                    gradientBrush.GradientStops[1].Color.ToPlatform())
            },
            _ => StrokeStyle.GetSolidColor(UIColor.Blue)
        };

        if (!string.IsNullOrWhiteSpace(texture))
        {
            var textureStyle = new TextureStyle(new UIImage(texture));
            //style.StampStyle = textureStyle;
            //ToDo: This is workaround. remove when property is added
            void_objc_msgSend_IntPtr(style.Handle, ObjCRuntime.Selector.GetHandle("setStampStyle:"), textureStyle.Handle);
        }

        return style;
    }

    [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
    private static extern void void_objc_msgSend_IntPtr(nint receiver, nint selector, nint arg0);

    public static StyleSpan ToSpan(this Brush? brush, string? texture)
    {
        return StyleSpan.FromStyle(brush.ToStrokeStyle(texture));
    }
}