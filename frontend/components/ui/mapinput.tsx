"use client";

import { useEffect } from "react";
import { MapContainer, TileLayer, Marker, useMapEvents, useMap } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";

const icon = L.icon({
  iconUrl: "https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon.png",
  shadowUrl: "https://unpkg.com/leaflet@1.7.1/dist/images/marker-shadow.png",
  iconSize: [25, 41],
  iconAnchor: [12, 41],
});

interface MapInputProps {
  lat: number | null;
  lng: number | null;
  onChange: (lat: number, lng: number) => void;
}

function MapEvents({ onChange }: { onChange: (lat: number, lng: number) => void }) {
  useMapEvents({
    click(e) {
      onChange(e.latlng.lat, e.latlng.lng);
    },
  });
  return null;
}

function ChangeView({ center }: { center: [number, number] }) {
  const map = useMap();
  useEffect(() => {
    map.setView(center, map.getZoom());
  }, [center, map]);
  return null;
}

export default function MapInput({ lat, lng, onChange }: MapInputProps) {
  const malmoPosition: [number, number] = [55.6050, 13.0038];
  
  const activeCenter: [number, number] = lat && lng ? [lat, lng] : malmoPosition;

  useEffect(() => {
    
    if ("geolocation" in navigator) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const { latitude, longitude } = position.coords;
          // Sätt positionen automatiskt i formuläret
          onChange(latitude, longitude);
        },
        (error) => {
          console.log("Could not find position, or declined:", error.message);
        },
        { enableHighAccuracy: true, timeout: 5000 }
      );
    }
  }, []); 

  return (
    <div className="h-64 w-full rounded-md overflow-hidden border border-input">
      <MapContainer
        center={activeCenter}
        zoom={13}
        style={{ height: "100%", width: "100%" }}
      >
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <MapEvents onChange={onChange} />
        <ChangeView center={activeCenter} />
        
        {lat && lng && <Marker position={[lat, lng]} icon={icon} />}
      </MapContainer>
    </div>
  );
}