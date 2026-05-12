"use client";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Spinner } from "@/components/ui/spinner";
import { Textarea } from "@/components/ui/textarea";
import { API_BASE_URL } from "@/lib/config";
import {
  Bike,
  CheckCircle2,
  Footprints,
  Leaf,
  Loader2,
  Recycle,
  Send,
  Sprout,
  Trophy,
  Users,
  Zap,
} from "lucide-react";
import { FormEvent, useEffect, useMemo, useState } from "react";
import { toast } from "sonner";

type PageState = "loading" | "ready" | "not-member" | "error";

type AccountProfile = {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  avatarImageUrl: string | null;
  createdAt: string;
};

type Community = {
  id: string;
  name: string;
  ecoPoints?: number;
  residentsCount?: number;
  latitude: number | null;
  longitude: number | null;
};

type GetMyCommunityResponse = {
  community: Community;
  isCommunityManager: boolean;
};

type EcoPointTransaction = {
  id: string;
  communityId: string;
  userId: string;
  initiativeId: string | null;
  amount: number;
  createdAt: string;
  reason: string | null;
};

type EcoAction = {
  label: string;
  reason: string;
  amount: number;
  icon: typeof Bike;
};

type CommunityEcoActionSummary = {
  communityId: string;
  totalActions: number;
  totalEcoPoints: number;
  activeMembers: number;
  actionsThisMonth: number;
  topActionReason: string | null;
  topActionCount: number;
};

const actionCatalog: EcoAction[] = [
  {
    label: "Biked to work",
    reason: "Biked to work",
    amount: 12,
    icon: Bike,
  },
  {
    label: "Walked instead of drove",
    reason: "Walked instead of drove",
    amount: 10,
    icon: Footprints,
  },
  {
    label: "Recycled household waste",
    reason: "Recycled household waste",
    amount: 8,
    icon: Recycle,
  },
  {
    label: "Saved energy at home",
    reason: "Saved energy at home",
    amount: 9,
    icon: Zap,
  },
  {
    label: "Planted something green",
    reason: "Planted something green",
    amount: 14,
    icon: Sprout,
  },
];

function matchAction(reason: string | null | undefined) {
  if (!reason) return undefined;

  return actionCatalog.find(
    (action) => action.reason.toLowerCase() === reason.toLowerCase()
  );
}

