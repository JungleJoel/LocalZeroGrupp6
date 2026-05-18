"use client";

import { useState, useEffect } from "react";
import { API_BASE_URL } from "@/lib/config";
import { MapPin, Calendar, Coins, Loader2, CheckCircle2 } from "lucide-react"; 
import { toast } from "sonner";
import InitiativeCard from "@/components//ui/InitiativeCard";

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
          {initiatives.map((item) => (
            <InitiativeCard key={item.id} item={item} />
          ))}
        </div>
      )}
    </div>
  );
}