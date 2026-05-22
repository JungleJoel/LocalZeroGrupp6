"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { API_BASE_URL } from "@/lib/config";
import { InitiativeDTO } from "@/types/initiativeDTO";
import {
  InitiativeCommentDTO,
  createInitiativeComment,
  getInitiativeComments,
  likeInitiativeComment,
  unlikeInitiativeComment,
} from "@/lib/comments";
import { toast } from "sonner";
import {
  ArrowLeft,
  Calendar,
  CalendarCheck,
  Leaf,
  MapPin,
  Lock,
  Globe,
  Heart,
  Loader2,
  MessageCircle,
  Send,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import LoadingSpinner from "@/components/LoadingSpinner";

function getStatus(initiative: InitiativeDTO): "active" | "upcoming" | "ended" {
  if (initiative.endedAt) return "ended";
  return new Date(initiative.startsAt) <= new Date() ? "active" : "upcoming";
}

function formatDate(dateStr: string | null) {
  if (!dateStr) return null;
  return new Date(dateStr).toLocaleDateString("en-SE", {
    day: "numeric",
    month: "long",
    year: "numeric",
  });
}

export default function InitiativePage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [initiative, setInitiative] = useState<InitiativeDTO | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isJoined, setIsJoined] = useState(false);
  const [isJoinLoading, setIsJoinLoading] = useState(false);
  const [isLikeLoading, setIsLikeLoading] = useState(false);
  const [locationName, setLocationName] = useState<string | null>(null);
  const [comments, setComments] = useState<InitiativeCommentDTO[]>([]);
  const [commentBody, setCommentBody] = useState("");
  const [isCommentsLoading, setIsCommentsLoading] = useState(true);
  const [isCommentPosting, setIsCommentPosting] = useState(false);
  const [likingCommentId, setLikingCommentId] = useState<string | null>(null);

  useEffect(() => {
    async function fetchInitiative() {
      try {
        const response = await fetch(`${API_BASE_URL}/Initiative/${id}`, {
          credentials: "include",
        });
        if (response.ok) {
          const data : InitiativeDTO = await response.json();
          setInitiative(data);
          setIsJoined(data.isParticipating);
        } else {
          throw new Error("Could not load initiative");
        }
      } catch (error: any) {
        toast.error(error.message);
      } finally {
        setIsLoading(false);
      }
    }
    fetchInitiative();
  }, [id]);

  useEffect(() => {
    async function fetchComments() {
      try {
        const data = await getInitiativeComments(id);
        setComments(data);
      } catch (error: any) {
        toast.error(error.message);
      } finally {
        setIsCommentsLoading(false);
      }
    }

    fetchComments();
  }, [id]);

   useEffect(() => {
  if (!initiative) return;
  async function fetchLocationName() {
    try {
      const response = await fetch(
        `https://nominatim.openstreetmap.org/reverse?lat=${initiative!.latitude}&lon=${initiative!.longitude}&format=json`,
        { headers: { "Accept-Language": "sv" } }
      );
      if (response.ok) {
        const data = await response.json();
        console.log("shit i recieve",data);
        setLocationName([data.address.road, data.address.city].filter(Boolean).join(", ") ?? null);
      }
    } catch {}
  }
  fetchLocationName();
}, [initiative]);


  if (isLoading) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <LoadingSpinner />
      </div>
    );
  }

  if (!initiative) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <p className="text-muted-foreground">Initiative not found.</p>
      </div>
    );
  }

 
  
  async function handleJoinLeave() {
    setIsJoinLoading(true);
    try {
      const response = await fetch(
        `${API_BASE_URL}/Initiative/${id}/${isJoined ? "leave" : "join"}`,
        {
          method: isJoined ? "DELETE" : "POST", // leave=DELETE, join=POST
          credentials: "include",
        }
      );
      if (!response.ok) throw new Error("Action failed");
      setIsJoined(!isJoined);
      toast.success(isJoined ? "Left initiative" : "Joined initiative");
    } catch (error: any) {
      toast.error(error.message);
    } finally {
      setIsJoinLoading(false);
    }
  }

  async function handlePostComment() {
    const trimmedBody = commentBody.trim();
    if (!trimmedBody) return;

    setIsCommentPosting(true);
    try {
      const comment = await createInitiativeComment(id, trimmedBody);
      setComments((current) => [...current, comment]);
      setCommentBody("");
      toast.success("Comment posted");
    } catch (error: any) {
      toast.error(error.message);
    } finally {
      setIsCommentPosting(false);
    }
  }

  async function handleLikeInitiative() {
    if (!initiative) return;

    setIsLikeLoading(true);
    try {
      const response = await fetch(`${API_BASE_URL}/Initiative/${id}/like`, {
        method: initiative.isLiked ? "DELETE" : "POST",
        credentials: "include",
      });

      if (!response.ok) throw new Error("Could not update like");

      if (initiative.isLiked) {
        setInitiative({
          ...initiative,
          isLiked: false,
          likeCount: Math.max(0, initiative.likeCount - 1),
        });
      } else {
        setInitiative((await response.json()) as InitiativeDTO);
      }
    } catch (error: any) {
      toast.error(error.message);
    } finally {
      setIsLikeLoading(false);
    }
  }

  async function handleLikeComment(comment: InitiativeCommentDTO) {
    setLikingCommentId(comment.id);
    try {
      if (comment.isLiked) {
        await unlikeInitiativeComment(id, comment.id);
        setComments((current) =>
          current.map((item) =>
            item.id === comment.id
              ? {
                  ...item,
                  isLiked: false,
                  likeCount: Math.max(0, item.likeCount - 1),
                }
              : item
          )
        );
      } else {
        const updatedComment = await likeInitiativeComment(id, comment.id);
        setComments((current) =>
          current.map((item) =>
            item.id === comment.id ? updatedComment : item
          )
        );
      }
    } catch (error: any) {
      toast.error(error.message);
    } finally {
      setLikingCommentId(null);
    }
  }

  const status = getStatus(initiative);

  return (
    <div className="flex justify-center px-6 pt-12">
      <div className="w-full max-w-4xl">
        <div className="mb-6 flex items-center gap-3">
          <Button
            variant="ghost"
            size="sm"
            className="gap-1.5 text-muted-foreground"
            onClick={() => router.back()}
          >
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </div>

        {/* Title & Status */}
        <div className="mb-3 flex flex-wrap items-center gap-3">
          <h1 className="text-3xl font-semibold sm:text-4xl">
            {initiative.name}
          </h1>
          <span
            className={`rounded-full px-3 py-1 text-sm font-medium ${
              status === "active"
                ? "bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400"
                : status === "upcoming"
                  ? "bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400"
                  : "bg-muted text-muted-foreground"
            }`}
          >
            {status === "active"
              ? "Active"
              : status === "upcoming"
                ? "Upcoming"
                : "Ended"}
          </span>

          {status !== "ended" && (
            <Button
              variant={isJoined ? "outline" : "default"}
              onClick={handleJoinLeave}
              disabled={isJoinLoading}
            >
              {isJoinLoading ? (
                <Loader2 className="h-4 w-4 animate-spin" />
              ) : isJoined ? (
                "Leave"
              ) : (
                "Join"
              )}
            </Button>
          )}

          <Button
            variant="outline"
            onClick={handleLikeInitiative}
            disabled={isLikeLoading}
            className="gap-2"
          >
            {isLikeLoading ? (
              <Loader2 className="h-4 w-4 animate-spin" />
            ) : (
              <Heart
                className="h-4 w-4"
                fill={initiative.isLiked ? "currentColor" : "none"}
              />
            )}
            {initiative.likeCount}
          </Button>

          <Button style={{ marginLeft: "auto" }}>
            {status !== "ended" ? (
              <a
                href={`https://www.google.com/maps/search/?api=1&query=${initiative.latitude},${initiative.longitude}`}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center gap-2"
              >
                <MapPin className="h-4 w-4" />
                View on map
              </a>
            ) : (
              <a>Map not Available</a>
            )}
          </Button>
        </div>

        {/* Details */}
        <div className="mt-6 grid grid-cols-1 items-start gap-4 sm:grid-cols-2">
          {/* Description */}
          <div className="col-span-full rounded-xl border bg-card p-5 shadow-sm">
            <h2 className="mb-2 text-sm font-medium text-muted-foreground">
              About this initiative
            </h2>
            <p className="leading-relaxed">{initiative.description}</p>
          </div>

          {/* Dates */}
          <div className="rounded-xl border bg-card p-5 shadow-sm">
            <h2 className="mb-3 text-sm font-medium text-muted-foreground">
              Dates
            </h2>
            <div className="flex flex-col gap-3">
              <div className="flex items-center gap-3">
                <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                  <Calendar className="h-4 w-4 text-primary" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Starts</p>
                  <p className="font-medium">
                    {formatDate(initiative.startsAt)}
                  </p>
                </div>
              </div>
              {initiative.estimatedEndsAt && (
                <div className="flex items-center gap-3">
                  <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                    <CalendarCheck className="h-4 w-4 text-primary" />
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground">
                      Estimated end
                    </p>
                    <p className="font-medium">
                      {formatDate(initiative.estimatedEndsAt)}
                    </p>
                  </div>
                </div>
              )}
              {initiative.endedAt && (
                <div className="flex items-center gap-3">
                  <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-muted">
                    <CalendarCheck className="h-4 w-4 text-muted-foreground" />
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground">Ended</p>
                    <p className="font-medium">
                      {formatDate(initiative.endedAt)}
                    </p>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* Location & visibility */}
          <div className="rounded-xl border bg-card p-5 shadow-sm">
            <h2 className="mb-3 text-sm font-medium text-muted-foreground">
              Details
            </h2>
            <div className="flex flex-col gap-3">
              <div className="flex items-center gap-3">
                <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                  <MapPin className="h-4 w-4 text-primary" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Location</p>
                  <p className="font-medium">
                    {locationName ?? `${initiative.latitude.toFixed(4)}, ${initiative.longitude.toFixed(4)}`}
                  </p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                  {initiative.isPublic ? (
                    <Globe className="h-4 w-4 text-primary" />
                  ) : (
                    <Lock className="h-4 w-4 text-primary" />
                  )}
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Visibility</p>
                  <p className="font-medium">
                    {initiative.isPublic ? "Public" : "Community only"}
                  </p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10">
                  <Leaf className="h-4 w-4 text-primary" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">
                    Eco points per participant
                  </p>
                  <p className="font-medium">
                    {initiative.ecoPointsPerParticipant} pts
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="mt-4 rounded-xl border bg-card p-5 shadow-sm">
          <div className="mb-4 flex items-center gap-2">
            <MessageCircle className="h-4 w-4 text-primary" />
            <h2 className="text-sm font-medium text-muted-foreground">
              Comments
            </h2>
          </div>

          <div className="mb-5 flex flex-col gap-2">
            <Textarea
              placeholder="Write a comment..."
              value={commentBody}
              onChange={(event) => setCommentBody(event.target.value)}
              disabled={isCommentPosting}
              className="min-h-20"
            />
            <div className="flex justify-end">
              <Button
                onClick={handlePostComment}
                disabled={isCommentPosting || !commentBody.trim()}
                className="gap-2"
              >
                {isCommentPosting ? (
                  <Loader2 className="h-4 w-4 animate-spin" />
                ) : (
                  <Send className="h-4 w-4" />
                )}
                Post
              </Button>
            </div>
          </div>

          {isCommentsLoading ? (
            <p className="text-sm text-muted-foreground">Loading comments...</p>
          ) : comments.length === 0 ? (
            <p className="text-sm text-muted-foreground">
              No comments yet. Start the conversation.
            </p>
          ) : (
            <div className="flex flex-col gap-4">
              {comments.map((comment) => (
                <div key={comment.id} className="border-t pt-4 first:border-t-0 first:pt-0">
                  <div className="mb-1 flex flex-wrap items-center gap-x-2 gap-y-1">
                    <p className="text-sm font-medium">{comment.authorName}</p>
                    <p className="text-xs text-muted-foreground">
                      {new Date(comment.createdAt).toLocaleString("en-SE", {
                        day: "numeric",
                        month: "short",
                        year: "numeric",
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </p>
                  </div>
                  <p className="whitespace-pre-wrap break-words text-sm leading-relaxed">
                    {comment.body}
                  </p>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => handleLikeComment(comment)}
                    disabled={likingCommentId === comment.id}
                    className="mt-2 gap-1.5 px-2 text-muted-foreground"
                  >
                    {likingCommentId === comment.id ? (
                      <Loader2 className="h-3.5 w-3.5 animate-spin" />
                    ) : (
                      <Heart
                        className="h-3.5 w-3.5"
                        fill={comment.isLiked ? "currentColor" : "none"}
                      />
                    )}
                    {comment.likeCount}
                  </Button>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
