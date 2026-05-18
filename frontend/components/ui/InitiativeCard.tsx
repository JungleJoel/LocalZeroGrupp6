"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { API_BASE_URL } from "@/lib/config";
import { Calendar, Coins, CheckCircle2, Users } from "lucide-react"; 
import dynamic from "next/dynamic";

const StaticMiniMap = dynamic(() => import("@/components/ui/staticminimap"), {
  ssr: false,
  loading: () => <div className="h-32 w-full bg-muted animate-pulse rounded-md" />
});

interface InitiativeCardProps {
  item: any;
}

export default function InitiativeCard({ item }: InitiativeCardProps) {
  const [userCount, setUserCount] = useState<number>(0);
  const isFinished = item.endedAt !== null;

  useEffect(() => {
    async function fetchParticipants() {
      try {
        const response = await fetch(`${API_BASE_URL}/Initiative/${item.id}/participants`, {
          credentials: "include",
        });
        if (response.ok) {
          const participants = await response.json();
          console.log("Detta kommer tillbaka från API/Initiative/id/blablabla:", participants);
          setUserCount(participants.length);
        }
      } catch (error) {
        console.error(`Kunde inte hämta deltagare för initiativ ${item.id}:`, error);
      }
    }

    if (item.id) {
      fetchParticipants();
    }
  }, [item.id]);

  return (
    <Card className={`hover:shadow-lg transition-shadow flex flex-col justify-between ${isFinished ? "opacity-80" : ""}`}>
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
            
            <div className="flex items-center text-sm gap-2 text-muted-foreground">
              <Users className="h-4 w-4 text-emerald-500" />
              <span>
                {isFinished 
                  ? `${userCount} user${userCount === 1 ? "" : "s"} helped` 
                  : `${userCount} user${userCount === 1 ? "" : "s"} are going`}
              </span>
            </div>
          </div>
        </CardContent>
      </div>

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
}