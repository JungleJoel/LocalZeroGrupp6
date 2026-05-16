export interface ConversationSummaryDTO {
  userId: string;
  firstName: string;
  lastName: string;
  avatarImageUrl: string | null;
  lastMessage: string;
  lastMessageAt: string;
}

export interface DirectMessageDTO {
  id: string;
  senderId: string;
  recipientId: string;
  body: string;
  createdAt: string;
}

export interface SelectedUser {
  id: string;
  firstName: string;
  lastName: string;
}

export function initials(first: string, last: string) {
  return `${first[0] ?? ""}${last[0] ?? ""}`.toUpperCase();
}

export function formatTime(iso: string) {
  return new Date(iso).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
}

export function formatDate(iso: string) {
  const d = new Date(iso);
  if (d.toDateString() === new Date().toDateString()) return formatTime(iso);
  return d.toLocaleDateString([], { month: "short", day: "numeric" });
}
