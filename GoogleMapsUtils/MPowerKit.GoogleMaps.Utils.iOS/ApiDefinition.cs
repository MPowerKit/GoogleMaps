using System;
using System.IO;
using System.Runtime.InteropServices;
using CoreGraphics;
using CoreLocation;
using Foundation;
using Google.Maps;
using Google.Maps.Utils;
using ObjCRuntime;
using UIKit;

namespace Google.Maps.Utils
{
    [Protocol, Model]
    [BaseType(typeof(NSObject), Name = "GMUClusterItem")]
    interface ClusterItem
    {
        // @required @property (readonly, nonatomic) CLLocationCoordinate2D position;
        [Abstract]
        [Export("position")]
        CLLocationCoordinate2D Position { get; }
    }

    // @interface GMSMarker_GMUClusteritem
    interface IClusterItem
    {
    }

    [Protocol, Model]
    [BaseType(typeof(NSObject), Name = "GMUCluster")]
    interface Cluster
    {
        // @required @property (readonly, nonatomic) CLLocationCoordinate2D position;
        [Abstract]
        [Export("position")]
        CLLocationCoordinate2D Position { get; }

        // @required @property (readonly, nonatomic) NSUInteger count;
        [Abstract]
        [Export("count")]
        nuint Count { get; }

        // @required @property (readonly, nonatomic) NSArray<id<GMUClusterItem>> * _Nonnull items;
        [Abstract]
        [Export("items")]
        IClusterItem[] Items { get; }
    }

    public interface ICluster
    {
    }

    [Protocol, Model]
    [BaseType(typeof(NSObject), Name = "GMUClusterAlgorithm")]
    interface ClusterAlgorithm
    {
        // @required -(void)addItems:(NSArray<id<GMUClusterItem>> * _Nonnull)items;
        [Abstract]
        [Export("addItems:")]
        void AddItems(IClusterItem[] items);

        // @required -(void)removeItem:(id<GMUClusterItem> _Nonnull)item;
        [Abstract]
        [Export("removeItem:")]
        void RemoveItem(IClusterItem item);

        // @required -(void)clearItems;
        [Abstract]
        [Export("clearItems")]
        void ClearItems();

        // @required -(NSArray<id<GMUCluster>> * _Nonnull)clustersAtZoom:(float)zoom;
        [Abstract]
        [Export("clustersAtZoom:")]
        ICluster[] ClustersAtZoom(float zoom);
    }

    interface IClusterAlgorithm { }

    [Protocol, Model]
    [BaseType(typeof(NSObject), Name = "GMUClusterIconGenerator")]
    interface ClusterIconGenerator
    {
        // @required -(UIImage *)iconForSize:(NSUInteger)size;
        [Abstract]
        [Export("iconForSize:")]
        UIImage IconForSize(nuint size);
    }

    interface IClusterIconGenerator { }

    [Protocol, Model]
    [BaseType(typeof(NSObject), Name = "GMUClusterRenderer")]
    interface ClusterRenderer
    {
        // @required -(void)renderClusters:(NSArray<id<GMUCluster>> * _Nonnull)clusters;
        [Abstract]
        [Export("renderClusters:")]
        void RenderClusters(ICluster[] clusters);

        // @required -(void)update;
        [Abstract]
        [Export("update")]
        void Update();
    }

    public interface IClusterRenderer
    {
    }

    // @protocol GMUClusterManagerDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject), Name = "GMUClusterManagerDelegate")]
    interface ClusterManagerDelegate
    {
        // @optional -(BOOL)clusterManager:(GMUClusterManager * _Nonnull)clusterManager didTapCluster:(id<GMUCluster> _Nonnull)cluster;
        [Export("clusterManager:didTapCluster:")]
        bool DidTapCluster(ClusterManager clusterManager, ICluster cluster);

        // @optional -(BOOL)clusterManager:(GMUClusterManager * _Nonnull)clusterManager didTapClusterItem:(id<GMUClusterItem> _Nonnull)clusterItem;
        [Export("clusterManager:didTapClusterItem:")]
        bool DidTapClusterItem(ClusterManager clusterManager, IClusterItem clusterItem);
    }

    public interface IClusterManagerDelegate
    {
    }

    // @interface GMUClusterManager : NSObject
    [BaseType(typeof(NSObject), Name = "GMUClusterManager")]
    [DisableDefaultCtor]
    interface ClusterManager
    {
        // -(instancetype _Nonnull)initWithMap:(id)mapView algorithm:(id<GMUClusterAlgorithm> _Nonnull)algorithm renderer:(id<GMUClusterRenderer> _Nonnull)renderer __attribute__((objc_designated_initializer));
        [Export("initWithMap:algorithm:renderer:")]
        [DesignatedInitializer]
        IntPtr Constructor(NSObject mapView, IClusterAlgorithm algorithm, IClusterRenderer renderer);

