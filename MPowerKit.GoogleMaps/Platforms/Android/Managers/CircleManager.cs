using Android.Content;
using Android.Gms.Maps.Model;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using GMap = Android.Gms.Maps.GoogleMap;
using NCircle = Android.Gms.Maps.Model.Circle;
using VCircle = MPowerKit.GoogleMaps.Circle;

namespace MPowerKit.GoogleMaps;

public class CircleManager : ItemsMapFeatureManager<VCircle, NCircle, GoogleMap, GMap, GoogleMapHandler>
{
    protected override void SubscribeToEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        platformView.CircleClick += PlatformView_CircleClick;
    }

    protected override void UnsubscribeFromEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        platformView.CircleClick -= PlatformView_CircleClick;

        base.UnsubscribeFromEvents(virtualView, platformView, handler);
    }

    protected override string GetVirtualViewItemsPropertyName()
    {
        return GoogleMap.CirclesProperty.PropertyName;
    }

    protected override IEnumerable<VCircle> GetVirtualViewItems()
    {
        return VirtualView!.Circles;
    }

    protected override void RemoveItemFromPlatformView(NCircle? nItem)
    {
        nItem?.Remove();
    }

    protected override NCircle AddItemToPlatformView(VCircle vItem)
    {
        return PlatformView!.AddCircle(vItem.ToNative(Handler!.Context));
    }

    protected override void ItemPropertyChanged(VCircle vItem, NCircle nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (propertyName == Shape.IsEnabledProperty.PropertyName)
        {
            OnIsEnabledChanged(vItem, nItem);
        }
        else if (propertyName == VCircle.RadiusProperty.PropertyName)
        {
            OnRadiusChanged(vItem, nItem);
        }
        else if (propertyName == VCircle.CenterProperty.PropertyName)
        {
            OnCenterChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeDashArrayProperty.PropertyName)
        {
            OnStrokeDashArrayChanged(vItem, nItem);
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
        else if (propertyName == Shape.StrokeProperty.PropertyName)
        {
            OnStrokeChanged(vItem, nItem);
        }
        else if (propertyName == Shape.FillProperty.PropertyName)
        {
            OnFillChanged(vItem, nItem);
        }
    }

    protected virtual void OnIsEnabledChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.Clickable = vCircle.IsEnabled;
    }

    protected virtual void OnRadiusChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.Radius = vCircle.Radius;
    }

    protected virtual void OnCenterChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.Center = vCircle.Center.ToLatLng();
    }

    protected virtual void OnStrokeDashArrayChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.StrokePattern = vCircle.StrokeDashPattern.Length > 0
            ? vCircle.StrokeDashPattern.Select(v => Handler!.Context.ToPixels(v)).ToArray().ToPatternItems()
            : null;
    }

    protected virtual void OnIsVisibleChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.Visible = vCircle.IsVisible;
    }

    protected virtual void OnZIndexChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.ZIndex = vCircle.ZIndex;
    }

    protected virtual void OnStrokeThicknessChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.StrokeWidth = Handler!.Context.ToPixels(vCircle.StrokeThickness);
    }

    protected virtual void OnStrokeChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.StrokeColor = (vCircle.Stroke as SolidColorBrush)?.Color.ToInt()
            ?? Android.Graphics.Color.Black.ToArgb();
    }

    protected virtual void OnFillChanged(VCircle vCircle, NCircle nCircle)
    {
        nCircle.FillColor = (vCircle.Fill as SolidColorBrush)?.Color.ToInt()
            ?? Android.Graphics.Color.Black.ToArgb();
    }

    protected virtual void PlatformView_CircleClick(object? sender, GMap.CircleClickEventArgs e)
    {
        var circle = Items.Single(c => (NativeObjectAttachedProperty.GetNativeObject(c) as NCircle)!.Id == e.Circle.Id);

        VirtualView!.SendCircleClick(circle);
    }
}

public static class CircleExtensions
{
    public static CircleOptions ToNative(this VCircle circle, Context context)
    {
        var options = new CircleOptions();

        options.InvokeCenter(circle.Center.ToLatLng());
        options.InvokeRadius(circle.Radius);
        options.Clickable(circle.IsEnabled);
        options.InvokeFillColor((circle.Fill as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Black.ToArgb());
        options.InvokeStrokeColor((circle.Stroke as SolidColorBrush)?.Color.ToInt() ?? Android.Graphics.Color.Black.ToArgb());
        options.InvokeStrokeWidth(context.ToPixels(circle.StrokeThickness));
        options.InvokeZIndex(circle.ZIndex);
        options.Visible(circle.IsVisible);

        if (circle.StrokeDashPattern.Length != 0)
            options.InvokeStrokePattern(circle.StrokeDashPattern.Select(v => context.ToPixels(v)).ToArray().ToPatternItems());

        return options;
    }
}