"use client";

import { GetMyCommunityResponseDTO } from "@/types/getMyCommunityResponseDTO";
import { useEffect, useState } from "react";
import { API_BASE_URL } from "@/lib/config";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";
import { EcoPointsCard } from "./EcoPointsCard";
import { NotAMember } from "./NotAMember";

export default function Community() {
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [hasNotJoinedYet, setHasNotJoinedYet] = useState<boolean>(false);
  const [apiResponse, setApiResponse] =
    useState<GetMyCommunityResponseDTO | null>(null);

  async function getMyCommunity() {
    try {
      setIsLoading(true);

      const response = await fetch(`${API_BASE_URL}/Community/my-community`, {
        method: "GET",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (response.ok) {
        setApiResponse(await response.json());
      } else {
        if (response.status == 404) {
          setHasNotJoinedYet(true);
        } else {
          const json = await response.json();
          throw new Error(json.detail || "Could not get community");
        }
      }
    } catch (error: any) {
      toast.error(error.message);
      console.log(error);
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    getMyCommunity();
  }, []);

  if (isLoading) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <Loader2 className="h-12 w-12 animate-spin text-primary" />
      </div>
    );
  }

  if (hasNotJoinedYet) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <NotAMember />
      </div>
    );
  }

  if (apiResponse == null) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <h1>Error</h1>
      </div>
    );
  }

  return (
    <div className="flex justify-center px-6 pt-12">
      <div className="w-full max-w-4xl">
        <div className="mb-3 flex flex-row items-center gap-3">
          <h1 className="text-3xl font-semibold sm:text-4xl">
            {apiResponse.community.name}
          </h1>
          <p className="hidden text-muted-foreground sm:block">
            You are a {apiResponse.isCommunityManager ? "manager" : "resident"}{" "}
            of this community
          </p>
        </div>
        <EcoPointsCard community={apiResponse.community} />
      </div>
    </div>
  );
}