        // @property (readonly, nonatomic) id<GMUClusterAlgorithm> _Nonnull algorithm;
        [Export("algorithm")]
        IClusterAlgorithm Algorithm { get; }

        [Wrap("WeakDelegate")]
        [NullAllowed]
        IClusterManagerDelegate Delegate { get; }

        // @property (readonly, nonatomic, weak) id<GMUClusterManagerDelegate> _Nullable delegate;
        [NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
        NSObject WeakDelegate { get; }

        [Wrap("WeakMapDelegate")]
        [NullAllowed]
        IMapViewDelegate MapDelegate { get; }

        // @property (readonly, nonatomic, weak) id _Nullable mapDelegate;
        [NullAllowed, Export("mapDelegate", ArgumentSemantic.Weak)]
        NSObject WeakMapDelegate { get; }

        // -(void)setMapDelegate:(id _Nullable)mapDelegate;
        [Export("setMapDelegate:")]
        void SetMapDelegate([NullAllowed] IMapViewDelegate mapDelegate);

        // -(void)setDelegate:(id<GMUClusterManagerDelegate> _Nullable)delegate mapDelegate:(id _Nullable)mapDelegate;
        [Export("setDelegate:mapDelegate:")]
        void SetDelegate([NullAllowed] IClusterManagerDelegate @delegate, [NullAllowed] IMapViewDelegate mapDelegate);

        // -(void)addItem:(id<GMUClusterItem> _Nonnull)item;
        [Export("addItem:")]
        void AddItem(IClusterItem item);

        // -(void)addItems:(NSArray<id<GMUClusterItem>> * _Nonnull)items;
        [Export("addItems:")]
        void AddItems(IClusterItem[] items);

        // -(void)removeItem:(id<GMUClusterItem> _Nonnull)item;
        [Export("removeItem:")]
        void RemoveItem(IClusterItem item);

        // -(void)clearItems;
        [Export("clearItems")]
        void ClearItems();

        // -(void)cluster;
        [Export("cluster")]
        void Cluster();
    }

    // @interface GMUDefaultClusterIconGenerator : NSObject <GMUClusterIconGenerator>
    [BaseType(typeof(ClusterIconGenerator), Name = "GMUDefaultClusterIconGenerator")]
    interface DefaultClusterIconGenerator : IClusterIconGenerator
    {
        // -(instancetype _Nonnull)initWithBuckets:(NSArray<NSNumber *> * _Nonnull)buckets;
        [Export("initWithBuckets:")]
        IntPtr Constructor(NSNumber[] buckets);

        // -(instancetype _Nonnull)initWithBuckets:(NSArray<NSNumber *> * _Nonnull)buckets backgroundImages:(NSArray<UIImage *> * _Nonnull)backgroundImages;
        [Export("initWithBuckets:backgroundImages:")]
        IntPtr Constructor(NSNumber[] buckets, UIImage[] backgroundImages);

        // -(instancetype _Nonnull)initWithBuckets:(NSArray<NSNumber *> * _Nonnull)buckets backgroundColors:(NSArray<UIColor *> * _Nonnull)backgroundColors;
        [Export("initWithBuckets:backgroundColors:")]
        IntPtr Constructor(NSNumber[] buckets, UIColor[] backgroundColors);

        // -(UIImage * _Nonnull)iconForSize:(NSUInteger)size;
        [Override]
        [Export("iconForSize:")]
        UIImage IconForSize(nuint size);
    }

    // @protocol GMUClusterRendererDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject), Name = "GMUClusterRendererDelegate")]
    interface ClusterRendererDelegate
    {
        // @optional -(GMSMarker * _Nullable)renderer:(id<GMUClusterRenderer> _Nonnull)renderer markerForObject:(id _Nonnull)object;
        [Export("renderer:markerForObject:")]
        [return: NullAllowed]
        Marker Renderer(IClusterRenderer renderer, NSObject @object);

        // @optional -(void)renderer:(id<GMUClusterRenderer> _Nonnull)renderer willRenderMarker:(GMSMarker * _Nonnull)marker;
        [Export("renderer:willRenderMarker:")]
        void WillRenderMarker(IClusterRenderer renderer, Marker marker);

        // @optional -(void)renderer:(id<GMUClusterRenderer> _Nonnull)renderer didRenderMarker:(GMSMarker * _Nonnull)marker;
        [Export("renderer:didRenderMarker:")]
        void DidRenderMarker(IClusterRenderer renderer, Marker marker);
    }

