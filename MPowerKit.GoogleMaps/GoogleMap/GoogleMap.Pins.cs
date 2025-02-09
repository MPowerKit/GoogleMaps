using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace MPowerKit.GoogleMaps;

public partial class GoogleMap
{
    public const string PinManagerName = nameof(PinManagerName);

    public event Action<Pin>? PinClick;
    public event Action<Pin>? PinDragStart;
    public event Action<Pin>? PinDragging;
    public event Action<Pin>? PinDragEnd;
    public event Action<Pin>? InfoWindowClick;
    public event Action<Pin>? InfoWindowLongClick;
    public event Action<Pin>? InfoWindowClosed;

    protected Pin PrevSelectedPin { get; set; }

    protected virtual void InitPins()
    {
        this.PropertyChanging += GoogleMap_Pins_PropertyChanging;
        this.PropertyChanged += GoogleMap_Pins_PropertyChanged;
    }

    protected virtual void GoogleMap_Pins_PropertyChanging(object sender, Microsoft.Maui.Controls.PropertyChangingEventArgs e)
    {
        if (e.PropertyName == PinsProperty.PropertyName)
        {
            OnPinsChanging();
        }
        else if (e.PropertyName == SelectedPinProperty.PropertyName)
        {
            SelectedPinChanging();
        }
    }

