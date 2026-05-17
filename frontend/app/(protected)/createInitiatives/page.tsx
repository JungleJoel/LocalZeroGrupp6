"use client";

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { API_BASE_URL } from "@/lib/config";
import { toast } from "sonner";
import { useRouter } from "next/navigation";
import dynamic from "next/dynamic";

import DatePicker, { registerLocale } from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { sv } from "date-fns/locale/sv";

registerLocale("sv", sv);

const MapInput = dynamic(() => import("./mapinput"), {
  ssr: false,
  loading: () => <div className="h-64 w-full bg-muted animate-pulse rounded-md flex items-center justify-center text-sm text-muted-foreground">Laddar karta...</div>
});

export default function CreateInitiative() {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [latitude, setLatitude] = useState<number | null>(null);
  const [longitude, setLongitude] = useState<number | null>(null);
  const [startsAt, setStartsAt] = useState<Date | null>(new Date()); 
  const [estimatedEndsAt, setEstimatedEndsAt] = useState<Date | null>(null);
  
  const [ecoPoints, setEcoPoints] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [communities, setCommunities] = useState<any[]>([]); 
  const [selectedCommunityId, setSelectedCommunityId] = useState("");

  const router = useRouter();

  useEffect(() => {
    async function fetchCommunities() {
      try {
        const response = await fetch(`${API_BASE_URL}/Community/getCommunities`, {
          credentials: "include"
        });
        if (response.ok) {
          const data = await response.ok ? await response.json() : [];
          setCommunities(data);
          if (data.length > 0) setSelectedCommunityId(data[0].id);
        }
      } catch (err) {
        console.error("Could not fetch communities", err);
      }
    }
    fetchCommunities();
  }, []);

  const handleMapChange = (lat: number, lng: number) => {
    setLatitude(lat);
    setLongitude(lng);
  };

  async function onSubmit(e: any) {
    e.preventDefault();
    if (!selectedCommunityId) {
        toast.error("You must pick a community!");
        return;
    }
    if (latitude === null || longitude === null) {
        toast.error("You must select a location on the map!");
        return;
    }
    if (!startsAt) {
        toast.error("Event starting time must be selected!");
        return;
    }

    try {
      setIsLoading(true);

      const payload = {
        CommunityId: selectedCommunityId, 
        Name: name.trim(),
        Description: description.trim(),
        CategoryId: "00000000-0000-0000-0000-000000000001", 
        PresetId: null,
        IsPublic: true,
        Latitude: latitude,
        Longitude: longitude,
        StartsAt: startsAt.toISOString(),
        EstimatedEndsAt: estimatedEndsAt ? estimatedEndsAt.toISOString() : null,
        EcoPointsPerParticipant: parseInt(ecoPoints)
      };

      const response = await fetch(`${API_BASE_URL}/Initiative/create`, {
        method: "POST",
        credentials: "include",
        body: JSON.stringify(payload),
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (response.ok) {
        toast.success("Initiative created!");
        router.push("/home");
      } else {
        const json = await response.json();
        throw new Error(json.detail || "Failed to create initiative");
      }
    } catch (error: any) {
      toast.error(error.message);
      console.log(error);
    } finally {
      setIsLoading(false);
    }
  }

  
  const datePickerClassName = "flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 ";

  return (
    <div className="flex min-h-screen items-center justify-center p-4">
      <Card className="w-full max-w-md shadow-2xl">
        <CardHeader>
          <CardTitle>Create Initiative</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={onSubmit} className="space-y-4">
            <div>
              <Label>Select Community</Label>
              <select 
                className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                value={selectedCommunityId} 
                onChange={(e) => setSelectedCommunityId(e.target.value)}
                required
              >
                <option value="">Choose a community...</option>
                {communities.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
            </div>
            
            <div>
              <Label>Name</Label>
              <Input value={name} onChange={(e) => setName(e.target.value)} required />
            </div>

            <div>
              <Label>Description</Label>
              <Input value={description} onChange={(e) => setDescription(e.target.value)} required />
            </div>

           <div className="space-y-2">
              <Label>Select a location on the map</Label>
              <MapInput lat={latitude} lng={longitude} onChange={handleMapChange} />
              {latitude && longitude && (
                <p className="text-xs text-muted-foreground mt-1">
                  Selected coordinates: {latitude.toFixed(5)}, {longitude.toFixed(5)}
                </p>
              )}
            </div>

            
            <div className="grid grid-cols-2 gap-4">
              <div className="flex flex-col space-y-2">
                <Label>Starts At</Label>
                <DatePicker
                  selected={startsAt}
                  onChange={(date) => setStartsAt(date)}
                  showTimeSelect
                  timeFormat="HH:mm"
                  timeIntervals={15}
                  timeCaption="Time"
                  dateFormat="yyyy-MM-dd HH:mm"
                  locale="sv"
                  required
                  className={datePickerClassName}
                  wrapperClassName="w-full"
                  calendarStartDay={1}
                />
              </div>

              <div className="flex flex-col space-y-2">
                <Label>Estimated Ends At</Label>
                <DatePicker
                  selected={estimatedEndsAt}
                  onChange={(date) => setEstimatedEndsAt(date)}
                  showTimeSelect
                  timeFormat="HH:mm"
                  timeIntervals={15}
                  timeCaption="Time"
                  dateFormat="yyyy-MM-dd HH:mm"
                  locale="sv"
                  isClearable 
                  minDate={startsAt || new Date()}
                  className={datePickerClassName}
                  wrapperClassName="w-full"
                  placeholderText="Event ends at..."
                  calendarStartDay={1}
                />
              </div>
            </div>
            
            <div>
              <Label>Eco Points</Label>
              <Input value={ecoPoints} onChange={(e) => setEcoPoints(e.target.value)} required />
            </div>

            <Button type="submit" disabled={isLoading} className="w-full">
              Create
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}