"use client";

import { CommunityMemberDTO } from "@/types/communityMemberDTO";
import { useState } from "react";
import { toast } from "sonner";
import { API_BASE_URL } from "@/lib/config";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Users, Shield } from "lucide-react";

interface MembersDialogProps {
  communityId: string;
  communityName: string;
}

export function MembersDialog({
  communityId,
  communityName,
}: MembersDialogProps) {
  const [members, setMembers] = useState<CommunityMemberDTO[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [hasFetched, setHasFetched] = useState(false);

  async function fetchMembers() {
    if (hasFetched) return;
    try {
      setIsLoading(true);
      const response = await fetch(
        `${API_BASE_URL}/Community/${communityId}/members`,
        {
          method: "GET",
          credentials: "include",
          headers: { "Content-Type": "application/json" },
        }
      );

      if (response.ok) {
        setMembers(await response.json());
        setHasFetched(true);
      } else {
        const json = await response.json();
        toast.error(json.detail || "Could not load members");
      }
    } catch {
      toast.error("Could not load members");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button
          variant="outline"
          size="sm"
          className="shrink-0"
          onClick={fetchMembers}
        >
          <Users className="h-4 w-4" />
          Members
        </Button>
      </DialogTrigger>
      <DialogContent className="max-w-sm">
        <DialogHeader>
          <DialogTitle>{communityName} members</DialogTitle>
        </DialogHeader>

        {isLoading ? (
          <p className="py-4 text-center text-sm text-muted-foreground">
            Loading…
          </p>
        ) : (
          <ul className="flex flex-col gap-1 py-1">
            {members.map((member) => (
              <li
                key={member.userId}
                className="flex items-center gap-3 rounded-lg px-2 py-2 hover:bg-muted/50"
              >
                {member.avatarImageUrl ? (
                  <img
                    src={member.avatarImageUrl}
                    alt=""
                    className="h-8 w-8 rounded-full object-cover"
                  />
                ) : (
                  <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-muted text-xs font-semibold text-muted-foreground">
                    {member.firstName[0]}
                    {member.lastName[0]}
                  </div>
                )}
                <div className="min-w-0 flex-1">
                  <p className="truncate text-sm font-medium">
                    {member.firstName} {member.lastName}
                  </p>
                </div>
                {member.isManager && (
                  <span className="flex items-center gap-1 rounded-full bg-primary/10 px-2 py-0.5 text-xs font-medium text-primary">
                    Manager
                  </span>
                )}
              </li>
            ))}
          </ul>
        )}
      </DialogContent>
    </Dialog>
  );
}