    protected virtual void GoogleMap_Pins_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == PinsProperty.PropertyName)
        {
            OnPinsChanged();
        }
        else if (e.PropertyName == SelectedPinDataProperty.PropertyName && PinsSource is not null)
        {
            SelectedPinDataChanged();
        }
        else if (e.PropertyName == SelectedPinProperty.PropertyName)
        {
            SelectedPinChanged();
        }
    }

    protected virtual void OnPinsChanging()
    {
        if (Pins is INotifyCollectionChanged collectionChanged)
        {
            collectionChanged.CollectionChanged -= Pins_CollectionChanged;
        }
    }

    protected virtual void OnPinsChanged()
    {
        if (Pins is INotifyCollectionChanged collectionChanged)
        {
            collectionChanged.CollectionChanged += Pins_CollectionChanged;
        }
    }

    protected virtual void Pins_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnAddPins(e.NewItems!.Cast<Pin>(), e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Remove:
                OnRemovePins(e.OldItems!.Cast<Pin>(), e.OldStartingIndex);
                break;
            case NotifyCollectionChangedAction.Replace:
                OnRemovePins(e.OldItems!.Cast<Pin>(), e.OldStartingIndex);
                OnAddPins(e.NewItems!.Cast<Pin>(), e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
                OnResetPins();
                break;
        }
    }

    protected virtual void OnAddPins(IEnumerable<Pin> oldPins, int newIndex)
    {

    }

    protected virtual void OnRemovePins(IEnumerable<Pin> oldPins, int oldIndex)
    {
        if (SelectedPin is not null && oldPins.Contains(SelectedPin))
        {
            SelectedPin = null;
        }
    }

    protected virtual void OnResetPins()
    {
        if (SelectedPin is not null && Pins?.Contains(SelectedPin) is not true)
        {
            SelectedPin = null;
        }
    }

    protected virtual void SelectedPinDataChanged()
    {
        var selectedPin = Pins.FirstOrDefault(p => p.BindingContext == SelectedPinData);
        if (selectedPin?.CanBeSelected is true)
        {
            SelectedPin = selectedPin;
        }
        else
        {
            SelectedPinData = null;
        }
    }

    protected virtual void SelectedPinChanging()
    {
        PrevSelectedPin = SelectedPin;
    }

    protected virtual void SelectedPinChanged()
    {
        PrevSelectedPin?.HideInfoWindow();

        if (PinsSource is not null)
        {
            var selectedPinData = PinsSource.OfType<object>().FirstOrDefault(p => p == SelectedPin?.BindingContext);
            SelectedPinData = selectedPinData;
        }

        SelectedPin?.ShowInfoWindow();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPinClick(Pin pin)
    {
        if (SelectedPin == pin && pin.InfoWindowShown)
        {
            pin.ShowInfoWindow();
        }
        else if (pin.CanBeSelected)
        {
            SelectedPin = pin;
        }

        PinClick?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (PinClickedCommand?.CanExecute(parameter) is true)
            PinClickedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPinDragStart(Pin pin)
    {
        PinDragStart?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (PinDragStartedCommand?.CanExecute(parameter) is true)
            PinDragStartedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPinDragging(Pin pin)
    {
        PinDragging?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (PinDraggingCommand?.CanExecute(parameter) is true)
            PinDraggingCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendPinDragEnd(Pin pin)
    {
        PinDragEnd?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (PinDragEndedCommand?.CanExecute(parameter) is true)
            PinDragEndedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendInfoWindowClick(Pin pin)
    {
        InfoWindowClick?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (InfoWindowClickedCommand?.CanExecute(parameter) is true)
            InfoWindowClickedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendInfoWindowLongClick(Pin pin)
    {
        InfoWindowLongClick?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (InfoWindowLongClickedCommand?.CanExecute(parameter) is true)
            InfoWindowLongClickedCommand.Execute(parameter);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void SendInfoWindowClosed(Pin pin)
    {
        InfoWindowClosed?.Invoke(pin);

        var parameter = pin.BindingContext ?? pin;

        if (InfoWindowClosedCommand?.CanExecute(parameter) is true)
            InfoWindowClosedCommand.Execute(parameter);

        // Not sure if this needs to be here
        //if (SelectedPin is null || SelectedPin != pin) return;

        //SelectedPin = null;
    }

    #region InfoWindowTemplate
    public DataTemplate InfoWindowTemplate
    {
        get => (DataTemplate)GetValue(InfoWindowTemplateProperty);
        set => SetValue(InfoWindowTemplateProperty, value);
    }

    public static readonly BindableProperty InfoWindowTemplateProperty =
        BindableProperty.Create(
            nameof(InfoWindowTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap)
            );
    #endregion

    #region Pins
    public IEnumerable<Pin> Pins
    {
        get => (IEnumerable<Pin>)GetValue(PinsProperty);
        set => SetValue(PinsProperty, value);
    }

    public static readonly BindableProperty PinsProperty =
        BindableProperty.Create(
            nameof(Pins),
            typeof(IEnumerable<Pin>),
            typeof(GoogleMap)
            );
    #endregion

    #region PinsSource
    public IEnumerable PinsSource
    {
        get => (IEnumerable)GetValue(PinsSourceProperty);
        set => SetValue(PinsSourceProperty, value);
    }

    public static readonly BindableProperty PinsSourceProperty =
        BindableProperty.Create(
            nameof(PinsSource),
            typeof(IEnumerable),
            typeof(GoogleMap));
    #endregion

    #region SelectedPin
    public Pin SelectedPin
    {
        get => (Pin)GetValue(SelectedPinProperty);
        set => SetValue(SelectedPinProperty, value);
    }

    public static readonly BindableProperty SelectedPinProperty =
        BindableProperty.Create(
            nameof(SelectedPin),
            typeof(Pin),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.TwoWay
            );
    #endregion

    #region SelectedPinData
    public object SelectedPinData
    {
        get => (object)GetValue(SelectedPinDataProperty);
        set => SetValue(SelectedPinDataProperty, value);
    }

    public static readonly BindableProperty SelectedPinDataProperty =
        BindableProperty.Create(
            nameof(SelectedPinData),
            typeof(object),
            typeof(GoogleMap),
            defaultBindingMode: BindingMode.TwoWay
            );
    #endregion

    #region PinItemTemplate
    public DataTemplate PinItemTemplate
    {
        get => (DataTemplate)GetValue(PinItemTemplateProperty);
        set => SetValue(PinItemTemplateProperty, value);
    }

    public static readonly BindableProperty PinItemTemplateProperty =
        BindableProperty.Create(
            nameof(PinItemTemplate),
            typeof(DataTemplate),
            typeof(GoogleMap));
    #endregion

    #region PinClickedCommand
    public ICommand PinClickedCommand
    {
        get => (ICommand)GetValue(PinClickedCommandProperty);
        set => SetValue(PinClickedCommandProperty, value);
    }

    public static readonly BindableProperty PinClickedCommandProperty =
        BindableProperty.Create(
            nameof(PinClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region PinDragStartedCommand
    public ICommand PinDragStartedCommand
    {
        get => (ICommand)GetValue(PinDragStartedCommandProperty);
        set => SetValue(PinDragStartedCommandProperty, value);
    }

    public static readonly BindableProperty PinDragStartedCommandProperty =
        BindableProperty.Create(
            nameof(PinDragStartedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region PinDraggedCommand
    public ICommand PinDraggingCommand
    {
        get => (ICommand)GetValue(PinDraggingCommandProperty);
        set => SetValue(PinDraggingCommandProperty, value);
    }

    public static readonly BindableProperty PinDraggingCommandProperty =
        BindableProperty.Create(
            nameof(PinDraggingCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region PinDragEndedCommand
    public ICommand PinDragEndedCommand
    {
        get => (ICommand)GetValue(PinDragEndedCommandProperty);
        set => SetValue(PinDragEndedCommandProperty, value);
    }

    public static readonly BindableProperty PinDragEndedCommandProperty =
        BindableProperty.Create(
            nameof(PinDragEndedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region InfoWindowClickedCommand
    public ICommand InfoWindowClickedCommand
    {
        get => (ICommand)GetValue(InfoWindowClickedCommandProperty);
        set => SetValue(InfoWindowClickedCommandProperty, value);
    }

    public static readonly BindableProperty InfoWindowClickedCommandProperty =
        BindableProperty.Create(
            nameof(InfoWindowClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region InfoWindowLongClickedCommand
    public ICommand InfoWindowLongClickedCommand
    {
        get => (ICommand)GetValue(InfoWindowLongClickedCommandProperty);
        set => SetValue(InfoWindowLongClickedCommandProperty, value);
    }

    public static readonly BindableProperty InfoWindowLongClickedCommandProperty =
        BindableProperty.Create(
            nameof(InfoWindowLongClickedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion

    #region InfoWindowClosedCommand
    public ICommand InfoWindowClosedCommand
    {
        get => (ICommand)GetValue(InfoWindowClosedCommandProperty);
        set => SetValue(InfoWindowClosedCommandProperty, value);
    }

    public static readonly BindableProperty InfoWindowClosedCommandProperty =
        BindableProperty.Create(
            nameof(InfoWindowClosedCommand),
            typeof(ICommand),
            typeof(GoogleMap));
    #endregion
}