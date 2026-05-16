"use client";

import { RefObject } from "react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Send } from "lucide-react";
import { DirectMessageDTO, SelectedUser, formatTime, initials } from "./types";

interface Props {
  selectedUser: SelectedUser;
  messages: DirectMessageDTO[];
  currentUserId: string;
  input: string;
  sending: boolean;
  onInputChange: (value: string) => void;
  onSend: () => void;
  bottomRef: RefObject<HTMLDivElement | null>;
}

export function MessageThread({
  selectedUser,
  messages,
  currentUserId,
  input,
  sending,
  onInputChange,
  onSend,
  bottomRef,
}: Props) {
  function handleKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      onSend();
    }
  }

  return (
    <>
      <div className="flex items-center gap-3 border-b p-4">
        <Avatar className="h-8 w-8">
          <AvatarFallback>
            {initials(selectedUser.firstName, selectedUser.lastName)}
          </AvatarFallback>
        </Avatar>
        <span className="font-medium">
          {selectedUser.firstName} {selectedUser.lastName}
        </span>
      </div>

      <ScrollArea className="flex-1 px-4 py-2">
        <div className="flex flex-col gap-1">
          {messages.map((m) => {
            const mine = m.senderId === currentUserId;
            return (
              <div
                key={m.id}
                className={`flex ${mine ? "justify-end" : "justify-start"}`}
              >
                <div
                  className={`max-w-lg rounded-2xl px-3 py-2 text-sm break-words whitespace-pre-wrap ${
                    mine
                      ? "bg-primary text-primary-foreground"
                      : "bg-muted text-foreground"
                  }`}
                >
                  <p>{m.body}</p>
                  <p
                    className={`mt-0.5 text-right text-[10px] ${
                      mine ? "text-primary-foreground/70" : "text-muted-foreground"
                    }`}
                  >
                    {formatTime(m.createdAt)}
                  </p>
                </div>
              </div>
            );
          })}
          <div ref={bottomRef} />
        </div>
      </ScrollArea>

      <div className="flex items-center gap-2 border-t p-4">
        <Input
          placeholder="Type a message…"
          value={input}
          onChange={(e) => onInputChange(e.target.value)}
          onKeyDown={handleKeyDown}
          disabled={sending}
          className="flex-1"
        />
        <Button onClick={onSend} disabled={sending || !input.trim()} size="icon">
          <Send className="h-4 w-4" />
        </Button>
      </div>
    </>
  );
}
