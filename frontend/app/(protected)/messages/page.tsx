"use client";

import { useEffect, useRef, useState } from "react";
import { API_BASE_URL } from "@/lib/config";
import { toast } from "sonner";
import { UserDTO } from "@/types/userDTO";
import { CommunityMemberDTO } from "@/types/communityMemberDTO";
import LoadingSpinner from "@/components/LoadingSpinner";
import { ConversationList } from "./ConversationList";
import { MessageThread } from "./MessageThread";
import { NewConversationDialog } from "./NewConversationDialog";
import { ConversationSummaryDTO, DirectMessageDTO, SelectedUser } from "./types";

export default function MessagesPage() {
  const [currentUser, setCurrentUser] = useState<UserDTO | null>(null);
  const [conversations, setConversations] = useState<ConversationSummaryDTO[]>([]);
  const [selectedUser, setSelectedUser] = useState<SelectedUser | null>(null);
  const [messages, setMessages] = useState<DirectMessageDTO[]>([]);
  const [input, setInput] = useState("");
  const [sending, setSending] = useState(false);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [members, setMembers] = useState<CommunityMemberDTO[]>([]);
  const [loadingMembers, setLoadingMembers] = useState(false);
  const [pageReady, setPageReady] = useState(false);

  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    async function init() {
      try {
        const res = await fetch(`${API_BASE_URL}/Auth/check-auth`, {
          credentials: "include",
        });
        if (!res.ok) return;
        setCurrentUser(await res.json());
      } catch {
        toast.error("Failed to load user session");
      } finally {
        setPageReady(true);
      }
    }
    init();
  }, []);

  useEffect(() => {
    if (!pageReady) return;
    const fetch_ = async () => {
      try {
        const res = await fetch(`${API_BASE_URL}/DirectMessage/conversations`, {
          credentials: "include",
        });
        if (res.ok) setConversations(await res.json());
      } catch {}
    };
    fetch_();
    const interval = setInterval(fetch_, 1000);
    return () => clearInterval(interval);
  }, [pageReady]);

  useEffect(() => {
    if (!selectedUser) return;
    const fetch_ = async () => {
      try {
        const res = await fetch(
          `${API_BASE_URL}/DirectMessage/conversation/${selectedUser.id}`,
          { credentials: "include" }
        );
        if (res.ok) setMessages(await res.json());
      } catch {}
    };
    setMessages([]);
    fetch_();
    const interval = setInterval(fetch_, 3000);
    return () => clearInterval(interval);
  }, [selectedUser?.id]);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  function selectUser(id: string, firstName: string, lastName: string) {
    setSelectedUser({ id, firstName, lastName });
    setDialogOpen(false);
  }

  async function openNewConversationDialog() {
    setDialogOpen(true);
    if (members.length > 0 || !currentUser?.community) return;
    setLoadingMembers(true);
    try {
      const res = await fetch(
        `${API_BASE_URL}/Community/${currentUser.community.id}/members`,
        { credentials: "include" }
      );
      if (res.ok) {
        const all: CommunityMemberDTO[] = await res.json();
        setMembers(all.filter((m) => m.userId !== currentUser.id));
      }
    } catch {
      toast.error("Failed to load community members");
    } finally {
      setLoadingMembers(false);
    }
  }

  async function sendMessage() {
    if (!selectedUser || !input.trim()) return;
    setSending(true);
    try {
      const res = await fetch(`${API_BASE_URL}/DirectMessage`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ recipientId: selectedUser.id, body: input.trim() }),
      });
      if (!res.ok) {
        const json = await res.json();
        throw new Error(json.detail || "Failed to send message");
      }
      setInput("");
      const msg: DirectMessageDTO = await res.json();
      setMessages((prev) => [...prev, msg]);
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : "Failed to send message");
    } finally {
      setSending(false);
    }
  }

  if (!pageReady) return (
        <div className="flex h-screen flex-col items-center justify-center">
          <LoadingSpinner />
        </div>
        );

  return (
    <div className="flex h-screen w-full overflow-hidden">
      <ConversationList
        conversations={conversations}
        selectedUser={selectedUser}
        onSelect={selectUser}
        onNewConversation={openNewConversationDialog}
      />

      <div className="flex flex-1 flex-col">
        {selectedUser ? (
          <MessageThread
            selectedUser={selectedUser}
            messages={messages}
            currentUserId={currentUser?.id ?? ""}
            input={input}
            sending={sending}
            onInputChange={setInput}
            onSend={sendMessage}
            bottomRef={bottomRef}
          />
        ) : (
          <div className="flex flex-1 items-center justify-center text-muted-foreground">
            Select a conversation or start a new one
          </div>
        )}
      </div>

      <NewConversationDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        hasCommunity={!!currentUser?.community}
        loadingMembers={loadingMembers}
        members={members}
        onSelect={selectUser}
      />
    </div>
  );
}
