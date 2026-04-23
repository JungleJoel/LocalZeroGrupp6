import { Button } from "@/components/ui/button";
import { Info, Search } from "lucide-react";

export function NotAMember() {
  return (
    <div className="flex flex-col items-center gap-5">
      <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-primary/60 backdrop-blur-sm">
        <Info className="h-6 w-6 text-white" />
      </div>
      <h1 className="text-xl font-semibold">
        You have not joined a community yet.
      </h1>
      <Button className="w-full">
        <Search /> Find your community
      </Button>
    </div>
  );
}
