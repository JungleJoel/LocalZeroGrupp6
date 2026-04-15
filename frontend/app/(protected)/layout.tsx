import checkAuth from "@/lib/checkAuth";
import { redirect } from "next/navigation";

export default async function ProtectedLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  let isAuthenticated = false;

  isAuthenticated = await checkAuth();

  if (!isAuthenticated) {
    redirect("/");
  }

  return children;
}
