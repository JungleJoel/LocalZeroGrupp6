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
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { UserDTO } from "@/types/userDTO";
import {
  Bike,
  Home,
  HeartHandshake,
  Leaf,
  ListTree,
  MessageSquare,
  PlusCircle,
  Settings,
} from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";

const navItems = [
  { label: "Home", href: "/home", icon: Home },
  { label: "Community", href: "/community", icon: HeartHandshake },
  /*{ label: "Create Initiative", href: "/createInitiatives", icon: PlusCircle },
  { label: "Initiatives", href: "/initiatives", icon: Leaf },*/
  { label: "Eco Actions", href: "/ecoactions", icon: Bike },
  { label: "Messages", href: "/messages", icon: MessageSquare },
  { label: "Settings", href: "/account/settings", icon: Settings },
];

export function AppSidebar({user}:{user: UserDTO}) {
  const pathname = usePathname();

  return (
    <Sidebar>
      <SidebarHeader className="px-5 py-6">
        <div className="flex items-center gap-3">
          <Avatar size="lg" className="rounded-xl">
            <AvatarImage src={user.avatarImageUrl ?? undefined} alt={user.firstName} />
            <AvatarFallback className="rounded-xl bg-primary text-white font-semibold">
              {user.firstName[0]}{user.lastName[0]}
            </AvatarFallback>
          </Avatar>
          <div className="flex flex-col gap-0.5 leading-none">
            <span className="text-base font-semibold tracking-tight">
              Welcome, {user.firstName}
            </span>
            <span className="text-sm text-muted-foreground">
              <span className="text-primary font-bold">{user.ecoPoints}</span> Eco Point{user.ecoPoints == 1 ? null : "s"} 
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