    interface IClusterRendererDelegate { }

    // @interface GMUDefaultClusterRenderer : NSObject <GMUClusterRenderer>
    [BaseType(typeof(ClusterRenderer), Name = "GMUDefaultClusterRenderer")]
    interface DefaultClusterRenderer : ClusterRenderer
    {
        // @required -(void)renderClusters:(NSArray<id<GMUCluster>> * _Nonnull)clusters;
        [Override]
        [Export("renderClusters:")]
        void RenderClusters(ICluster[] clusters);

        // @required -(void)update;
        [Override]
        [Export("update")]
        void Update();

        // -(instancetype _Nonnull)initWithMapView:(GMSMapView * _Nonnull)mapView clusterIconGenerator:(id<GMUClusterIconGenerator> _Nonnull)iconGenerator;
        [Export("initWithMapView:clusterIconGenerator:")]
        IntPtr Constructor(MapView mapView, IClusterIconGenerator iconGenerator);

        // @property (nonatomic) BOOL animatesClusters;
        [Export("animatesClusters")]
        bool AnimatesClusters { get; set; }

        // @property (nonatomic) NSUInteger minimumClusterSize;
        [Export("minimumClusterSize")]
        nuint MinimumClusterSize { get; set; }

        // @property (nonatomic) NSUInteger maximumClusterZoom;
        [Export("maximumClusterZoom")]
        nuint MaximumClusterZoom { get; set; }

        // @property (nonatomic) double animationDuration;
        [Export("animationDuration")]
        double AnimationDuration { get; set; }

        // @property (nonatomic) int zIndex;
        [Export("zIndex")]
        int ZIndex { get; set; }

        [Wrap("WeakDelegate")]
        [NullAllowed]
        IClusterRendererDelegate Delegate { get; set; }

        // @property (nonatomic, weak) id<GMUClusterRendererDelegate> _Nullable delegate;
        [NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
        NSObject WeakDelegate { get; set; }

        // @property (readonly, nonatomic) NSArray<GMSMarker *> * _Nonnull markers;
        [Export("markers")]
        Marker[] Markers { get; }

        // -(BOOL)shouldRenderAsCluster:(id<GMUCluster> _Nonnull)cluster atZoom:(float)zoom;
        [Export("shouldRenderAsCluster:atZoom:")]
        bool ShouldRenderAsCluster(ICluster cluster, float zoom);
    }

    [Protocol, Model]
    [BaseType(typeof(NSObject), Name = "GMUGeometry")]
    interface Geometry
    {
        // @required @property (readonly, nonatomic) NSString * _Nonnull type;
        [Abstract]
        [Export("type")]
        string Type { get; }
    }

    public interface IGeometry { }

    // @interface GMUStyle : NSObject
    [BaseType(typeof(NSObject), Name = "GMUStyle")]
    interface Style
    {
        // @property (readonly, nonatomic) NSString * _Nonnull styleID;
        [Export("styleID")]
        string StyleID { get; }

        // @property (readonly, nonatomic) UIColor * _Nullable strokeColor;
        [NullAllowed, Export("strokeColor")]
        UIColor StrokeColor { get; }

        // @property (readonly, nonatomic) UIColor * _Nullable fillColor;
        [NullAllowed, Export("fillColor")]
        UIColor FillColor { get; }

        // @property (readonly, nonatomic) CGFloat width;
        [Export("width")]
        nfloat Width { get; }

        // @property (readonly, nonatomic) CGFloat scale;
        [Export("scale")]
        nfloat Scale { get; }

        // @property (readonly, nonatomic) CGFloat heading;
        [Export("heading")]
        nfloat Heading { get; }

        // @property (readonly, nonatomic) CGPoint anchor;
        [Export("anchor")]
        CGPoint Anchor { get; }

        // @property (readonly, nonatomic) NSString * _Nullable iconUrl;
        [NullAllowed, Export("iconUrl")]
        string IconUrl { get; }

        // @property (readonly, nonatomic) NSString * _Nullable title;
        [NullAllowed, Export("title")]
        string Title { get; }

        // @property (readonly, nonatomic) BOOL hasFill;
        [Export("hasFill")]
        bool HasFill { get; }

        // @property (readonly, nonatomic) BOOL hasStroke;
        [Export("hasStroke")]
        bool HasStroke { get; }

