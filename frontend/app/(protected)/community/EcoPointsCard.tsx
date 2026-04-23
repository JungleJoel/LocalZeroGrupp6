"use client";

import { CommunityDTO } from "@/types/communityDTO";
import { Leaf } from "lucide-react";

interface EcoPointsCardProps {
  community: CommunityDTO;
}

export function EcoPointsCard({ community }: EcoPointsCardProps) {
  return (
    <div className="relative overflow-hidden rounded-xl bg-gradient-to-r from-primary to-primary/85 p-6 shadow-lg">
      <div className="absolute inset-0 bg-[radial-gradient(circle_at_30%_50%,rgba(255,255,255,0.1),transparent_50%)]" />

      <div className="relative flex items-center justify-between gap-6">
        <div className="flex items-center gap-4">
          <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-full bg-white/20 backdrop-blur-sm">
            <Leaf className="h-7 w-7 text-white" />
          </div>

          <div className="flex flex-col">
            <span className="text-md font-medium text-emerald-100">
              Eco Points
            </span>
            <span className="text-3xl font-bold tracking-tight text-white tabular-nums">
              {community.ecoPoints}
            </span>
          </div>
        </div>

        <div className="hidden max-w-md text-left sm:block">
          <p className="text-sm leading-relaxed text-emerald-50/90">
            Eco points represent your community&apos;s collective environmental
            impact. Earn more by participating in initiatives.
          </p>
        </div>
      </div>
    </div>
  );
}
