"use client";

import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MessageSquarePlus } from "lucide-react";
import { ConversationSummaryDTO, SelectedUser, formatDate, initials } from "./types";

interface Props {
  conversations: ConversationSummaryDTO[];
  selectedUser: SelectedUser | null;
  onSelect: (id: string, firstName: string, lastName: string) => void;
  onNewConversation: () => void;
}

export function ConversationList({ conversations, selectedUser, onSelect, onNewConversation }: Props) {
  return (
    <div className="flex w-72 shrink-0 flex-col border-r">
      <div className="flex items-center justify-between border-b p-4">
        <h2 className="text-lg font-semibold">Messages</h2>
        <Button
          variant="ghost"
          size="icon"
          onClick={onNewConversation}
          title="New conversation"
        >
          <MessageSquarePlus className="h-5 w-5" />
        </Button>
      </div>

      <ScrollArea className="flex-1">
        {conversations.length === 0 ? (
          <p className="p-4 text-sm text-muted-foreground">
            No conversations yet. Start one with a community member.
          </p>
        ) : (
          conversations.map((c) => {
            const active = selectedUser?.id === c.userId;
            return (
              <button
                key={c.userId}
                onClick={() => onSelect(c.userId, c.firstName, c.lastName)}
                className={`flex w-full items-center gap-3 px-4 py-3 text-left transition-colors hover:bg-muted ${active ? "bg-muted" : ""}`}
              >
                <Avatar className="h-9 w-9 shrink-0">
                  <AvatarFallback>{initials(c.firstName, c.lastName)}</AvatarFallback>
                </Avatar>
                <div className="min-w-0 flex-1">
                  <div className="flex items-baseline justify-between gap-1">
                    <span className="truncate text-sm font-medium">
                      {c.firstName} {c.lastName}
                    </span>
                    <span className="shrink-0 text-xs text-muted-foreground">
                      {formatDate(c.lastMessageAt)}
                    </span>
                  </div>
                  <p className="truncate text-xs text-muted-foreground">{c.lastMessage}</p>
                </div>
              </button>
            );
          })
        )}
      </ScrollArea>
    </div>
  );
}