function formatDate(date: string) {
  return new Date(date).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export default function EcoActions() {
  const [pageState, setPageState] = useState<PageState>("loading");
  const [profile, setProfile] = useState<AccountProfile | null>(null);
  const [communityResponse, setCommunityResponse] =
    useState<GetMyCommunityResponse | null>(null);
  const [transactions, setTransactions] = useState<EcoPointTransaction[]>([]);
  const [communitySummary, setCommunitySummary] =
    useState<CommunityEcoActionSummary | null>(null);
  const [selectedAction, setSelectedAction] = useState<EcoAction>(
    actionCatalog[0]
  );
  const [reason, setReason] = useState(actionCatalog[0].reason);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function loadCommunitySummary(communityId: string) {
    const response = await fetch(
      `${API_BASE_URL}/EcoActions/community/${communityId}/summary`,
      {
        credentials: "include",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (!response.ok) {
      throw new Error("Could not load community achievements");
    }

    return (await response.json()) as CommunityEcoActionSummary;
  }

  async function loadTracker() {
    const [profileResponse, communityResponse] = await Promise.all([
      fetch(`${API_BASE_URL}/Account/me`, {
        credentials: "include",
        headers: { "Content-Type": "application/json" },
      }),
      fetch(`${API_BASE_URL}/Community/my-community`, {
        credentials: "include",
        headers: { "Content-Type": "application/json" },
      }),
    ]);

    if (communityResponse.status === 404) {
      setPageState("not-member");
      return;
    }

    if (!profileResponse.ok || !communityResponse.ok) {
      throw new Error("Could not load eco-actions");
    }

    const profileJson = (await profileResponse.json()) as AccountProfile;
    const communityJson =
      (await communityResponse.json()) as GetMyCommunityResponse;

    setProfile(profileJson);
    setCommunityResponse(communityJson);

    const [historyResponse, summary] = await Promise.all([
      fetch(
        `${API_BASE_URL}/EcoActions/community/${communityJson.community.id}/user/${profileJson.id}/eco-actions`,
        {
          credentials: "include",
          headers: { "Content-Type": "application/json" },
        }
      ),
      loadCommunitySummary(communityJson.community.id),
    ]);

    if (!historyResponse.ok) {
      throw new Error("Could not load eco-actions");
    }

    const history = (await historyResponse.json()) as EcoPointTransaction[];

    setTransactions(
      history.sort(
        (a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      )
    );
    setCommunitySummary(summary);
    setPageState("ready");
  }

  useEffect(() => {
    loadTracker().catch((error: Error) => {
      toast.error(error.message);
      setPageState("error");
    });
  }, []);

  const totalPoints = useMemo(
    () =>
      transactions.reduce(
        (total, transaction) => total + transaction.amount,
        0
      ),
    [transactions]
  );

  const communityAchievements = useMemo(() => {
    if (!communitySummary) return [];

    return [
      {
        title: "Community actions",
        detail: `${communitySummary.totalActions} eco-actions logged by ${communitySummary.activeMembers} members`,
        icon: Users,
      },
      {
        title: "This month",
        detail: `${communitySummary.actionsThisMonth} eco-actions logged this month`,
        icon: Leaf,
      },
      {
        title: communitySummary.topActionReason ?? "Top action",
        detail:
          communitySummary.topActionCount > 0
            ? `${communitySummary.topActionCount} matching actions logged`
            : "No community eco-actions logged yet",
        icon: matchAction(communitySummary.topActionReason)?.icon ?? Trophy,
      },
    ];
  }, [communitySummary]);

  function chooseAction(action: EcoAction) {
    setSelectedAction(action);
    setReason(action.reason);
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!profile || !communityResponse) return;

    const trimmedReason = reason.trim();
    if (!trimmedReason) {
      toast.error("Add a short note for your eco-action");
      return;
    }

    try {
      setIsSubmitting(true);
      const response = await fetch(
        `${API_BASE_URL}/EcoActions/community/${communityResponse.community.id}/user/${profile.id}/eco-actions`,
        {
          method: "POST",
          credentials: "include",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            amount: selectedAction.amount,
            reason: trimmedReason,
          }),
        }
      );

      if (!response.ok) {
        const json = await response.json().catch(() => null);
        throw new Error(json?.detail || "Could not log eco-action");
      }

      const transaction = (await response.json()) as EcoPointTransaction;
      setTransactions((current) => [transaction, ...current]);
      setCommunitySummary(
        await loadCommunitySummary(communityResponse.community.id)
      );
      setReason(selectedAction.reason);
      toast.success("Eco-action logged");
    } catch (error: unknown) {
      toast.error(
        error instanceof Error ? error.message : "Could not log eco-action"
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  if (pageState === "loading") {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <Spinner className="h-8 w-8" />
      </div>
    );
  }

  if (pageState === "not-member") {
    return (
      <div className="flex h-screen flex-col items-center justify-center px-6 text-center">
        <Leaf className="mb-4 h-12 w-12 text-primary" />
        <h1 className="text-3xl font-semibold">Join a community first</h1>
        <p className="mt-2 max-w-md text-muted-foreground">
          Eco-actions are connected to your community, so join one before
          logging eco-action progress.
        </p>
      </div>
    );
  }

  if (pageState === "error" || !profile || !communityResponse) {
    return (
      <div className="flex h-screen flex-col items-center justify-center">
        <h1>Something went wrong</h1>
      </div>
    );
  }

  return (
    <div className="mx-auto flex w-full max-w-6xl flex-col gap-6 px-6 py-10">
      <div className="flex flex-col gap-2">
        <Badge variant="outline" className="w-fit">
          {communityResponse.community.name}
        </Badge>
        <div className="flex flex-col justify-between gap-3 md:flex-row md:items-end">
          <div>
            <h1 className="text-3xl font-semibold tracking-tight sm:text-4xl">
              Eco Actions
            </h1>
            <p className="mt-2 max-w-2xl text-muted-foreground">
              Log personal eco-actions and see the community impact grow.
            </p>
          </div>
          <div className="grid grid-cols-2 gap-2 text-sm sm:grid-cols-3">
            <div className="rounded-lg border bg-card px-4 py-3">
              <p className="text-muted-foreground">Actions</p>
              <p className="text-2xl font-semibold tabular-nums">
                {transactions.length}
              </p>
            </div>
            <div className="rounded-lg border bg-card px-4 py-3">
              <p className="text-muted-foreground">Eco Points</p>
              <p className="text-2xl font-semibold tabular-nums">
                {totalPoints}
              </p>
            </div>
            <div className="rounded-lg border bg-card px-4 py-3">
              <p className="text-muted-foreground">Community Actions</p>
              <p className="text-2xl font-semibold tabular-nums">
                {communitySummary?.totalActions ?? 0}
              </p>
            </div>
          </div>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-[minmax(0,1.1fr)_minmax(320px,0.9fr)]">
        <Card>
          <CardHeader>
            <CardTitle>Log an eco-action</CardTitle>
            <CardDescription>
              Choose a common action or edit the note before saving.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-5">
              <div className="grid gap-3 sm:grid-cols-2">
                {actionCatalog.map((action) => {
                  const Icon = action.icon;
                  const isSelected = selectedAction.label === action.label;

                  return (
                    <button
                      key={action.label}
                      type="button"
                      onClick={() => chooseAction(action)}
                      className={`flex min-h-24 items-start gap-3 rounded-lg border p-4 text-left transition ${
                        isSelected
                          ? "border-primary bg-primary/10"
                          : "bg-card hover:bg-muted"
                      }`}
                    >
                      <Icon className="mt-0.5 h-5 w-5 shrink-0 text-primary" />
                      <span className="flex flex-col gap-1">
                        <span className="font-medium">{action.label}</span>
                        <span className="text-sm text-muted-foreground">
                          +{action.amount} points
                        </span>
                      </span>
                    </button>
                  );
                })}
              </div>

              <div className="space-y-2">
                <Label htmlFor="reason">Action note</Label>
                <Textarea
                  id="reason"
                  value={reason}
                  onChange={(event) => setReason(event.target.value)}
                  placeholder="Biked to work"
                  className="min-h-24"
                />
              </div>

              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? (
                  <Loader2 className="h-4 w-4 animate-spin" />
                ) : (
                  <Send className="h-4 w-4" />
                )}
                Log action
              </Button>
            </form>
          </CardContent>
        </Card>

        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>Community achievements</CardTitle>
              <CardDescription>
                Live highlights from your community's logged eco-actions.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              {communityAchievements.map((achievement) => {
                const Icon = achievement.icon;

                return (
                  <div
                    key={achievement.title}
                    className="flex items-start gap-3 rounded-lg border bg-muted/30 p-3"
                  >
                    <Icon className="mt-0.5 h-5 w-5 shrink-0 text-primary" />
                    <div>
                      <p className="font-medium">{achievement.title}</p>
                      <p className="text-sm text-muted-foreground">
                        {achievement.detail}
                      </p>
                    </div>
                  </div>
                );
              })}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Personal milestones</CardTitle>
              <CardDescription>
                Based on your eco-action history.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="flex items-center gap-3">
                <CheckCircle2 className="h-5 w-5 text-primary" />
                <span>{transactions.length} actions logged</span>
              </div>
              <div className="flex items-center gap-3">
                <Trophy className="h-5 w-5 text-primary" />
                <span>{totalPoints} eco points earned here</span>
              </div>
              <div className="flex items-center gap-3">
                <Leaf className="h-5 w-5 text-primary" />
                <span>{transactions.length} personal eco-actions tracked</span>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Recent eco-actions</CardTitle>
          <CardDescription>Your logged eco-action activity.</CardDescription>
        </CardHeader>
        <CardContent>
          {transactions.length === 0 ? (
            <div className="rounded-lg border border-dashed p-8 text-center text-muted-foreground">
              No eco-actions logged yet.
            </div>
          ) : (
            <div className="divide-y rounded-lg border">
              {transactions.map((transaction) => {
                const action = matchAction(transaction.reason);
                const Icon = action?.icon ?? Leaf;
                const displayReason = transaction.reason ?? "Eco-action";

                return (
                  <div
                    key={transaction.id}
                    className="flex flex-col gap-3 p-4 sm:flex-row sm:items-center sm:justify-between"
                  >
                    <div className="flex items-start gap-3">
                      <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-primary/10">
                        <Icon className="h-5 w-5 text-primary" />
                      </div>
                      <div>
                        <p className="font-medium">{displayReason}</p>
                        <p className="text-sm text-muted-foreground">
                          {formatDate(transaction.createdAt)}
                        </p>
                      </div>
                    </div>
                    <div className="flex flex-wrap gap-2 sm:justify-end">
                      <Badge variant="secondary">
                        +{transaction.amount} points
                      </Badge>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
