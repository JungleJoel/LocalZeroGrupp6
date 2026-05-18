"use client";

import { useEffect, useState } from "react";
import { API_BASE_URL } from "@/lib/config";
import { InitiativeDTO } from "@/types/initiativeDTO";
import { toast } from "sonner";
import { Calendar, Leaf, ArrowRight, Plus, Users } from "lucide-react";
import Link from "next/link";
import { Button } from "@/components/ui/button";

interface CommunityInitiativesProps {
  communityId: string;
}

function getStatus(initiative: InitiativeDTO): "active" | "upcoming" {
  return new Date(initiative.startsAt) <= new Date() ? "active" : "upcoming";
}

function ParticipantCount({ initiativeId }: { initiativeId: string }) {
  const [count, setCount] = useState<number>(0);

  useEffect(() => {
    async function fetchCount() {
      try {
        const response = await fetch(`${API_BASE_URL}/Initiative/${initiativeId}/participants`, {
          credentials: "include",
        });
        if (response.ok) {
          const data = await response.json();
          setCount(data.length);
        }
      } catch (error) {
        console.error("could not fetch participants:", error);
      }
    }
    if (initiativeId) fetchCount();
  }, [initiativeId]);

  return (
    <div className="flex items-center gap-1">
      <Users className="h-3.5 w-3.5 text-emerald-500" />
      <span>{count} {count === 1 ? "user" : "users"}</span>
    </div>
  );
}

export function CommunityInitiatives({ communityId }: CommunityInitiativesProps) {
  const [initiatives, setInitiatives] = useState<InitiativeDTO[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function fetchInitiatives() {
      try {
        const response = await fetch(
          `${API_BASE_URL}/Initiative/community/${communityId}`,
          { credentials: "include" }
        );
        if (response.ok) {
          setInitiatives(await response.json());
        } else {
          throw new Error("Could not load initiatives");
        }
      } catch (error: any) {
        toast.error(error.message);
      } finally {
        setIsLoading(false);
      }
    }
    fetchInitiatives();
  }, [communityId]);

  return (
    <div className="mt-8">
      <div className="mb-4 flex items-center justify-between">
  <h2 className="text-xl font-semibold">Available initiatives</h2>
  <Link href={`/createInitiatives?communityId=${communityId}`}>
    <Button className="h-7"><Plus className="h-4 w-4" /> Create initiative</Button> 
  </Link>
</div>

      {isLoading ? (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {[1, 2, 3].map((i) => (
            <div
              key={i}
              className="h-36 animate-pulse rounded-xl bg-muted"
            />
          ))}
        </div>
      ) : initiatives.length === 0 ? (
        <div className="flex flex-col items-center justify-center rounded-xl border border-dashed py-12 text-center text-muted-foreground">
          <Leaf className="mb-3 h-8 w-8 opacity-40" />
          <p className="text-sm">No active initiatives in your community yet.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {initiatives.map((initiative) => {
            const status = getStatus(initiative);
            return (
              <Link
                key={initiative.id}
                href={`/community/initiative/${initiative.id}`}
                className="flex flex-col gap-3 rounded-xl border bg-card p-5 shadow-sm transition-shadow hover:shadow-md"
              >
                <div className="flex items-start justify-between gap-2">
                  <h3 className="font-semibold leading-tight">{initiative.name}</h3>
                  <span
                    className={`shrink-0 rounded-full px-2.5 py-0.5 text-xs font-medium ${
                      status === "active"
                        ? "bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400"
                        : "bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400"
                    }`}
                  >
                    {status === "active" ? "Active" : "Upcoming"}
                  </span>
                </div>

                <p className="line-clamp-2 text-sm text-muted-foreground">
                  {initiative.description}
                </p>

                <div className="mt-auto flex items-center justify-between text-xs text-muted-foreground">
                  <div className="flex items-center gap-1">
                    <Calendar className="h-3.5 w-3.5" />
                    <span>
                      {new Date(initiative.startsAt).toLocaleDateString("en-SE", {
                        day: "numeric",
                        month: "short",
                        year: "numeric",
                      })}
                    </span>
                  </div>
                  <ParticipantCount initiativeId={initiative.id} />
                  <div className="flex items-center gap-1 font-medium text-primary">
                    <Leaf className="h-3.5 w-3.5" />
                    <span>{initiative.ecoPointsPerParticipant} pts</span>
                  </div>
                </div>
              </Link>
            );
          })}
        </div>
      )}
    </div>
  );
}
