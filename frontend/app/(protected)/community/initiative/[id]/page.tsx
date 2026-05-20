"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { API_BASE_URL } from "@/lib/config";
import { InitiativeDTO } from "@/types/initiativeDTO";
import { toast } from "sonner";
import {
  ArrowLeft,
  Calendar,
  CalendarCheck,
  Leaf,
  MapPin,
  Lock,
  Globe,
  Loader2,
  Share2, 
  Ban
} from "lucide-react";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import LoadingSpinner from "@/components/LoadingSpinner";

function getStatus(initiative: InitiativeDTO): "active" | "upcoming" | "ended" {
  if (initiative.endedAt) return "ended";
  return new Date(initiative.startsAt) <= new Date() ? "active" : "upcoming";
}

function formatDate(dateStr: string | null) {
  if (!dateStr) return null;
  return new Date(dateStr).toLocaleDateString("en-SE", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });
}

export default function InitiativePage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [initiative, setInitiative] = useState<InitiativeDTO | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isJoined, setIsJoined] = useState(false);
  const [isJoinLoading, setIsJoinLoading] = useState(false);
  const [locationName, setLocationName] = useState<string | null>(null);

  useEffect(() => {
    async function fetchInitiative() {
      try {
        const response = await fetch(`${API_BASE_URL}/Initiative/${id}`, {
          credentials: "include",
        });
        if (response.ok) {
          const data : InitiativeDTO = await response.json();
          setInitiative(data);
          setIsJoined(data.isParticipating);
        } else {
          throw new Error("Could not load initiative");
        }
      } catch (error: any) {
        toast.error(error.message);
      } finally {
        setIsLoading(false);
      }
    }
    fetchInitiative();
  }, [id]);

   useEffect(() => {
  if (!initiative) return;
  async function fetchLocationName() {
    try {
      const response = await fetch(
        `https://nominatim.openstreetmap.org/reverse?lat=${initiative!.latitude}&lon=${initiative!.longitude}&format=json`,
        { headers: { "Accept-Language": "sv" } }
      );
      if (response.ok) {
        const data = await response.json();
        console.log("shit i recieve",data);
        setLocationName([data.address.road, data.address.city].filter(Boolean).join(", ") ?? null);
      }
    } catch {}
  }
  fetchLocationName();
}, [initiative]);


  if (isLoading) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <LoadingSpinner />
      </div>
    );
  }

  if (!initiative) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <p className="text-muted-foreground">Initiative not found.</p>
      </div>
    );
  }

 
  
  async function handleJoinLeave() {
    setIsJoinLoading(true);
    try {
      const response = await fetch(
        `${API_BASE_URL}/Initiative/${id}/${isJoined ? "leave" : "join"}`,
        {
          method: isJoined ? "DELETE" : "POST", // leave=DELETE, join=POST
          credentials: "include",
        }
      );
      if (!response.ok) throw new Error("Action failed");
      setIsJoined(!isJoined);
      toast.success(isJoined ? "Left initiative" : "Joined initiative");
    } catch (error: any) {
      toast.error(error.message);
    } finally {
      setIsJoinLoading(false);
    }
  }

  const status = getStatus(initiative);

  return (
    <div className="flex justify-center px-6 pt-12">
      <div className="w-full max-w-4xl">
        <div className="mb-6 flex items-center gap-3">
          <Button
            variant="ghost"
            size="sm"
            className="gap-1.5 text-muted-foreground"
            onClick={() => router.back()}
          >
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </div>

        {/* Title & Status */}
        <div className="mb-3 flex flex-wrap items-center gap-3">
          <h1 className="text-3xl font-semibold sm:text-4xl">
            {initiative.name}
          </h1>
          <span
            className={`rounded-full px-3 py-1 text-sm font-medium ${
              status === "active"
                ? "bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400"
                : status === "upcoming"
                  ? "bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400"
                  : "bg-muted text-muted-foreground"
            }`}
          >
            {status === "active"
              ? "Active"
              : status === "upcoming"
                ? "Upcoming"
                : "Ended"}
          </span>

          {status !== "ended" && (
            <Button
              variant={isJoined ? "outline" : "default"}
              onClick={handleJoinLeave}
              disabled={isJoinLoading}
            >
              {isJoinLoading ? (
                <Loader2 className="h-4 w-4 animate-spin" />
              ) : isJoined ? (
                "Leave"
              ) : (
                "Join"
              )}
            </Button>
          )}

          <Button style={{ marginLeft: "auto" }}>
            {status !== "ended" ? (
              <a
                href={`https://www.google.com/maps/search/?api=1&query=${initiative.latitude},${initiative.longitude}`}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center gap-2"
              >
                <MapPin className="h-4 w-4" />
                View on map
              </a>
            ) : (
              <a>Map not Available</a>
            )}
          </Button>
        </div>

        {/* Details */}
        <div className="mt-6 grid grid-cols-1 items-start gap-4 sm:grid-cols-2">
          {/* Description */}
          <div className="col-span-full rounded-xl border bg-card p-5 shadow-sm">
            <h2 className="mb-2 text-sm font-medium text-muted-foreground">
              About this initiative
            </h2>
            <p className="leading-relaxed">{initiative.description}</p>
          </div>

          {/* Dates */}
          <div className="rounded-xl border bg-card p-5 shadow-sm">
            <h2 className="mb-3 text-sm font-medium text-muted-foreground">
              Dates
            </h2>
            <div className="flex flex-col gap-3">
              <div className="flex items-center gap-3">
                <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                  <Calendar className="h-4 w-4 text-primary" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Starts</p>
                  <p className="font-medium">
                    {formatDate(initiative.startsAt)}
                  </p>
                </div>
              </div>
              {initiative.estimatedEndsAt && (
                <div className="flex items-center gap-3">
                  <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                    <CalendarCheck className="h-4 w-4 text-primary" />
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground">
                      Estimated end
                    </p>
                    <p className="font-medium">
                      {formatDate(initiative.estimatedEndsAt)}
                    </p>
                  </div>
                </div>
              )}
              {initiative.endedAt && (
                <div className="flex items-center gap-3">
                  <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-muted">
                    <CalendarCheck className="h-4 w-4 text-muted-foreground" />
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground">Ended</p>
                    <p className="font-medium">
                      {formatDate(initiative.endedAt)}
                    </p>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* Location & visibility */}
          <div className="rounded-xl border bg-card p-5 shadow-sm">
            <h2 className="mb-3 text-sm font-medium text-muted-foreground">
              Details
            </h2>
            <div className="flex flex-col gap-3">
              <div className="flex items-center gap-3">
                <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                  <MapPin className="h-4 w-4 text-primary" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Location</p>
                  <p className="font-medium">
                    {locationName ?? `${initiative.latitude.toFixed(4)}, ${initiative.longitude.toFixed(4)}`}
                  </p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                  {initiative.isPublic ? (
                    <Globe className="h-4 w-4 text-primary" />
                  ) : (
                    <Lock className="h-4 w-4 text-primary" />
                  )}
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Visibility</p>
                  <p className="font-medium">
                    {initiative.isPublic ? "Public" : "Community only"}
                  </p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                  <Leaf className="h-4 w-4 text-primary" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">
                    Eco points per participant
                  </p>
                  <p className="font-medium">
                    {initiative.ecoPointsPerParticipant} pts
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
