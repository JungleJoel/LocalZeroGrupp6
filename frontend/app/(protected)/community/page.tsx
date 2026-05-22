"use client";
/*COMMUNITY PAGE*/
import { GetMyCommunityResponseDTO } from "@/types/getMyCommunityResponseDTO";
import { MyJoinRequestDTO } from "@/types/myJoinRequestDTO";
import { useEffect, useState } from "react";
import { API_BASE_URL } from "@/lib/config";
import { toast } from "sonner";
import { EcoPointsCard } from "./EcoPointsCard";
import { CommunityInitiatives } from "./CommunityInitiatives";
import { NotAMember } from "./NotAMember";
import { PendingRequest } from "./PendingRequest";
import { CommunityJoinRequests } from "./CommunityJoinRequests";
import { MembersDialog } from "./MembersDialog";
import LoadingSpinner from "@/components/LoadingSpinner";
import { Button } from "@/components/ui/button";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { LogOut } from "lucide-react";

type PageState = "loading" | "community" | "pending" | "not-member" | "error";

export default function Community() {
  const [pageState, setPageState] = useState<PageState>("loading");
  const [apiResponse, setApiResponse] =
    useState<GetMyCommunityResponseDTO | null>(null);
  const [pendingRequest, setPendingRequest] = useState<MyJoinRequestDTO | null>(
    null
  );
  const [isLeaving, setIsLeaving] = useState(false);
  const [isCancelling, setIsCancelling] = useState(false);

  async function getMyCommunity() {
    const response = await fetch(`${API_BASE_URL}/Community/my-community`, {
      method: "GET",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
    });

    if (response.ok) {
      setApiResponse(await response.json());
      setPageState("community");
      return;
    }

    if (response.status === 404) {
      await checkPendingRequest();
      return;
    }

    const json = await response.json();
    throw new Error(json.detail || "Could not get community");
  }

  async function checkPendingRequest() {
    const response = await fetch(`${API_BASE_URL}/Community/my-join-request`, {
      method: "GET",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
    });

    if (response.ok) {
      setPendingRequest(await response.json());
      setPageState("pending");
    } else {
      setPageState("not-member");
    }
  }

  useEffect(() => {
    getMyCommunity().catch((error: any) => {
      toast.error(error.message);
      setPageState("error");
    });
  }, []);

  async function handleLeave() {
    if (!apiResponse) return;
    try {
      setIsLeaving(true);
      const response = await fetch(
        `${API_BASE_URL}/Community/${apiResponse.community.id}/leave`,
        {
          method: "POST",
          credentials: "include",
          headers: { "Content-Type": "application/json" },
        }
      );

      if (response.ok) {
        toast.success("You have left the community");
        setApiResponse(null);
        setPageState("not-member");
      } else {
        const json = await response.json();
        toast.error(json.detail || "Could not leave community");
      }
    } catch {
      toast.error("Could not leave community");
    } finally {
      setIsLeaving(false);
    }
  }

  async function handleCancelRequest() {
    if (!pendingRequest) return;
    try {
      setIsCancelling(true);
      const response = await fetch(
        `${API_BASE_URL}/Community/${pendingRequest.communityId}/cancel-request`,
        {
          method: "DELETE",
          credentials: "include",
          headers: { "Content-Type": "application/json" },
        }
      );

      if (response.ok) {
        toast.success("Join request cancelled");
        setPendingRequest(null);
        setPageState("not-member");
      } else {
        const json = await response.json();
        toast.error(json.detail || "Could not cancel request");
      }
    } catch {
      toast.error("Could not cancel request");
    } finally {
      setIsCancelling(false);
    }
  }

  if (pageState === "loading") {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <LoadingSpinner />
      </div>
    );
  }

  if (pageState === "pending" && pendingRequest) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <PendingRequest
          communityName={pendingRequest.communityName}
          onCancel={handleCancelRequest}
          isCancelling={isCancelling}
        />
      </div>
    );
  }

  if (pageState === "not-member") {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <NotAMember />
      </div>
    );
  }

  if (pageState === "error" || apiResponse == null) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <h1>Something went wrong</h1>
      </div>
    );
  }

  return (
    <div className="flex justify-center px-6 pt-12">
      <div className="w-full max-w-4xl">
        <div className="mb-3 flex flex-row items-center justify-between gap-3">
          <div className="flex flex-row items-center gap-3">
            <h1 className="text-3xl font-semibold sm:text-4xl">
              {apiResponse.community.name}
            </h1>
            <p className="hidden text-muted-foreground sm:block">
              You are a{" "}
              {apiResponse.isCommunityManager ? "manager" : "resident"} of this
              community
            </p>
          </div>

          <div className="flex items-center gap-2">
            <MembersDialog
              communityId={apiResponse.community.id}
              communityName={apiResponse.community.name}
            />

            <AlertDialog>
              <AlertDialogTrigger asChild>
                <Button variant="outline" size="sm" className="shrink-0">
                  <LogOut className="h-4 w-4" />
                  Leave
                </Button>
              </AlertDialogTrigger>
              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>Leave community?</AlertDialogTitle>
                  <AlertDialogDescription>
                    You will lose access to{" "}
                    <span className="font-medium">
                      {apiResponse.community.name}
                    </span>
                    . You can request to join again later.
                    {apiResponse.isCommunityManager && (
                      <span className="mt-2 block text-amber-600 dark:text-amber-400">
                        As a manager, you can only leave if there is at least
                        one other manager.
                      </span>
                    )}
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel>Cancel</AlertDialogCancel>
                  <AlertDialogAction
                    onClick={handleLeave}
                    disabled={isLeaving}
                    className="text-destructive-foreground bg-destructive hover:bg-destructive/90"
                  >
                    {isLeaving ? "Leaving…" : "Leave community"}
                  </AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
          </div>
        </div>

        <EcoPointsCard community={apiResponse.community} />
        <CommunityInitiatives communityId={apiResponse.community.id} isManager={apiResponse.isCommunityManager} />

        {apiResponse.isCommunityManager && (
          <CommunityJoinRequests communityId={apiResponse.community.id} />
        )}
      </div>
    </div>
  );
}
