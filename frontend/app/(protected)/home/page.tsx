import { Button } from "@/components/ui/button";
import Link from "next/link";

export default function HomePage() {
  return (
    <div className="mx-auto flex min-h-screen w-full max-w-5xl flex-col justify-center gap-8 px-6 py-12">
      <div className="space-y-3">
        <p className="text-sm uppercase tracking-[0.3em] text-muted-foreground">
          Local Zero
        </p>
        <h1 className="text-4xl font-semibold tracking-tight sm:text-5xl">
          Your community dashboard
        </h1>
        <p className="max-w-2xl text-muted-foreground sm:text-lg">
          Track initiatives, log eco-actions, and jump into your community from one place.
        </p>
      </div>

      <div className="flex flex-wrap gap-3">
        <Link href="/community">
          <Button>Open Community</Button>
        </Link>
        <Link href="/ecoactions">
          <Button variant="secondary">Log Eco Actions</Button>
        </Link>
        <Link href="/initiatives">
          <Button variant="outline">Browse Initiatives</Button>
        </Link>
      </div>
    </div>
  );
}