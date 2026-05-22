import { API_BASE_URL } from "@/lib/config";

export interface InitiativeCommentDTO {
  id: string;
  initiativeId: string;
  userId: string;
  authorName: string;
  body: string;
  createdAt: string;
  likeCount: number;
  isLiked: boolean;
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

async function getErrorMessage(response: Response, fallback: string) {
  try {
    const data = await response.json();
    return data.detail || data.title || fallback;
  } catch {
    return `${fallback} (${response.status})`;
  }
}

export async function likeInitiativeComment(
  initiativeId: string,
  commentId: string
) {
  const response = await fetch(
    `${API_BASE_URL}/Initiative/${initiativeId}/comments/${commentId}/like`,
    {
      method: "POST",
      credentials: "include",
    }
  );

  if (!response.ok) {
    throw new Error(await getErrorMessage(response, "Could not like comment"));
  }

  return (await response.json()) as InitiativeCommentDTO;
}

export async function unlikeInitiativeComment(
  initiativeId: string,
  commentId: string
) {
  const response = await fetch(
    `${API_BASE_URL}/Initiative/${initiativeId}/comments/${commentId}/like`,
    {
      method: "DELETE",
      credentials: "include",
    }
  );

  if (!response.ok) {
    throw new Error(await getErrorMessage(response, "Could not unlike comment"));
  }
}
