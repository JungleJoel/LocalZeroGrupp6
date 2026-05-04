"use client";
import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { API_BASE_URL } from "@/lib/config";
import { MapPin, Calendar, Coins, Loader2 } from "lucide-react"; 
import { toast } from "sonner";

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
          setInitiatives(data);
        } else {
          throw new Error("Kunde inte hämta initiativ");
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
      <h1 className="text-3xl font-bold mb-6">Alla Initiativ</h1>
      
      {initiatives.length === 0 ? (
        <p className="text-muted-foreground">Inga initiativ hittades i databasen.</p>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {initiatives.map((item) => (
            <Card key={item.id} className="hover:shadow-lg transition-shadow">
              <CardHeader>
                <CardTitle>{item.name}</CardTitle>
                <CardDescription className="line-clamp-2">
                  {item.description}
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                
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

                <div className="flex items-center text-sm gap-2">
                  <MapPin className="h-4 w-4 text-red-500" />
                  <span className="text-muted-foreground">
                    Lat: {item.latitude.toFixed(2)}, Long: {item.longitude.toFixed(2)}
                  </span>
                </div>

                <div className="flex items-center text-sm gap-2 font-medium">
                  <Coins className="h-4 w-4 text-yellow-500" />
                  <span>{item.ecoPointsPerParticipant} Eco Points</span>
                </div>

                {item.isPublic ? (
                  <span className="inline-flex items-center rounded-full bg-green-100 px-2.5 py-0.5 text-xs font-medium text-green-800">
                    Offentlig
                  </span>
                ) : (
                  <span className="inline-flex items-center rounded-full bg-gray-100 px-2.5 py-0.5 text-xs font-medium text-gray-800">
                    Privat
                  </span>
                )}
                
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}