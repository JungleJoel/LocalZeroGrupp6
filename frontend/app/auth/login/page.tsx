"use client";
import { useState, FormEvent } from "react"; // Added FormEvent type
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Users } from "lucide-react";
import { API_BASE_URL } from "@/lib/config";
import { toast } from "sonner";
import { useRouter } from "next/navigation";
import { LoginRequestDTO } from "@/types/loginRequestDTO";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const router = useRouter();

  async function onSubmit(e: any) {
    e.preventDefault();

    try {
      setIsLoading(true);

      const payload: LoginRequestDTO = { email: email, password: password }

      const response = await fetch(`${API_BASE_URL}/Auth/login`, {
        method: "POST",
        credentials: "include",
        body: JSON.stringify(payload),
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (response.ok) {
        router.replace("/home");
      } else {
        const json = await response.json();
        throw new Error(json.detail || "Login failed");
      }
    } catch (error: any) {
      toast.error(error.message);
      console.log(error);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center p-4">
      <Card className="w-full max-w-md border-0 shadow-2xl">
        <CardHeader className="space-y-3 text-center">
          <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-primary/15">
            <Users className="h-8 w-8 text-primary" />
          </div>
          <div>
            <CardTitle className="text-2xl font-semibold text-foreground">
              Local Zero
            </CardTitle>
          </div>
        </CardHeader>
        <CardContent>
          <form onSubmit={onSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="password">Password</Label>
              <Input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>
            <Button
              disabled={isLoading}
              type="submit"
              className="w-full"
              size="lg"
            >
              Log in
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
