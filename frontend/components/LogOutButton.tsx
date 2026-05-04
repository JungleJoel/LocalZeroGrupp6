"use client";
import { toast } from "sonner";
import { Button } from "./ui/button";
import { useRouter } from "next/navigation";
import { API_BASE_URL } from "@/lib/config";

export function LogOutButton({ className }: { className?: string }) {
  const router = useRouter();

  async function handleLogout() {
    try {
      const response = await fetch(`${API_BASE_URL}/Auth/logout`, {
        method: "POST",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (response.ok) {
        router.replace("/");
      } else {
        const json = await response.json();
        throw new Error(json.detail);
      }
    } catch (error: any) {
      toast.error(error.message);
      console.log(error);
    }
  }

  return (
    <Button onClick={handleLogout} className={className}>
      Log out
    </Button>
  );
}
