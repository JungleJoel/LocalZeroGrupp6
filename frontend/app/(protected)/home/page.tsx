"use client";
import { Button } from "@/components/ui/button";
import { API_BASE_URL } from "@/lib/config";
import { useRouter } from "next/navigation";
import { toast } from "sonner";

export default function Home() {
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
    <div className="flex h-screen flex-col items-center justify-center gap-3">
      <h1 className="text-xl">You are logged in</h1>
      <Button onClick={handleLogout}>Log out</Button>
    </div>
  );
}
