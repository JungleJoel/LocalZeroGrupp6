"use client";

import { CommunityDTO } from "@/types/communityDTO";
import { MapPin, Users, Leaf, ArrowRight, Clock, X } from "lucide-react";
import { Button } from "@/components/ui/button";

interface CommunityCardProps {
  community: CommunityDTO;
  pendingRequestCommunityId: string | null;
  onJoin?: (community: CommunityDTO) => void;
  onCancel?: (community: CommunityDTO) => void;
}

export function CommunityCard({
  community,
  pendingRequestCommunityId,
  onJoin,
  onCancel,
}: CommunityCardProps) {
  const isPendingForThis = pendingRequestCommunityId === community.id;
  const hasPendingElsewhere =
    pendingRequestCommunityId !== null && !isPendingForThis;

  return (
    <div className="group relative overflow-hidden rounded-xl border border-border bg-card p-5">
      <div className="flex flex-col gap-4">
        {/* Header */}
        <div className="flex items-start justify-between gap-3">
          <div className="flex items-center gap-3">
            <div>
              <h2 className="text-lg font-semibold leading-tight text-foreground">
                {community.name}
              </h2>
              {community.latitude != null && community.longitude != null && (
                <div className="mt-0.5 flex items-center gap-1 text-xs text-muted-foreground">
                  <MapPin className="h-3 w-3" />
                  <span>
                    {community.latitude.toFixed(3)},{" "}
                    {community.longitude.toFixed(3)}
                  </span>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Stats row */}
        <div className="flex items-center gap-3">
          <div className="flex items-center gap-1.5 rounded-full bg-primary/10 px-3 py-1.5">
            <Leaf className="h-3.5 w-3.5 text-primary" />
            <span className="text-xs font-semibold tabular-nums text-primary">
              {community.ecoPoints.toLocaleString()} eco points
            </span>
          </div>

          <div className="flex items-center gap-1.5 rounded-full bg-muted px-3 py-1.5">
            <Users className="h-3.5 w-3.5 text-muted-foreground" />
            <span className="text-xs font-medium tabular-nums text-muted-foreground">
              {community.residentsCount.toLocaleString()}{" "}
              {community.residentsCount === 1 ? "resident" : "residents"}
            </span>
          </div>
        </div>

        {/* Footer / CTA */}
        <div className="flex items-center justify-end border-t border-border pt-3">
          {isPendingForThis ? (
            <div className="flex items-center gap-2">
              <span className="flex items-center gap-1 text-xs text-muted-foreground">
                <Clock className="h-3.5 w-3.5" />
                Request pending
              </span>
              <Button
                size="sm"
                variant="ghost"
                className="h-8 gap-1.5 text-xs text-destructive hover:bg-destructive/10 hover:text-destructive"
                onClick={() => onCancel?.(community)}
              >
                <X className="h-3.5 w-3.5" />
                Cancel
              </Button>
            </div>
          ) : (
            <Button
              size="sm"
              variant="ghost"
              className="h-8 gap-1.5 text-xs text-primary hover:bg-primary/10 hover:text-primary"
              onClick={() => onJoin?.(community)}
              disabled={hasPendingElsewhere}
              title={
                hasPendingElsewhere
                  ? "Cancel your other pending request first"
                  : undefined
              }
            >
              Request to join
              <ArrowRight className="h-3.5 w-3.5" />
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