        // -(instancetype _Nonnull)initWithStyleID:(NSString * _Nonnull)styleID strokeColor:(UIColor * _Nullable)strokeColor fillColor:(UIColor * _Nullable)fillColor width:(CGFloat)width scale:(CGFloat)scale heading:(CGFloat)heading anchor:(CGPoint)anchor iconUrl:(NSString * _Nullable)iconUrl title:(NSString * _Nullable)title hasFill:(BOOL)hasFill hasStroke:(BOOL)hasStroke;
        [Export("initWithStyleID:strokeColor:fillColor:width:scale:heading:anchor:iconUrl:title:hasFill:hasStroke:")]
        IntPtr Constructor(string styleID, [NullAllowed] UIColor strokeColor, [NullAllowed] UIColor fillColor, nfloat width, nfloat scale, nfloat heading, CGPoint anchor, [NullAllowed] string iconUrl, [NullAllowed] string title, bool hasFill, bool hasStroke);
    }

    [Protocol, Model]
    [BaseType(typeof(NSObject), Name = "GMUGeometryContainer")]
    interface GeometryContainer
    {
        // @required @property (readonly, nonatomic) id<GMUGeometry> _Nonnull geometry;
        [Abstract]
        [Export("geometry")]
        Geometry Geometry { get; }

        // @required @property (nonatomic) GMUStyle * _Nullable style;
        [Abstract]
        [NullAllowed, Export("style", ArgumentSemantic.Assign)]
        Style Style { get; set; }
    }

    public interface IGeometryContainer
    {
    }

    // @interface GMUFeature : NSObject <GMUGeometryContainer>
    [BaseType(typeof(GeometryContainer), Name = "GMUFeature")]
    interface Feature : IGeometryContainer
    {
        // @required @property (readonly, nonatomic) id<GMUGeometry> _Nonnull geometry;
        [Override]
        [Export("geometry")]
        Geometry Geometry { get; }

        // @required @property (nonatomic) GMUStyle * _Nullable style;
        [Override]
        [NullAllowed, Export("style", ArgumentSemantic.Assign)]
        Style Style { get; set; }

        // @property (readonly, nonatomic) NSString * _Nullable identifier;
        [NullAllowed, Export("identifier")]
        string Identifier { get; }

        // @property (readonly, nonatomic) NSDictionary<NSString *,NSObject *> * _Nullable properties;
        [NullAllowed, Export("properties")]
        NSDictionary<NSString, NSObject> Properties { get; }

        // @property (readonly, nonatomic) int * _Nullable boundingBox;
        [NullAllowed, Export("boundingBox")]
        CoordinateBounds BoundingBox { get; }

        // -(instancetype _Nonnull)initWithGeometry:(id<GMUGeometry> _Nonnull)geometry identifier:(NSString * _Nullable)identifier properties:(NSDictionary<NSString *,NSObject *> * _Nullable)properties boundingBox:(id)boundingBox;
        [Export("initWithGeometry:identifier:properties:boundingBox:")]
        IntPtr Constructor(Geometry geometry, [NullAllowed] string identifier, [NullAllowed] NSDictionary<NSString, NSObject> properties, CoordinateBounds boundingBox);
    }

    // @interface GMUGeoJSONParser : NSObject
    [BaseType(typeof(NSObject), Name = "GMUGeoJSONParser")]
    interface GMUGeoJSONParser
    {
        // @property (readonly, nonatomic) NSArray<id<GMUGeometryContainer>> * _Nonnull features;
        [Export("features")]
        GeometryContainer[] Features { get; }

        // -(instancetype _Nonnull)initWithURL:(NSURL * _Nonnull)url;
        [Export("initWithURL:")]
        IntPtr Constructor(NSUrl url);

        // -(instancetype _Nonnull)initWithData:(NSData * _Nonnull)data;
        [Export("initWithData:")]
        IntPtr Constructor(NSData data);

        // -(instancetype _Nonnull)initWithStream:(NSInputStream * _Nonnull)stream;
        [Export("initWithStream:")]
        IntPtr Constructor(NSInputStream stream);

        // -(void)parse;
        [Export("parse")]
        void Parse();
    }

    // @interface GMUGeometryCollection : NSObject <GMUGeometry>
    [BaseType(typeof(Geometry), Name = "GMUGeometryCollection")]
    interface GeometryCollection : IGeometry
    {
        [Override]
        [Export("type")]
        string Type { get; }

        // @property (readonly, nonatomic) NSArray<id<GMUGeometry>> * _Nonnull geometries;
        [Export("geometries")]
        Geometry[] Geometries { get; }

        // -(instancetype _Nonnull)initWithGeometries:(NSArray<id<GMUGeometry>> * _Nonnull)geometries;
        [Export("initWithGeometries:")]
        IntPtr Constructor(Geometry[] geometries);
    }

    // @interface GMUPair : NSObject
    [BaseType(typeof(NSObject), Name = "GMUPair")]
    interface Pair
    {
        // @property (readonly, nonatomic) NSString * _Nonnull key;
        [Export("key")]
        string Key { get; }

