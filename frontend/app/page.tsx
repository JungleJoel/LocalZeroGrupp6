import { Button } from "@/components/ui/button";
import Link from "next/link";

export default function Page() {
  return (
    <div className="flex h-screen flex-col items-center justify-center gap-3">
      <h1 className="text-xl">Local Zero</h1>
      <Link href="/auth/login">
        <Button>/auth/login</Button>
      </Link>
      <Link href="/community">
        <Button>/community (protected)</Button>
      </Link>
    </div>
  );
}