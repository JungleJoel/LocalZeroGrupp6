"use client";

import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { ScrollArea } from "@/components/ui/scroll-area";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import LoadingSpinner from "@/components/LoadingSpinner";
import { CommunityMemberDTO } from "@/types/communityMemberDTO";
import { initials } from "./types";

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  hasCommunity: boolean;
  loadingMembers: boolean;
  members: CommunityMemberDTO[];
  onSelect: (id: string, firstName: string, lastName: string) => void;
}

export function NewConversationDialog({
  open,
  onOpenChange,
  hasCommunity,
  loadingMembers,
  members,
  onSelect,
}: Props) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-sm">
        <DialogHeader>
          <DialogTitle>New Conversation</DialogTitle>
        </DialogHeader>
        {!hasCommunity ? (
          <p className="text-sm text-muted-foreground">
            You need to be in a community to start new conversations.
          </p>
        ) : loadingMembers ? (
          <LoadingSpinner />
        ) : members.length === 0 ? (
          <p className="text-sm text-muted-foreground">
            No other members in your community.
          </p>
        ) : (
          <ScrollArea className="max-h-72">
            {members.map((m) => (
              <button
                key={m.userId}
                onClick={() => onSelect(m.userId, m.firstName, m.lastName)}
                className="flex w-full items-center gap-3 rounded-md px-2 py-2 text-left transition-colors hover:bg-muted"
              >
                <Avatar className="h-8 w-8">
                  <AvatarFallback>{initials(m.firstName, m.lastName)}</AvatarFallback>
                </Avatar>
                <span className="text-sm font-medium">
                  {m.firstName} {m.lastName}
                </span>
              </button>
            ))}
          </ScrollArea>
        )}
      </DialogContent>
    </Dialog>
  );
}