        // @property (readonly, nonatomic) NSString * _Nonnull styleUrl;
        [Export("styleUrl")]
        string StyleUrl { get; }

        // -(instancetype _Nonnull)initWithKey:(NSString * _Nonnull)styleID styleUrl:(NSString * _Nonnull)strokeColor;
        [Export("initWithKey:styleUrl:")]
        IntPtr Constructor(string styleID, string strokeColor);
    }

    // @interface GMUStyleMap : NSObject
    [BaseType(typeof(NSObject), Name = "GMUStyleMap")]
    interface StyleMap
    {
        // @property (readonly, nonatomic) NSString * _Nonnull styleMapId;
        [Export("styleMapId")]
        string StyleMapId { get; }

        // @property (readonly, nonatomic) NSArray<GMUPair *> * _Nonnull pairs;
        [Export("pairs")]
        Pair[] Pairs { get; }

        // -(instancetype _Nonnull)initWithId:(NSString * _Nonnull)styleMapId pairs:(NSArray<GMUPair *> * _Nonnull)pairs;
        [Export("initWithId:pairs:")]
        IntPtr Constructor(string styleMapId, Pair[] pairs);
    }

    // @interface GMUGeometryRenderer : NSObject
    [BaseType(typeof(NSObject), Name = "GMUGeometryRenderer")]
    interface GeometryRenderer
    {
        // -(instancetype _Nonnull)initWithMap:(GMSMapView * _Nonnull)map geometries:(NSArray<id<GMUGeometryContainer>> * _Nonnull)geometries;
        [Export("initWithMap:geometries:")]
        IntPtr Constructor(MapView map, GeometryContainer[] geometries);

        // -(instancetype _Nonnull)initWithMap:(GMSMapView * _Nonnull)map geometries:(NSArray<id<GMUGeometryContainer>> * _Nonnull)geometries styles:(NSArray<GMUStyle *> * _Nullable)styles;
        [Export("initWithMap:geometries:styles:")]
        IntPtr Constructor(MapView map, GeometryContainer[] geometries, [NullAllowed] Style[] styles);

        // -(instancetype _Nonnull)initWithMap:(GMSMapView * _Nonnull)map geometries:(NSArray<id<GMUGeometryContainer>> * _Nonnull)geometries styles:(NSArray<GMUStyle *> * _Nullable)styles styleMaps:(NSArray<GMUStyleMap *> * _Nullable)styleMaps;
        [Export("initWithMap:geometries:styles:styleMaps:")]
        IntPtr Constructor(MapView map, GeometryContainer[] geometries, [NullAllowed] Style[] styles, [NullAllowed] StyleMap[] styleMaps);

        // -(void)render;
        [Export("render")]
        void Render();

        // -(void)clear;
        [Export("clear")]
        void Clear();
    }

    // @interface GMUGradient : NSObject
    [BaseType(typeof(NSObject), Name = "GMUGradient")]
    interface Gradient
    {
        // @property (readonly, nonatomic) NSUInteger mapSize;
        [Export("mapSize")]
        nuint MapSize { get; }

        // @property (readonly, nonatomic) NSArray<UIColor *> * _Nonnull colors;
        [Export("colors")]
        UIColor[] Colors { get; }

        // @property (readonly, nonatomic) NSArray<NSNumber *> * _Nonnull startPoints;
        [Export("startPoints")]
        NSNumber[] StartPoints { get; }

        // -(instancetype _Nonnull)initWithColors:(NSArray<UIColor *> * _Nonnull)colors startPoints:(NSArray<NSNumber *> * _Nonnull)startPoints colorMapSize:(NSUInteger)mapSize;
        [Export("initWithColors:startPoints:colorMapSize:")]
        IntPtr Constructor(UIColor[] colors, NSNumber[] startPoints, nuint mapSize);

        // -(NSArray<UIColor *> * _Nonnull)generateColorMap;
        [Export("generateColorMap")]
        UIColor[] GenerateColorMap { get; }
    }

    // @interface GMUGridBasedClusterAlgorithm : NSObject <GMUClusterAlgorithm>
    [BaseType(typeof(ClusterAlgorithm), Name = "GMUGridBasedClusterAlgorithm")]
    interface GridBasedClusterAlgorithm : IClusterAlgorithm
    {
        [Override]
        [Export("addItems:")]
        void AddItems(IClusterItem[] items);

        [Override]
        [Export("removeItem:")]
        void RemoveItem(IClusterItem item);

        [Override]
        [Export("clearItems")]
        void ClearItems();

