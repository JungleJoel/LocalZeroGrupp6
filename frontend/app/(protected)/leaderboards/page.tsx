"use client";

import {Table, TableBody, TableCell, TableHead, TableHeader, TableRow,} from "@/components/ui/table";
import { CommunityDTO } from "@/types/communityDTO";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { API_BASE_URL } from "@/lib/config";
import LoadingSpinner from "@/components/LoadingSpinner";
import { Trophy } from "lucide-react";

export default function CommunityLeaderboard() {
  const [communities, setCommunities] = useState<CommunityDTO[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchCommunities = async () => {
      try {
        const res = await fetch(`${API_BASE_URL}/Community/getCommunities`, {
            credentials: "include",
        });
        if (!res.ok) throw new Error("Failed to fetch communities");
        const data = await res.json();
        const sorted = [...data].sort((a, b) => b.ecoPoints - a.ecoPoints);
        setCommunities(sorted);
      } catch {
        toast.error("Kunde inte hämta communities");
      } finally {
        setLoading(false);
      }
    };

    fetchCommunities();
  }, []);

  const getMedalColor = (index: number) => {
    if (index === 0) return "text-yellow-400";
    if (index === 1) return "text-gray-400";
    if (index === 2) return "text-amber-600";
    return "text-muted-foreground";
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="rounded-xl border bg-card p-4">
      <div className="flex items-center gap-2 mb-4">
        <Trophy className="text-yellow-400" size={20} />
        <h2 className="text-lg font-semibold">Leaderboard</h2>
      </div>

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead className="w-12">#</TableHead>
            <TableHead>Community</TableHead>
            <TableHead className="text-right">Deltagare</TableHead>
            <TableHead className="text-right">EcoPoints</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {communities.map((community, index) => (
            <TableRow key={community.id}>
              <TableCell>
                <span className={`font-bold ${getMedalColor(index)}`}>
                  {index + 1}
                </span>
              </TableCell>
              <TableCell className="font-medium">{community.name}</TableCell>
              <TableCell className="text-right">{community.residentsCount}</TableCell>
              <TableCell className="text-right font-semibold text-green-500">
                {community.ecoPoints}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}