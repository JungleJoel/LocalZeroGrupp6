"use client";

import { MapContainer, TileLayer, Marker } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";

// Fix för standard-ikoner
const icon = L.icon({
  iconUrl: "https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon.png",
  shadowUrl: "https://unpkg.com/leaflet@1.7.1/dist/images/marker-shadow.png",
  iconSize: [20, 32], // Något mindre ikon för minikartan
  iconAnchor: [10, 32],
});

interface MiniMapProps {
  lat: number;
  lng: number;
}

export default function StaticMiniMap({ lat, lng }: MiniMapProps) {
  const position: [number, number] = [lat, lng];

  return (
    <div className="h-60 w-full rounded-md overflow-hidden border border-border relative">
      <MapContainer
        center={position}
        zoom={14}
        style={{ height: "100%", width: "100%" }}
        // Inaktivera alla interaktioner så kartan fungerar som en statisk bild
        zoomControl={false}
        scrollWheelZoom={false}
        doubleClickZoom={false}
        dragging={false}
        boxZoom={false}
        touchZoom={false}
      >
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OSM</a>'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <Marker position={position} icon={icon} />
      </MapContainer>
      
      {/* Ett osynligt lager ovanpå kartan som hindrar klickhändelser från att störa när man scrollar på sidan */}
      <div className="absolute inset-0 z-[500] bg-transparent cursor-default" />
    </div>
  );
}