        [Override]
        [Export("clustersAtZoom:")]
        ICluster[] ClustersAtZoom(float zoom);
    }

    // @interface GMUGroundOverlay : NSObject <GMUGeometry>
    [BaseType(typeof(Geometry), Name = "GMUGroundOverlay")]
    interface GroundOverlay : IGeometry
    {
        [Override]
        [Export("type")]
        string Type { get; }

        // @property (readonly, nonatomic) CLLocationCoordinate2D northEast;
        [Export("northEast")]
        CLLocationCoordinate2D NorthEast { get; }

        // @property (readonly, nonatomic) CLLocationCoordinate2D southWest;
        [Export("southWest")]
        CLLocationCoordinate2D SouthWest { get; }

        // @property (readonly, nonatomic) int zIndex;
        [Export("zIndex")]
        int ZIndex { get; }

        // @property (readonly, nonatomic) double rotation;
        [Export("rotation")]
        double Rotation { get; }

        // @property (readonly, nonatomic) NSString * _Nonnull href;
        [Export("href")]
        string Href { get; }

        // -(instancetype _Nonnull)initWithCoordinate:(CLLocationCoordinate2D)northEast southWest:(CLLocationCoordinate2D)southWest zIndex:(int)zIndex rotation:(double)rotation href:(NSString * _Nonnull)href;
        [Export("initWithCoordinate:southWest:zIndex:rotation:href:")]
        IntPtr Constructor(CLLocationCoordinate2D northEast, CLLocationCoordinate2D southWest, int zIndex, double rotation, string href);
    }

    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface GQTPointQuadTreeItem
    {
        // @required -(GQTPoint)point;
        [Abstract]
        [Export("point")]
        GQTPoint Point { get; }
    }

    interface IGQTPointQuadTreeItem { }

    // @interface GMUWeightedLatLng : NSObject <GQTPointQuadTreeItem>
    [BaseType(typeof(GQTPointQuadTreeItem), Name = "GMUWeightedLatLng")]
    interface WeightedLatLng : IGQTPointQuadTreeItem
    {
        // @required -(GQTPoint)point;
        [Override]
        [Export("point")]
        GQTPoint Point { get; }

        // @property (readonly, nonatomic) float intensity;
        [Export("intensity")]
        float Intensity { get; }

        // -(instancetype _Nonnull)initWithCoordinate:(CLLocationCoordinate2D)coordinate intensity:(float)intensity;
        [Export("initWithCoordinate:intensity:")]
        IntPtr Constructor(CLLocationCoordinate2D coordinate, float intensity);
    }

    // @interface GMUHeatmapTileLayer
    [BaseType(typeof(SyncTileLayer), Name = "GMUHeatmapTileLayer")]
    interface HeatmapTileLayer
    {
        // @property (copy, nonatomic) NSArray<GMUWeightedLatLng *> * _Nonnull weightedData;
        [Export("weightedData", ArgumentSemantic.Copy)]
        WeightedLatLng[] WeightedData { get; set; }

        // @property (nonatomic) NSUInteger radius;
        [Export("radius")]
        nuint Radius { get; set; }

        // @property (nonatomic) GMUGradient * _Nonnull gradient;
        [Export("gradient", ArgumentSemantic.Assign)]
        Gradient Gradient { get; set; }

        // @property (nonatomic) NSUInteger minimumZoomIntensity;
        [Export("minimumZoomIntensity")]
        nuint MinimumZoomIntensity { get; set; }

        // @property (nonatomic) NSUInteger maximumZoomIntensity;
        [Export("maximumZoomIntensity")]
        nuint MaximumZoomIntensity { get; set; }
    }

    // @interface GMUKMLParser : NSObject
    [BaseType(typeof(NSObject), Name = "GMUKMLParser")]
    interface KMLParser
    {
        // @property (readonly, nonatomic) NSArray<id<GMUGeometryContainer>> * _Nonnull placemarks;
        [Export("placemarks")]
        GeometryContainer[] Placemarks { get; }

        // @property (readonly, nonatomic) NSArray<GMUStyle *> * _Nonnull styles;
        [Export("styles")]
        Style[] Styles { get; }

        // @property (readonly, nonatomic) NSArray<GMUStyleMap *> * _Nonnull styleMaps;
        [Export("styleMaps")]
        StyleMap[] StyleMaps { get; }

        // -(void)parse;
        [Export("parse")]
        void Parse();

        // -(instancetype _Nonnull)initWithURL:(NSURL * _Nonnull)url;
        [Export("initWithURL:")]
        IntPtr Constructor(NSUrl url);

        // -(instancetype _Nonnull)initWithData:(NSData * _Nonnull)data;
        [Export("initWithData:")]
        IntPtr Constructor(NSData data);

