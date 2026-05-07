'use client';

import { CommunityDTO } from "@/types/communityDTO";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { API_BASE_URL } from "@/lib/config";
import LoadingSpinner from "@/components/LoadingSpinner";
import { CommunityCard } from "./CommunityCard";
import { Search } from "lucide-react";
import { Input } from "@/components/ui/input";

export default function SearchCommunity() {
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [communities, setCommunities] = useState<CommunityDTO[]>([]);
  const [query, setQuery] = useState<string>("");

  async function getAllCommunities() {
    try {
      setIsLoading(true);

      const response = await fetch(`${API_BASE_URL}/Community/getCommunities`, {
        method: "GET",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (response.ok) {
        setCommunities(await response.json());
      } else {
        const json = await response.json();
        throw new Error(json.detail || "Could not get communities");
      }
    } catch (error: any) {
      toast.error(error.message);
      console.log(error);
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    getAllCommunities();
  }, []);

  const filtered = communities.filter((c) =>
    c.name.toLowerCase().includes(query.toLowerCase())
  );

  if (isLoading) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <LoadingSpinner />
      </div>
    );
  }

  return (
    <div className="flex justify-center px-6 pt-12">
      <div className="w-full max-w-4xl">
        {/* Header */}
        <div className="mb-6">
          <h1 className="text-3xl font-semibold sm:text-4xl">
            Find your community
          </h1>
          <p className="mt-1 text-muted-foreground">
            Join a community and start earning eco points together.
          </p>
        </div>

        {/* Search input */}
        <div className="relative mb-6">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            className="pl-9"
            placeholder="Search by name…"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
          />
        </div>

        {/* Grid */}
        {filtered.length === 0 ? (
          <div className="flex flex-col items-center gap-2 py-20 text-muted-foreground">
            <Search className="h-8 w-8 opacity-40" />
            <p className="text-sm">No communities found.</p>
          </div>
        ) : (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {filtered.map((community) => (
              <CommunityCard
                key={community.id}
                community={community}
                onJoin={(c) => alert("not implemented")}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
