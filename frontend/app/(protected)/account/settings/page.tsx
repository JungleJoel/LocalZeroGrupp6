"use client";

import * as React from "react";
import { toast } from "sonner";
import { UserIcon, Loader2Icon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { API_BASE_URL } from "@/lib/config";

interface AccountProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  avatarImageUrl: string | null;
  createdAt: string;
}

async function apiFetch(path: string, body: object, method = "PUT") {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    method,
    credentials: "include",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });
  if (!res.ok) throw new Error(await res.text() || `Request failed: ${res.status}`);
}

function useSubmit(fn: () => Promise<void>) {
  const [loading, setLoading] = React.useState(false);
  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await fn();
    } finally {
      setLoading(false);
    }
  };
  return { loading, submit };
}

function Field({ label, id, ...props }: { label: string; id: string } & React.ComponentProps<"input">) {
  return (
    <div className="flex flex-col gap-1.5">
      <Label htmlFor={id}>{label}</Label>
      <Input id={id} {...props} />
    </div>
  );
}

function SaveButton({ loading, label = "Save changes" }: { loading: boolean; label?: string }) {
  return (
    <div className="flex justify-end">
      <Button type="submit" disabled={loading}>
        {loading && <Loader2Icon className="size-4 animate-spin" />}
        {label}
      </Button>
    </div>
  );
}

function SettingsCard({ title, description, children }: { title: string; description: string; children: React.ReactNode }) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
        <CardDescription>{description}</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="flex flex-col gap-4">{children}</div>
      </CardContent>
    </Card>
  );
}

function UpdateNameCard({ profile }: { profile: AccountProfile }) {
  const [firstName, setFirstName] = React.useState(profile.firstName);
  const [lastName, setLastName] = React.useState(profile.lastName);
  const { loading, submit } = useSubmit(async () => {
    await apiFetch("/Account/name", { firstName, lastName });
    toast.success("Name updated successfully.");
  });

  return (
    <SettingsCard title="Display Name" description="Update your first and last name.">
      <form onSubmit={submit} className="flex flex-col gap-4">
        <div className="grid grid-cols-2 gap-3">
          <Field label="First name" id="firstName" value={firstName} onChange={(e) => setFirstName(e.target.value)} placeholder="First name" required />
          <Field label="Last name" id="lastName" value={lastName} onChange={(e) => setLastName(e.target.value)} placeholder="Last name" required />
        </div>
        <SaveButton loading={loading} />
      </form>
    </SettingsCard>
  );
}

function UpdateEmailCard({ profile }: { profile: AccountProfile }) {
  const [email, setEmail] = React.useState(profile.email);
  const [password, setPassword] = React.useState("");
  const { loading, submit } = useSubmit(async () => {
    await apiFetch("/Account/email", { newEmail: email, currentPassword: password });
    toast.success("Email updated successfully.");
    setPassword("");
  });

  return (
    <SettingsCard title="Email Address" description="Change the email linked to your account.">
      <form onSubmit={submit} className="flex flex-col gap-4">
        <Field label="New email" id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="you@example.com" required />
        <Field label="Current password" id="emailPassword" type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Confirm with your password" required />
        <SaveButton loading={loading} />
      </form>
    </SettingsCard>
  );
}

function UpdatePasswordCard() {
  const [current, setCurrent] = React.useState("");
  const [next, setNext] = React.useState("");
  const [confirm, setConfirm] = React.useState("");
  const { loading, submit } = useSubmit(async () => {
    if (next !== confirm) { toast.error("New passwords do not match."); return; }
    await apiFetch("/Account/password", { currentPassword: current, newPassword: next, confirmNewPassword: confirm });
    toast.success("Password updated successfully.");
    setCurrent(""); setNext(""); setConfirm("");
  });

  return (
    <SettingsCard title="Password" description="Choose a strong password of at least 8 characters.">
      <form onSubmit={submit} className="flex flex-col gap-4">
        <Field label="Current password" id="currentPassword" type="password" value={current} onChange={(e) => setCurrent(e.target.value)} placeholder="Current password" />
        <Field label="New password" id="newPassword" type="password" value={next} onChange={(e) => setNext(e.target.value)} placeholder="New password" />
        <Field label="Confirm new password" id="confirmPassword" type="password" value={confirm} onChange={(e) => setConfirm(e.target.value)} placeholder="Confirm new password" />
        <SaveButton loading={loading} label="Update password" />
      </form>
    </SettingsCard>
  );
}

function UpdateAvatarCard({ profile }: { profile: AccountProfile }) {
  const [url, setUrl] = React.useState(profile.avatarImageUrl ?? "");
  const { loading, submit } = useSubmit(async () => {
    await apiFetch("/Account/avatar", { avatarImageUrl: url });
    toast.success("Avatar updated successfully.");
  });

  return (
    <SettingsCard title="Profile Picture" description="Paste a URL to use as your avatar.">
      <form onSubmit={submit} className="flex flex-col gap-4">
        <div className="flex items-center gap-4">
          {url ? (
            <img src={url} alt="Avatar preview" className="size-14 rounded-full object-cover ring-2 ring-border" onError={(e) => (e.currentTarget.style.display = "none")} />
          ) : (
            <div className="flex size-14 items-center justify-center rounded-full bg-muted ring-2 ring-border">
              <UserIcon className="size-6 text-muted-foreground" />
            </div>
          )}
          <div className="flex-1">
            <Field label="Image URL" id="avatarUrl" type="url" value={url} onChange={(e) => setUrl(e.target.value)} placeholder="https://example.com/avatar.jpg" required />
          </div>
        </div>
        <SaveButton loading={loading} />
      </form>
    </SettingsCard>
  );
}

export default function AccountSettingsPage() {
  const [profile, setProfile] = React.useState<AccountProfile | null>(null);
  const [error, setError] = React.useState<string | null>(null);

  React.useEffect(() => {
    fetch(`${API_BASE_URL}/Account/me`, { credentials: "include" })
      .then((res) => res.json())
      .then(setProfile)
      .catch(() => setError("Failed to load profile. Please refresh."));
  }, []);

  if (error) return (
    <div className="flex min-h-[40vh] items-center justify-center">
      <p className="text-sm text-destructive">{error}</p>
    </div>
  );

  if (!profile) return (
    <div className="flex min-h-[40vh] items-center justify-center">
      <Loader2Icon className="size-5 animate-spin text-muted-foreground" />
    </div>
  );

  return (
    <div className="mx-auto max-w-2xl px-4 py-10">
      <div className="mb-8">
        <h1 className="text-2xl font-semibold">Account Settings</h1>
        <p className="mt-1 text-sm text-muted-foreground">Manage your personal information and security.</p>
      </div>
      <div className="flex flex-col gap-4">
        <UpdateAvatarCard profile={profile} />
        <UpdateNameCard profile={profile} />
        <UpdateEmailCard profile={profile} />
        <UpdatePasswordCard />
      </div>
    </div>
  );
}