        // -(instancetype _Nonnull)initWithStream:(NSInputStream * _Nonnull)stream;
        [Export("initWithStream:")]
        IntPtr Constructor(NSInputStream stream);
    }

    // @interface GMULineString : NSObject <GMUGeometry>
    [BaseType(typeof(Geometry), Name = "GMULineString")]
    interface LineString : IGeometry
    {
        [Override]
        [Export("type")]
        string Type { get; }

        // @property (readonly, nonatomic) int * _Nonnull path;
        [Export("path")]
        unsafe Google.Maps.Path Path { get; }

        // -(instancetype _Nonnull)initWithPath:(id)path;
        [Export("initWithPath:")]
        IntPtr Constructor(Google.Maps.Path path);
    }

    // @interface GMUNonHierarchicalDistanceBasedAlgorithm : NSObject <GMUClusterAlgorithm>
    [BaseType(typeof(ClusterAlgorithm), Name = "GMUNonHierarchicalDistanceBasedAlgorithm")]
    interface NonHierarchicalDistanceBasedAlgorithm : IClusterAlgorithm
    {
        // -(instancetype)initWithClusterDistancePoints:(NSUInteger)clusterDistancePoints;
        [Export("initWithClusterDistancePoints:")]
        IntPtr Constructor(nuint clusterDistancePoints);

        [Override]
        [Export("addItems:")]
        void AddItems(IClusterItem[] items);

        [Override]
        [Export("removeItem:")]
        void RemoveItem(IClusterItem item);

        [Override]
        [Export("clearItems")]
        void ClearItems();

        [Override]
        [Export("clustersAtZoom:")]
        ICluster[] ClustersAtZoom(float zoom);
    }

    // @interface GMUPlacemark : NSObject <GMUGeometryContainer>
    [BaseType(typeof(GeometryContainer), Name = "GMUPlacemark")]
    interface Placemark : IGeometryContainer
    {
        // @property (readonly, nonatomic) NSString * _Nullable title;
        [NullAllowed, Export("title")]
        string Title { get; }

        // @property (readonly, nonatomic) NSString * _Nullable snippet;
        [NullAllowed, Export("snippet")]
        string Snippet { get; }

        // @property (readonly, nonatomic) NSString * _Nullable styleUrl;
        [NullAllowed, Export("styleUrl")]
        string StyleUrl { get; }

        // @required @property (readonly, nonatomic) id<GMUGeometry> _Nonnull geometry;
        [Override]
        [Export("geometry")]
        Geometry Geometry { get; }

        // @required @property (nonatomic) GMUStyle * _Nullable style;
        [Override]
        [NullAllowed, Export("style", ArgumentSemantic.Assign)]
        Style Style { get; set; }

        // -(instancetype _Nonnull)initWithGeometry:(id<GMUGeometry> _Nullable)geometry title:(NSString * _Nullable)title snippet:(NSString * _Nullable)snippet style:(GMUStyle * _Nullable)style styleUrl:(NSString * _Nullable)styleUrl;
        [Export("initWithGeometry:title:snippet:style:styleUrl:")]
        IntPtr Constructor([NullAllowed] Geometry geometry, [NullAllowed] string title, [NullAllowed] string snippet, [NullAllowed] Style style, [NullAllowed] string styleUrl);
    }

    // @interface GMUPoint : NSObject <GMUGeometry>
    [BaseType(typeof(Geometry), Name = "GMUPoint")]
    interface Point : IGeometry
    {
        [Override]
        [Export("type")]
        string Type { get; }

        // @property (readonly, nonatomic) CLLocationCoordinate2D coordinate;
        [Export("coordinate")]
        CLLocationCoordinate2D Coordinate { get; }

        // -(instancetype _Nonnull)initWithCoordinate:(CLLocationCoordinate2D)coordinate;
        [Export("initWithCoordinate:")]
        IntPtr Constructor(CLLocationCoordinate2D coordinate);
    }

    // @interface GMUPolygon : NSObject <GMUGeometry>
    [BaseType(typeof(Geometry), Name = "GMUPolygon")]
    interface Polygon : IGeometry
    {
        [Override]
        [Export("type")]
        string Type { get; }

        // @property (readonly, nonatomic) NSArray * _Nonnull paths;
        [Export("paths")]
        Google.Maps.Path[] Paths { get; }

        // -(instancetype _Nonnull)initWithPaths:(NSArray * _Nonnull)paths;
        [Export("initWithPaths:")]
        IntPtr Constructor(Google.Maps.Path[] paths);
    }

