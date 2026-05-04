"use client";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { API_BASE_URL } from "@/lib/config";
import { toast } from "sonner";
import { useRouter } from "next/navigation";


export default function CreateInitiative() {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [latitude, setLatitude] = useState("");
  const [longitude, setLongitude] = useState("");
  const [startsAt, setStartsAt] = useState("");
  const [estimatedEndsAt, setEstimatedEndsAt] = useState("");
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
        console.error("Kunde inte hämta communities", err);
      }
    }
    fetchCommunities();
  }, []);

  async function onSubmit(e: any) {
    e.preventDefault();
    if (!selectedCommunityId) {
        toast.error("Du måste välja en community!");
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
     Latitude: parseFloat(latitude),
     Longitude: parseFloat(longitude),
     StartsAt: new Date(startsAt).toISOString(),
     EstimatedEndsAt: estimatedEndsAt ? new Date(estimatedEndsAt).toISOString() : null,
     EcoPointsPerParticipant: parseInt(ecoPoints)
    };

      const response = await fetch(`${API_BASE_URL}/Initiative`, {
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

            <div>
              <Label>Latitude</Label>
              <Input value={latitude} onChange={(e) => setLatitude(e.target.value)} required />
            </div>

            <div>
              <Label>Longitude</Label>
              <Input value={longitude} onChange={(e) => setLongitude(e.target.value)} required />
            </div>

            <div>
              <Label>Starts At</Label>
              <Input type="datetime-local" value={startsAt} onChange={(e) => setStartsAt(e.target.value)} required />
            </div>

            <div>
              <Label>Estimated Ends At</Label>
              <Input type="datetime-local" value={estimatedEndsAt} onChange={(e) => setEstimatedEndsAt(e.target.value)} />
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