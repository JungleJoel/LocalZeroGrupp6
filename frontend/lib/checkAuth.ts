import { cookies } from "next/headers";
import { API_BASE_URL } from "@/lib/config";
import { UserDTO } from "@/types/userDTO";

export default async function checkAuth(): Promise<UserDTO | null> {
  const cookieStore = await cookies();

  const sessionCookie = cookieStore.get("LocalZeroCookie");

  if (!sessionCookie) {
    return null;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/Auth/check-auth`, {
      cache: "no-cache",
      method: "GET",
      headers: {
        Cookie: `${sessionCookie.name}=${sessionCookie.value}`,
      },
    });

    if (response.ok) {
      return await response.json();
    } else {
      return null;
    }
  } catch (error) {
    console.log(error);
    return null;
  }
}
