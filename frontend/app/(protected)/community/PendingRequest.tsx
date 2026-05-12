import { Button } from "@/components/ui/button";
import { Clock, X, Search } from "lucide-react";
import Link from "next/link";

interface PendingRequestProps {
  communityName: string;
  onCancel: () => void;
  isCancelling: boolean;
}

export function PendingRequest({
  communityName,
  onCancel,
  isCancelling,
}: PendingRequestProps) {
  return (
    <div className="flex flex-col items-center gap-5">
      <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-amber-100 dark:bg-amber-900/30">
        <Clock className="h-6 w-6 text-amber-600 dark:text-amber-400" />
      </div>
      <div className="flex flex-col items-center gap-1 text-center">
        <h1 className="text-xl font-semibold">Join request pending</h1>
        <p className="text-sm text-muted-foreground">
          Waiting for <span className="font-medium text-foreground">{communityName}</span> to review your request.
        </p>
      </div>
      <div className="flex flex-col gap-2 w-full max-w-xs">
        <Button
          variant="destructive"
          onClick={onCancel}
          disabled={isCancelling}
          className="w-full"
        >
          <X className="h-4 w-4" />
          {isCancelling ? "Cancelling…" : "Cancel request"}
        </Button>
        <Link href="community/search">
          <Button variant="outline" className="w-full">
            <Search className="h-4 w-4" />
            Browse other communities
          </Button>
        </Link>
      </div>
    </div>
  );
}
