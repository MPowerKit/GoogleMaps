using Android.Content;
using Android.Gms.Maps.Model;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

using GMap = Android.Gms.Maps.GoogleMap;
using NPolyline = Android.Gms.Maps.Model.Polyline;
using VPolyline = Microsoft.Maui.Controls.Shapes.Polyline;

namespace MPowerKit.GoogleMaps;

public class PolylineManager : ItemsMapFeatureManager<VPolyline, NPolyline, GMap>
{
    protected override IEnumerable<VPolyline> VirtualViewItems => VirtualView!.Polylines;
    protected override string VirtualViewItemsPropertyName => GoogleMap.PolylinesProperty.PropertyName;

    protected override void SubscribeToEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        base.SubscribeToEvents(virtualView, platformView, handler);

        platformView.PolylineClick += PlatformView_PolylineClick;
    }

    protected override void UnsubscribeFromEvents(GoogleMap virtualView, GMap platformView, GoogleMapHandler handler)
    {
        platformView.PolylineClick -= PlatformView_PolylineClick;

        base.UnsubscribeFromEvents(virtualView, platformView, handler);
    }

    protected override void RemoveItemFromPlatformView(NPolyline? nItem)
    {
        nItem?.Remove();
    }

    protected override NPolyline AddItemToPlatformView(VPolyline vItem)
    {
        return PlatformView!.AddPolyline(vItem.ToNative(Handler!.Context));
    }

    protected override void ItemPropertyChanged(VPolyline vItem, NPolyline nItem, string? propertyName)
    {
        base.ItemPropertyChanged(vItem, nItem, propertyName);

        if (propertyName == Shape.IsEnabledProperty.PropertyName)
        {
            OnIsEnabledChanged(vItem, nItem);
        }
        else if (propertyName == VPolyline.PointsProperty.PropertyName)
        {
            OnPointsChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeLineJoinProperty.PropertyName)
        {
            OnStrokeLineJoinChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeDashArrayProperty.PropertyName)
        {
            OnStrokeDashArrayChanged(vItem, nItem);
        }
        else if (propertyName == Shape.IsVisibleProperty.PropertyName)
        {
            OnIsVisibleChanged(vItem, nItem);
        }
        else if (propertyName == Shape.StrokeLineCapProperty.PropertyName)
        {
            OnStrokeLineCapChanged(vItem, nItem);
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
        else if (propertyName == PolylineAttached.TextureStampProperty.PropertyName)
        {
            OnTextureStampChanged(vItem, nItem);
        }
    }

    protected virtual void OnIsEnabledChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.Clickable = vPolyline.IsEnabled;
    }

    protected virtual void OnPointsChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.Points = vPolyline.Points.Select(p => p.ToLatLng()).ToList();
    }

    protected virtual void OnStrokeLineJoinChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.JointType = (int)vPolyline.StrokeLineJoin;
    }

    protected virtual void OnStrokeDashArrayChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.Pattern = vPolyline.StrokeDashPattern.Length > 0
            ? vPolyline.StrokeDashPattern.Select(v => Handler!.Context.ToPixels(v)).ToArray().ToPatternItems()
            : null;
    }

    protected virtual void OnIsVisibleChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.Visible = vPolyline.IsVisible;
    }

    protected virtual void OnStrokeLineCapChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        var cap = vPolyline.StrokeLineCap.ToCap();

        nPolyline.StartCap = cap;
        nPolyline.EndCap = cap;
    }

    protected virtual void OnZIndexChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.ZIndex = vPolyline.ZIndex;
    }

    protected virtual void OnStrokeThicknessChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.Width = Handler!.Context.ToPixels(vPolyline.StrokeThickness);
    }

    protected virtual void OnStrokeChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        nPolyline.Spans = [vPolyline.Stroke.ToSpan(vPolyline, Handler!.Context)];
    }

    protected virtual void OnTextureStampChanged(VPolyline vPolyline, NPolyline nPolyline)
    {
        OnStrokeChanged(vPolyline, nPolyline);
    }

    protected virtual void PlatformView_PolylineClick(object? sender, GMap.PolylineClickEventArgs e)
    {
        var polyline = Items.SingleOrDefault(p => (NativeObjectAttachedProperty.GetNativeObject(p) as NPolyline)!.Id == e.Polyline.Id);
        if (polyline is null) return;

        VirtualView!.SendPolylineClick(polyline);
    }
}

public static class PolylineExtensions
{
    public static PolylineOptions ToNative(this VPolyline polyline, Context context)
    {
        var options = new PolylineOptions();

        foreach (var point in polyline.Points)
        {
            options.Add(point.ToLatLng());
        }

        options.Clickable(polyline.IsEnabled);

        options.AddSpan(polyline.Stroke.ToSpan(polyline, context));
        options.InvokeWidth(context.ToPixels(polyline.StrokeThickness));
        options.InvokeZIndex(polyline.ZIndex);
        var cap = polyline.StrokeLineCap.ToCap();
        options.InvokeStartCap(cap);
        options.InvokeEndCap(cap);
        options.InvokeJointType((int)polyline.StrokeLineJoin);

        if (polyline.StrokeDashPattern.Length != 0)
            options.InvokePattern(polyline.StrokeDashPattern.Select(v => context.ToPixels(v)).ToArray().ToPatternItems());

        options.Visible(polyline.IsVisible);

        return options;
    }

    public static StyleSpan ToSpan(this Brush? brush, VPolyline polyline, Context context)
    {
        var texture = PolylineAttached.GetTextureStamp(polyline);

        var builder = brush switch
        {
            SolidColorBrush solidBrush => StrokeStyle.ColorBuilder(solidBrush.Color.ToInt()),
            LinearGradientBrush gradientBrush => gradientBrush.GradientStops.Count switch
            {
                0 => StrokeStyle.ColorBuilder(Android.Graphics.Color.Black.ToArgb()),
                1 => StrokeStyle.ColorBuilder(gradientBrush.GradientStops[0].Color.ToInt()),
                _ => StrokeStyle.GradientBuilder(
                        gradientBrush.GradientStops[0].Color.ToInt(),
                        gradientBrush.GradientStops[1].Color.ToInt())
            },
            _ => StrokeStyle.ColorBuilder(Android.Graphics.Color.Black.ToArgb())
        };

        if (!string.IsNullOrWhiteSpace(texture))
        {
            var bitmap = texture.GetBitmap(context);
            builder.Stamp(TextureStyle.NewBuilder(BitmapDescriptorFactory.FromBitmap(bitmap)).Build());
        }

        return new StyleSpan(builder.Build());
    }

    public static Cap ToCap(this PenLineCap cap)
    {
        return cap switch
        {
            PenLineCap.Flat => new ButtCap(),
            PenLineCap.Round => new RoundCap(),
            PenLineCap.Square => new SquareCap(),
            _ => throw new ArgumentOutOfRangeException(nameof(cap), cap, null)
        };
    }
}