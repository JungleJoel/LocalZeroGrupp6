"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { API_BASE_URL } from "@/lib/config";
import { MapPin, Calendar, Coins, Loader2, CheckCircle2 } from "lucide-react"; 
import { toast } from "sonner";
import dynamic from "next/dynamic";

const StaticMiniMap = dynamic(() => import("@/components/ui/staticminimap"), {
  ssr: false,
  loading: () => <div className="h-32 w-full bg-muted animate-pulse rounded-md" />
});

export default function ListInitiatives() {
  const [initiatives, setInitiatives] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
  async function fetchInitiatives() {
    try {
      setIsLoading(true);
      const response = await fetch(`${API_BASE_URL}/Initiative`, {
        credentials: "include",
      });

      if (response.ok) {
        const data = await response.json();

        // Sort initiatives first
        const sortedData = data.sort((a: any, b: any) => {
          const aFinished = a.endedAt !== null;
          const bFinished = b.endedAt !== null;

          if (!aFinished && bFinished) return -1;
          if (aFinished && !bFinished) return 1;  

          if (!aFinished && !bFinished) {
            const aEndTime = a.estimatedEndsAt ? new Date(a.estimatedEndsAt).getTime() : Infinity;
            const bEndTime = b.estimatedEndsAt ? new Date(b.estimatedEndsAt).getTime() : Infinity;
            
            return aEndTime - bEndTime; 
          }
          if (aFinished && bFinished) {
            const aEndedTime = new Date(a.endedAt).getTime();
            const bEndedTime = new Date(b.endedAt).getTime();
            return bEndedTime - aEndedTime;
          }

          return 0;
        });

        setInitiatives(sortedData);
      } else {
        throw new Error("Could not fetch initiatives");
      }
    } catch (error: any) {
      toast.error(error.message);
      console.error(error);
    } finally {
      setIsLoading(false);
    }
  }

  fetchInitiatives();
}, []);

  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold mb-6">All Initiatives</h1>
      
      {initiatives.length === 0 ? (
        <p className="text-muted-foreground">No initiatives found in the database.</p>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {initiatives.map((item) => {
            const isFinished = item.endedAt !== null;

            return (
              <Card key={item.id} className={`hover:shadow-lg transition-shadow flex flex-col justify-between ${isFinished ? "opacity-80" : ""}`}>
                <div>
                  <CardHeader>
                    <div className="flex justify-between items-start gap-2">
                      <CardTitle className="text-xl font-bold leading-tight">{item.name}</CardTitle>
                      {isFinished && (
                        <span className="inline-flex items-center gap-1 shrink-0 rounded-full bg-red-100 px-2.5 py-0.5 text-xs font-bold text-red-800 border border-red-200">
                          <CheckCircle2 className="h-3 w-3" />
                          Finished
                        </span>
                      )}
                    </div>
                    <CardDescription className="line-clamp-2">
                      {item.description}
                    </CardDescription>
                  </CardHeader>
                  
                  <CardContent className="space-y-4">
                    <StaticMiniMap lat={item.latitude} lng={item.longitude} />

                    <div className="space-y-2">
                      <div className="flex items-center text-sm gap-2">
                        <Calendar className="h-4 w-4 text-blue-500" />
                        <span>
                          {new Date(item.startsAt).toLocaleDateString("sv-SE", {
                            day: "numeric",
                            month: "long",
                            year: "numeric",
                            hour: "2-digit",
                            minute: "2-digit",
                          })}
                        </span>
                      </div>

                      <div className="flex items-center text-sm gap-2 font-medium">
                        <Coins className="h-4 w-4 text-yellow-500" />
                        <span>{item.ecoPointsPerParticipant} Eco Points</span>
                      </div>
                    </div>
                  </CardContent>
                </div>

                {/* Lägger badgearna i botten på kortet för en renare look */}
                <div className="px-6 pb-4">
                  {!isFinished && (
                    item.isPublic ? (
                      <span className="inline-flex items-center rounded-full bg-green-100 px-2.5 py-0.5 text-xs font-medium text-green-800 dark:bg-green-900/30 dark:text-green-400">
                        Public
                      </span>
                    ) : (
                      <span className="inline-flex items-center rounded-full bg-gray-100 px-2.5 py-0.5 text-xs font-medium text-gray-800 dark:bg-gray-800 dark:text-gray-400">
                        Private
                      </span>
                    )
                  )}
                </div>
              </Card>
            );
          })}
        </div>
      )}
    </div>
  );
}