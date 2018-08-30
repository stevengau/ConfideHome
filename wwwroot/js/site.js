
function generateMap(data) {
    var jdata = JSON.parse(data);
    var e = [];
    var geo = {
        type: "FeatureCollection",
        features: []
    };

    for (var i = 0; i < jdata.length; i++) {
        var a = jdata[i];

        var h = {
            type: "Feature",
            geometry: {
                type: "Point",
                coordinates: [a.Location.coordinates[1], a.Location.coordinates[0]]
            },
            properties: {
                listing_id: a.MlsNo
            }
        };
        geo.features.push(h);
    }

    var json = {
        type: "FeatureCollection",
        features: [{
            type: "Feature",
            properties: {},
            geometry: {
                type: "Point",
                coordinates: [
                    -79.3838,
                    43.6475
                ]
            }
        }]
    };

    if (map.getSource("earthquakes") == undefined) {
        map.addSource("earthquakes", {
            type: "geojson",
            data: geo,
            cluster: true,
            clusterMaxZoom: 14, // Max zoom to cluster points on
            clusterRadius: 50 // Radius of each cluster when clustering points (defaults to 50)
        });
        //console.log("****************************************");
        map.addLayer({
            id: "clusters",
            type: "circle",
            source: "earthquakes",
            filter: ["has", "point_count"],
            paint: {
                "circle-color": [
                    "step",
                    ["get", "point_count"],
                    "#51bbd6",
                    100,
                    "#f1f075",
                    750,
                    "#f28cb1"
                ],
                "circle-radius": [
                    "step",
                    ["get", "point_count"],
                    20,
                    100,
                    30,
                    750,
                    40
                ]
            }
        });

        map.addLayer({
            id: "cluster-count",
            type: "symbol",
            source: "earthquakes",
            filter: ["has", "point_count"],
            layout: {
                "text-field": "{point_count_abbreviated}",
                "text-font": ["DIN Offc Pro Medium", "Arial Unicode MS Bold"],
                "text-size": 12
            }
        });

        map.addLayer({
            id: "unclustered-point",
            type: "circle",
            source: "earthquakes",
            filter: ["!", ["has", "point_count"]],
            paint: {
                "circle-color": "#11b4da",
                "circle-radius": 14,
                "circle-stroke-width": 1,
                "circle-stroke-color": "#fff"
            }
        });
    };

    map.getSource("earthquakes").setData(geo);


    // inspect a cluster on click
    map.on('click', 'clusters', function (e) {
        var features = map.queryRenderedFeatures(e.point, { layers: ['clusters'] });
        console.log(features);
        var clusterId = features[0].properties.cluster_id;
        var leaves = map.getSource('earthquakes').getClusterLeaves(clusterId, 50, 0, function (error, features) {

            console.log(features);
        });
    });

    map.on('mouseenter', 'clusters', function () {
        map.getCanvas().style.cursor = 'pointer';
    });
    map.on('mouseleave', 'clusters', function () {
        map.getCanvas().style.cursor = '';
    });

}
