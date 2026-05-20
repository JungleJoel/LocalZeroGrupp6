import { API_BASE_URL } from "@/lib/config";

export interface InitiativeCommentDTO {
  id: string;
  initiativeId: string;
  userId: string;
  authorName: string;
  body: string;
  createdAt: string;
}

export async function getInitiativeComments(initiativeId: string) {
  const response = await fetch(
    `${API_BASE_URL}/Initiative/${initiativeId}/comments`,
    { credentials: "include" }
  );

  if (!response.ok) {
    throw new Error("Could not load comments");
  }

  return (await response.json()) as InitiativeCommentDTO[];
}

export async function createInitiativeComment(
  initiativeId: string,
  body: string
) {
  const response = await fetch(
    `${API_BASE_URL}/Initiative/${initiativeId}/comments`,
    {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ body }),
    }
  );

  if (!response.ok) {
    throw new Error("Could not post comment");
  }

  return (await response.json()) as InitiativeCommentDTO;
}
