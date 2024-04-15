

var allMarkers = [];
function initializeMap(routeData) {
    routeData = JSON.parse(routeData);
    var map = L.map('map').setView([routeData.start.Latitude, routeData.start.Longitude], 13);
    

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);

    allMarkers.length = 0;
    routeData.legs.forEach(leg => {

        var markerIcon = L.divIcon({
            className: '',
            html: '<div class="marker-background"><i class="wi wi-map ' + leg.weather.icon + '"></i></div>',
            iconSize: [40, 40],
            iconAnchor: [25, 25]
        });

        var marker = L.marker([leg.start.Latitude, leg.start.Longitude], { icon: markerIcon }).addTo(map);

        allMarkers.push(marker);

        var popupContent = `<div>
            <strong>Time:</strong> ${leg.time}<br>
            <strong>Location:</strong> ${leg.start.Name}<br>
            <strong>Temp:</strong> ${leg.weather.temperature}° F<br>
            <strong>Wind:</strong> ${leg.weather.windSpeed} mph
        </div>`;

        marker.bindTooltip(popupContent, {
            permanent: false,
            direction: 'top',
            //className: 'custom-tooltip',
            //offset: L.point(0, -20)
        })
    });
    var markerGroup = L.featureGroup(allMarkers).addTo(map);

    var latlngs = routeData.legs.map(leg => [leg.start.Latitude, leg.start.Longitude]);
    latlngs.push([routeData.end.Latitude, routeData.end.Longitude]); // Ensure the route ends correctly
    var polyline = L.polyline(latlngs, { color: 'blue' }).addTo(map);
    map.fitBounds(polyline.getBounds());
    map.on('zoomend', function () {
        updateMarkerVisibility(map, allMarkers);
    })
    updateMarkerVisibility(map, allMarkers);
}


function updateMarkerVisibility(map) {
    const zoom = map.getZoom();
    const threshold = 0.02 * Math.pow(2, 28 - zoom);

    let shownMarkers = [];
    allMarkers.forEach(marker => {
        if (isTooClose(marker, shownMarkers, threshold)) {
            //marker.setOpacity(0);
            //marker.off('click');
            //marker.off('mouseover');
            marker.getElement().classList.add('marker-hidden');
            marker.getElement().classList.remove('marker-visible');
            marker.closeTooltip();
        } else {
            shownMarkers.push(marker); // Add to shown markers
            console.log("Showing marker", marker)
            //marker.setOpacity(1); // Show the marker
            //marker.on('click', function () { marker.openPopup() });
            //marker.bindTooltip()
            //marker.on('mouseover', function () { marker.showToolip();  });
            marker.getElement().classList.add('marker-visible');
            marker.getElement().classList.remove('marker-hidden');
            console.log(marker.getElement().classList);
        }
    });
}


function isTooClose(marker, shownMarkers, threshold) {
    return shownMarkers.some(shownMarker => marker.getLatLng().distanceTo(shownMarker.getLatLng()) < threshold);
}