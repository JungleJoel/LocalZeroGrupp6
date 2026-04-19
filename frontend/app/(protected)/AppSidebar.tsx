"use client";
import { LogOutButton } from "@/components/LogOutButton";
import { ToggleThemeButton } from "@/components/ToggleThemeButton";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar";
import { UserDTO } from "@/types/userDTO";
import {
  HeartHandshake,
  Home,
  Leaf,
  MessageSquare,
  Settings,
} from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";

const navItems = [
  { label: "Home", href: "/home", icon: Home },
  { label: "Community", href: "/community", icon: HeartHandshake },
  { label: "Initiatives", href: "/initiatives", icon: Leaf },
  { label: "Messages", href: "/messages", icon: MessageSquare },
  { label: "Settings", href: "/settings", icon: Settings },
];

export function AppSidebar({user}:{user: UserDTO}) {
  const pathname = usePathname();

  return (
    <Sidebar>
      <SidebarHeader className="px-5 py-6">
        <div className="flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-primary text-white">
            <Leaf className="h-5 w-5" />
          </div>
          <div className="flex flex-col gap-0.5 leading-none">
            <span className="text-base font-semibold tracking-tight">
              Local Zero
            </span>
            <span className="text-sm text-muted-foreground">
              Welcome, {user.firstName} 
            </span>
          </div>
        </div>
      </SidebarHeader>

      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
            <SidebarMenu>
              {navItems.map(({ label, href, icon: Icon }) => (
                <SidebarMenuItem key={href}>
                  <SidebarMenuButton asChild size="lg" isActive={pathname === href}>
                    <Link href={href} className="gap-3 py-5">
                      <Icon className="h-6 w-6" />
                      <span className="text-base">{label}</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>

      <SidebarFooter className="px-4 py-4">
        <div className="flex flex-row gap-2">
          <LogOutButton className="flex-1" />
          <ToggleThemeButton />
        </div>
      </SidebarFooter>
    </Sidebar>
  );
}
