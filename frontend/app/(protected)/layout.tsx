import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar";
import checkAuth from "@/lib/checkAuth";
import { redirect } from "next/navigation";
import { AppSidebar } from "./AppSidebar";

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

  return (
    <SidebarProvider>
      <AppSidebar />
      <SidebarTrigger className="absolute m-2 flex md:hidden" />
      <div className="w-screen">{children}</div>
    </SidebarProvider>
  );
}