    // @interface GMUSimpleClusterAlgorithm : NSObject <GMUClusterAlgorithm>
    [BaseType(typeof(ClusterAlgorithm), Name = "GMUSimpleClusterAlgorithm")]
    interface SimpleClusterAlgorithm : IClusterAlgorithm
    {
        [Override]
        [Export("addItems:")]
        void AddItems(IClusterItem[] items);

        [Override]
        [Export("removeItem:")]
        void RemoveItem(IClusterItem item);

        [Override]
        [Export("clearItems")]
        void ClearItems();

        [Override]
        [Export("clustersAtZoom:")]
        ICluster[] ClustersAtZoom(float zoom);
    }

    // @interface GMUStaticCluster : NSObject <GMUCluster>
    [BaseType(typeof(Cluster), Name = "GMUStaticCluster")]
    [DisableDefaultCtor]
    interface StaticCluster : ICluster
    {
        // -(instancetype _Nonnull)initWithPosition:(CLLocationCoordinate2D)position __attribute__((objc_designated_initializer));
        [Export("initWithPosition:")]
        [DesignatedInitializer]
        IntPtr Constructor(CLLocationCoordinate2D position);

        // @property (readonly, nonatomic) CLLocationCoordinate2D position;
        [Override]
        [Export("position")]
        CLLocationCoordinate2D Position { get; }

        // @property (readonly, nonatomic) NSUInteger count;
        [Override]
        [Export("count")]
        nuint Count { get; }

        // @property (readonly, nonatomic) NSArray<id<GMUClusterItem>> * _Nonnull items;
        [Override]
        [Export("items")]
        IClusterItem[] Items { get; }

        // -(void)addItem:(id<GMUClusterItem> _Nonnull)item;
        [Export("addItem:")]
        void AddItem(IClusterItem item);

        // -(void)removeItem:(id<GMUClusterItem> _Nonnull)item;
        [Export("removeItem:")]
        void RemoveItem(IClusterItem item);
    }

    // @interface GMUWrappingDictionaryKey : NSObject <NSCopying>
    [BaseType(typeof(NSCopying), Name = "GMUWrappingDictionaryKey")]
    interface WrappingDictionaryKey : INSCopying
    {
        [Override]
        [Export("copyWithZone:")]
        NSObject Copy(NSZone? zone);

        // -(instancetype)initWithObject:(id)object;
        [Export("initWithObject:")]
        IntPtr Constructor(NSObject @object);
    }

    // @interface GQTPointQuadTree : NSObject
    [BaseType(typeof(NSObject))]
    interface GQTPointQuadTree
    {
        // -(id)initWithBounds:(GQTBounds)bounds;
        [Export("initWithBounds:")]
        IntPtr Constructor(GQTBounds bounds);

        // -(BOOL)add:(id<GQTPointQuadTreeItem>)item;
        [Export("add:")]
        bool Add(GQTPointQuadTreeItem item);

        // -(BOOL)remove:(id<GQTPointQuadTreeItem>)item;
        [Export("remove:")]
        bool Remove(GQTPointQuadTreeItem item);

        // -(void)clear;
        [Export("clear")]
        void Clear();

        // -(NSArray *)searchWithBounds:(GQTBounds)bounds;
        [Export("searchWithBounds:")]
        NSObject[] SearchWithBounds(GQTBounds bounds);

        // -(NSUInteger)count;
        [Export("count")]
        nuint Count { get; }
    }

    // @interface GQTPointQuadTreeChild : NSObject
    [BaseType(typeof(NSObject))]
    interface GQTPointQuadTreeChild
    {
        // -(void)add:(id<GQTPointQuadTreeItem>)item withOwnBounds:(GQTBounds)bounds atDepth:(NSUInteger)depth;
        [Export("add:withOwnBounds:atDepth:")]
        void Add(GQTPointQuadTreeItem item, GQTBounds bounds, nuint depth);

        // -(BOOL)remove:(id<GQTPointQuadTreeItem>)item withOwnBounds:(GQTBounds)bounds;
        [Export("remove:withOwnBounds:")]
        bool Remove(GQTPointQuadTreeItem item, GQTBounds bounds);

        // -(void)searchWithBounds:(GQTBounds)searchBounds withOwnBounds:(GQTBounds)ownBounds results:(NSMutableArray *)accumulator;
        [Export("searchWithBounds:withOwnBounds:results:")]
        void SearchWithBounds(GQTBounds searchBounds, GQTBounds ownBounds, NSMutableArray accumulator);

        // -(void)splitWithOwnBounds:(GQTBounds)ownBounds atDepth:(NSUInteger)depth;
        [Export("splitWithOwnBounds:atDepth:")]
        void SplitWithOwnBounds(GQTBounds ownBounds, nuint depth);
    }
}