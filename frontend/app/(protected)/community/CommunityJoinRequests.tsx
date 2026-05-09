"use client";

import { CommunityJoinRequestWithUserDTO } from "@/types/communityJoinRequestWithUserDTO";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { API_BASE_URL } from "@/lib/config";
import { Button } from "@/components/ui/button";
import { Check, X, Users } from "lucide-react";

interface CommunityJoinRequestsProps {
  communityId: string;
}

export function CommunityJoinRequests({
  communityId,
}: CommunityJoinRequestsProps) {
  const [requests, setRequests] = useState<CommunityJoinRequestWithUserDTO[]>(
    []
  );
  const [isLoading, setIsLoading] = useState(true);
  const [processingId, setProcessingId] = useState<string | null>(null);

  async function fetchRequests() {
    try {
      setIsLoading(true);
      const response = await fetch(
        `${API_BASE_URL}/Community/${communityId}/get-requests`,
        {
          method: "GET",
          credentials: "include",
          headers: { "Content-Type": "application/json" },
        }
      );

      if (response.ok) {
        setRequests(await response.json());
      } else {
        const json = await response.json();
        toast.error(json.detail || "Could not load join requests");
      }
    } catch {
      toast.error("Could not load join requests");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    fetchRequests();
  }, [communityId]);

  async function handleApprove(request: CommunityJoinRequestWithUserDTO) {
    try {
      setProcessingId(request.id);
      const response = await fetch(
        `${API_BASE_URL}/Community/${communityId}/approve-request/${request.id}`,
        {
          method: "POST",
          credentials: "include",
          headers: { "Content-Type": "application/json" },
        }
      );

      if (response.ok) {
        setRequests((prev) => prev.filter((r) => r.id !== request.id));
        toast.success(
          `${request.userFirstName} ${request.userLastName} has been approved`
        );
      } else {
        const json = await response.json();
        toast.error(json.detail || "Could not approve request");
      }
    } catch {
      toast.error("Could not approve request");
    } finally {
      setProcessingId(null);
    }
  }

  async function handleDecline(request: CommunityJoinRequestWithUserDTO) {
    try {
      setProcessingId(request.id);
      const response = await fetch(
        `${API_BASE_URL}/Community/${communityId}/decline-request/${request.id}`,
        {
          method: "POST",
          credentials: "include",
          headers: { "Content-Type": "application/json" },
        }
      );

      if (response.ok) {
        setRequests((prev) => prev.filter((r) => r.id !== request.id));
        toast.success(
          `${request.userFirstName} ${request.userLastName}'s request has been declined`
        );
      } else {
        const json = await response.json();
        toast.error(json.detail || "Could not decline request");
      }
    } catch {
      toast.error("Could not decline request");
    } finally {
      setProcessingId(null);
    }
  }

  return (
    <div className="mt-8">
      <div className="mb-4 flex items-center gap-2">
        <Users className="h-5 w-5 text-muted-foreground" />
        <h2 className="text-xl font-semibold">Join requests</h2>
        {!isLoading && requests.length > 0 && (
          <span className="rounded-full bg-primary px-2 py-0.5 text-xs font-medium text-primary-foreground">
            {requests.length}
          </span>
        )}
      </div>

      {isLoading ? (
        <p className="text-sm text-muted-foreground">Loading requests…</p>
      ) : requests.length === 0 ? (
        <p className="text-sm text-muted-foreground">No pending join requests.</p>
      ) : (
        <div className="flex flex-col gap-3">
          {requests.map((request) => (
            <div
              key={request.id}
              className="flex items-center justify-between rounded-lg border border-border bg-card px-4 py-3"
            >
              <div>
                <p className="font-medium">
                  {request.userFirstName} {request.userLastName}
                </p>
                <p className="text-xs text-muted-foreground">
                  Requested{" "}
                  {new Date(request.createdAt).toLocaleDateString(undefined, {
                    day: "numeric",
                    month: "short",
                    year: "numeric",
                  })}
                </p>
              </div>
              <div className="flex gap-2">
                <Button
                  size="sm"
                  variant="outline"
                  className="h-8 gap-1.5 text-xs text-destructive hover:bg-destructive/10 hover:text-destructive"
                  onClick={() => handleDecline(request)}
                  disabled={processingId === request.id}
                >
                  <X className="h-3.5 w-3.5" />
                  Decline
                </Button>
                <Button
                  size="sm"
                  className="h-8 gap-1.5 text-xs"
                  onClick={() => handleApprove(request)}
                  disabled={processingId === request.id}
                >
                  <Check className="h-3.5 w-3.5" />
                  Approve
                </Button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
